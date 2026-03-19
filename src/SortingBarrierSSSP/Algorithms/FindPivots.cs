using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Algorithms;

/// <summary>
/// Implements the FindPivots sub-routine (Algorithm 2 from the paper).
///
/// Given a frontier set S and bound B, performs k rounds of Bellman-Ford-style relaxation
/// to identify "pivot" vertices — those whose shortest-path subtrees contain ≥ k vertices.
///
/// After FindPivots:
/// - Every vertex x in Ũ (vertices with d(x) < B reachable through S) either:
///   (a) is complete and in W, OR
///   (b) has its shortest path visiting a complete vertex in P (the pivot set)
/// - |P| ≤ |W|/k
/// - |W| = O(k·|S|)
/// </summary>
public static class FindPivots
{
    /// <summary>
    /// Result of the FindPivots procedure.
    /// </summary>
    /// <param name="Pivots">The pivot set P ⊆ S — vertices with large subtrees.</param>
    /// <param name="W">The set of vertices discovered during k-step relaxation.</param>
    public record FindPivotsResult(HashSet<int> Pivots, HashSet<int> W);

    /// <summary>
    /// Runs the FindPivots procedure (Algorithm 2).
    /// </summary>
    /// <param name="graph">The directed graph.</param>
    /// <param name="dist">Global distance estimates d̂[v]. Modified in-place during relaxation.</param>
    /// <param name="pred">Global predecessor array. Modified in-place.</param>
    /// <param name="bound">Upper bound B — only consider vertices with d̂ < B.</param>
    /// <param name="sourceSet">The frontier set S.</param>
    /// <param name="k">Parameter k = ⌊log^(1/3)(n)⌋.</param>
    /// <returns>The pivot set P and discovered set W.</returns>
    public static FindPivotsResult Run(
        DirectedGraph graph,
        double[] dist,
        int[] pred,
        double bound,
        HashSet<int> sourceSet,
        int k)
    {
        var W = new HashSet<int>(sourceSet);
        var currentLayer = new HashSet<int>(sourceSet); // W_{i-1}
        int sSize = sourceSet.Count;

        for (int step = 1; step <= k; step++)
        {
            var nextLayer = new HashSet<int>(); // W_i

            foreach (int u in currentLayer)
            {
                foreach (var edge in graph.GetEdges(u))
                {
                    int v = edge.To;
                    double newDist = dist[u] + edge.Weight;

                    // Relaxation with ≤ condition (paper requires equality for re-use across levels)
                    if (newDist <= dist[v])
                    {
                        dist[v] = newDist;
                        pred[v] = u;

                        if (newDist < bound)
                        {
                            nextLayer.Add(v);
                        }
                    }
                }
            }

            foreach (int v in nextLayer)
                W.Add(v);

            // Early termination: if W exceeds k·|S|, return P = S
            if (W.Count > k * sSize)
            {
                return new FindPivotsResult(new HashSet<int>(sourceSet), W);
            }

            currentLayer = nextLayer;
        }

        // Build forest F: edges (pred[v], v) for v in W where pred[v] is also in W
        // and d̂[v] = d̂[pred[v]] + w(pred[v], v)
        // Then find roots in S whose subtrees have ≥ k vertices.
        var subtreeSize = new Dictionary<int, int>();
        foreach (int v in W)
            subtreeSize[v] = 1;

        // Compute subtree sizes by processing vertices in reverse BFS order (leaves first).
        // We need to find, for each vertex in S, how many descendants it has in F.
        // F is defined by: edge (u,v) is in F if u,v ∈ W and d̂[v] = d̂[u] + w(u,v)
        var children = new Dictionary<int, List<int>>();
        var roots = new HashSet<int>();

        foreach (int v in W)
        {
            if (sourceSet.Contains(v) && (pred[v] == -1 || !W.Contains(pred[v])))
            {
                // v is a root in F (it's in S and its predecessor is not in W)
                roots.Add(v);
            }
            else if (pred[v] >= 0 && W.Contains(pred[v]))
            {
                if (!children.TryGetValue(pred[v], out var childList))
                {
                    childList = [];
                    children[pred[v]] = childList;
                }
                childList.Add(v);
            }
        }

        // BFS/DFS to compute subtree sizes from roots
        // Process in topological order (from leaves up)
        var topoOrder = new List<int>();
        var visited = new HashSet<int>();
        foreach (int root in roots)
            TopologicalDfs(root, children, visited, topoOrder);

        // Also process any W vertex that might be part of a tree but not reachable from roots
        foreach (int v in W)
        {
            if (!visited.Contains(v))
                TopologicalDfs(v, children, visited, topoOrder);
        }

        // Compute subtree sizes bottom-up
        for (int i = topoOrder.Count - 1; i >= 0; i--)
        {
            int v = topoOrder[i];
            if (children.TryGetValue(v, out var childList))
            {
                foreach (int child in childList)
                    subtreeSize[v] = subtreeSize.GetValueOrDefault(v, 1) + subtreeSize.GetValueOrDefault(child, 1);
            }
        }

        // Pivots: vertices in S whose subtree in F has ≥ k vertices
        var pivots = new HashSet<int>();
        foreach (int v in sourceSet)
        {
            if (subtreeSize.GetValueOrDefault(v, 1) >= k)
                pivots.Add(v);
        }

        return new FindPivotsResult(pivots, W);
    }

    private static void TopologicalDfs(int v, Dictionary<int, List<int>> children, HashSet<int> visited, List<int> order)
    {
        if (!visited.Add(v)) return;

        if (children.TryGetValue(v, out var childList))
        {
            foreach (int child in childList)
                TopologicalDfs(child, children, visited, order);
        }

        order.Add(v);
    }
}
