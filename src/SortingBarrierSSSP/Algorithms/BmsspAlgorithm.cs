using System.Diagnostics;
using SortingBarrierSSSP.DataStructures;
using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Algorithms;

/// <summary>
/// Implementation of the BMSSP (Bounded Multi-Source Shortest Path) algorithm
/// from "Breaking the Sorting Barrier for Directed Single-Source Shortest Paths"
/// by Duan, Mao, Mao, Shu, Yin (STOC 2025).
///
/// Time complexity: O(m · log^(2/3)(n)) in the comparison-addition model.
///
/// The algorithm works by:
/// 1. (Optional) Transforming the graph to constant degree
/// 2. Running BMSSP recursively with FindPivots to reduce frontier size
/// 3. Using a partition data structure for partial sorting
/// </summary>
public sealed class BmsspAlgorithm : ISsspAlgorithm
{
    private double[] _dist = [];
    private int[] _pred = [];
    private DirectedGraph _graph = null!;
    private int _k;
    private int _t;
    private long _edgeRelaxations;
    private long _heapOperations;

    public SsspResult Solve(DirectedGraph graph, int source)
    {
        var sw = Stopwatch.StartNew();

        // For small graphs, the constant-degree transform adds overhead without benefit.
        // We skip it and work directly on the original graph.
        // The paper's O(m·log^(2/3)(n)) bound assumes constant degree (m=O(n)),
        // but the algorithm is correct regardless.
        _graph = graph;
        int n = graph.VertexCount;

        // Initialize distance estimates
        _dist = new double[n];
        _pred = new int[n];
        Array.Fill(_dist, double.PositiveInfinity);
        Array.Fill(_pred, -1);
        _dist[source] = 0.0;
        _edgeRelaxations = 0;
        _heapOperations = 0;

        // Compute parameters
        double logN = Math.Max(1.0, Math.Log2(n));
        _k = Math.Max(1, (int)Math.Floor(Math.Pow(logN, 1.0 / 3.0)));
        _t = Math.Max(1, (int)Math.Floor(Math.Pow(logN, 2.0 / 3.0)));
        int levels = Math.Max(1, (int)Math.Ceiling(logN / _t));

        // Top-level call: BMSSP(l=levels, B=∞, S={source})
        var sourceSet = new HashSet<int> { source };
        Bmssp(levels, double.PositiveInfinity, sourceSet);

        sw.Stop();
        return new SsspResult(
            _dist,
            _pred,
            new SsspMetrics(_edgeRelaxations, _heapOperations, sw.Elapsed));
    }

