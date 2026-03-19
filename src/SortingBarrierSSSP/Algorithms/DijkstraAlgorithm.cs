using System.Diagnostics;
using SortingBarrierSSSP.DataStructures;
using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Algorithms;

/// <summary>
/// Classic Dijkstra's algorithm using a binary min-heap.
/// Time complexity: O(m + n·log(n)) with Fibonacci heap; O((m+n)·log(n)) with binary heap.
/// Serves as the correctness oracle and performance baseline.
/// </summary>
public sealed class DijkstraAlgorithm : ISsspAlgorithm
{
    public SsspResult Solve(DirectedGraph graph, int source)
    {
        int n = graph.VertexCount;
        var dist = new double[n];
        var pred = new int[n];
        Array.Fill(dist, double.PositiveInfinity);
        Array.Fill(pred, -1);
        dist[source] = 0.0;

        long edgeRelaxations = 0;
        long heapOps = 0;
        var sw = Stopwatch.StartNew();

        var heap = new BinaryMinHeap();
        heap.Insert(source, 0.0);
        heapOps++;

        while (heap.Count > 0)
        {
            var (u, du) = heap.ExtractMin();
            heapOps++;

            // Skip stale entries (distance already improved)
            if (du > dist[u]) continue;

            foreach (var edge in graph.GetEdges(u))
            {
                edgeRelaxations++;
                double newDist = dist[u] + edge.Weight;
                if (newDist < dist[edge.To])
                {
                    dist[edge.To] = newDist;
                    pred[edge.To] = u;

                    if (heap.Contains(edge.To))
                    {
                        heap.DecreaseKey(edge.To, newDist);
                    }
                    else
                    {
                        heap.Insert(edge.To, newDist);
                    }
                    heapOps++;
                }
            }
        }

        sw.Stop();
        return new SsspResult(dist, pred, new SsspMetrics(edgeRelaxations, heapOps, sw.Elapsed));
    }
}