    /// <summary>
    /// The core BMSSP procedure (Algorithm 3 from the paper).
    /// Returns (actualBound B', completedSet U).
    /// </summary>
    private (double ActualBound, HashSet<int> CompletedSet) Bmssp(
        int level,
        double bound,
        HashSet<int> sourceSet)
    {
        // Base case: level 0 — run mini-Dijkstra from the single source
        if (level == 0)
        {
            return BaseCase(bound, sourceSet);
        }

        // Step 1: FindPivots — shrink S to a smaller pivot set P
        var fpResult = FindPivots.Run(_graph, _dist, _pred, bound, sourceSet, _k);
        var pivots = fpResult.Pivots;      // P
        var pivotW = fpResult.W;           // W

        // Step 2: Initialize partition data structure D
        int m = Math.Max(1, (int)Math.Pow(2, (level - 1) * _t));
        // Clamp M to avoid absurdly large values for large levels
        m = Math.Min(m, _graph.VertexCount);

        var ds = new PartitionDataStructure(m, bound);

        // Insert pivots into D
        foreach (int x in pivots)
        {
            if (_dist[x] < bound)
                ds.Insert(x, _dist[x]);
        }

        double actualBound = pivots.Count > 0
            ? pivots.Min(x => _dist[x])
            : bound;

        var completedU = new HashSet<int>();
        long maxWorkload = (long)_k * Math.Max(1, Math.Min((long)Math.Pow(2, level * _t), _graph.VertexCount));

        // Step 3: Iterative loop — pull from D, recurse, relax
        while (completedU.Count < maxWorkload && !ds.IsEmpty)
        {
            // Pull smallest M entries from D
            var (pulled, pullBound) = ds.Pull();
            if (pulled.Count == 0) break;

            var si = new HashSet<int>(pulled.Select(e => e.Vertex));

            // Recursive call: BMSSP(level-1, pullBound, S_i)
            var (actualBi, ui) = Bmssp(level - 1, pullBound, si);

            // Add completed vertices
            foreach (int v in ui)
                completedU.Add(v);

            // Relax edges from completed vertices in U_i
            var kSet = new List<(int Vertex, double Value)>();

            foreach (int u in ui)
            {
                foreach (var edge in _graph.GetEdges(u))
                {
                    _edgeRelaxations++;
                    double newDist = _dist[u] + edge.Weight;
                    int v = edge.To;

                    if (newDist <= _dist[v])
                    {
                        _dist[v] = newDist;
                        _pred[v] = u;

                        if (newDist >= pullBound && newDist < bound)
                        {
                            // Insert directly into D
                            ds.Insert(v, newDist);
                        }
                        else if (newDist >= actualBi && newDist < pullBound)
                        {
                            // Record for BatchPrepend
                            kSet.Add((v, newDist));
                        }
                    }
                }
            }

            // BatchPrepend: K ∪ {x ∈ S_i : d̂[x] ∈ [B'_i, B̃_i)}
            foreach (var (vertex, _) in pulled)
            {
                if (_dist[vertex] >= actualBi && _dist[vertex] < pullBound)
                {
                    kSet.Add((vertex, _dist[vertex]));
                }
            }

            if (kSet.Count > 0)
                ds.BatchPrepend(kSet);

            actualBound = actualBi;
        }

        // Final: add vertices from W that are within the actual bound
        actualBound = ds.IsEmpty ? Math.Min(actualBound, bound) : actualBound;
        if (ds.IsEmpty)
            actualBound = bound; // successful execution

        foreach (int x in pivotW)
        {
            if (_dist[x] < actualBound)
                completedU.Add(x);
        }

        return (actualBound, completedU);
    }

    /// <summary>
    /// Base case (Algorithm 1): level 0, S is a singleton.
    /// Run mini-Dijkstra from the single source, finding at most k+1 closest vertices.
    /// </summary>
    private (double ActualBound, HashSet<int> CompletedSet) BaseCase(
        double bound,
        HashSet<int> sourceSet)
    {
        var u0 = new HashSet<int>(sourceSet);
        var heap = new BinaryMinHeap();

        foreach (int x in sourceSet)
        {
            if (_dist[x] < bound)
            {
                heap.Insert(x, _dist[x]);
                _heapOperations++;
            }
        }

        while (heap.Count > 0 && u0.Count < _k + 1)
        {
            var (u, du) = heap.ExtractMin();
            _heapOperations++;
            u0.Add(u);

            foreach (var edge in _graph.GetEdges(u))
            {
                _edgeRelaxations++;
                double newDist = _dist[u] + edge.Weight;
                int v = edge.To;

                if (newDist <= _dist[v] && newDist < bound)
                {
                    _dist[v] = newDist;
                    _pred[v] = u;

                    if (heap.Contains(v))
                    {
                        heap.DecreaseKey(v, newDist);
                    }
                    else if (!u0.Contains(v))
                    {
                        heap.Insert(v, newDist);
                    }
                    _heapOperations++;
                }
            }
        }

        if (u0.Count <= _k)
        {
            return (bound, u0);
        }
        else
        {
            double maxDist = u0.Max(v => _dist[v]);
            var filtered = new HashSet<int>(u0.Where(v => _dist[v] < maxDist));
            return (maxDist, filtered);
        }
    }
}
