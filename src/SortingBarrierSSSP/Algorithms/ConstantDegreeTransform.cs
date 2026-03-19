using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Algorithms;

/// <summary>
/// Transforms an arbitrary directed graph into a constant-degree graph (max in/out-degree 2)
/// that preserves all shortest-path distances.
///
/// From the paper (Section 2, Preliminaries):
/// Each vertex v is replaced by a cycle of vertices connected by zero-weight edges,
/// one per neighbor. Original edges are reconnected to the appropriate cycle vertices.
/// </summary>
public static class ConstantDegreeTransform
{
    /// <summary>
    /// Result of the constant-degree transformation.
    /// </summary>
    /// <param name="Graph">The transformed constant-degree graph.</param>
    /// <param name="OriginalToTransformed">Maps original vertex → its first cycle vertex in the transformed graph.</param>
    /// <param name="OriginalVertexCount">Number of vertices in the original graph.</param>
    public record TransformResult(DirectedGraph Graph, int[] OriginalToTransformed, int OriginalVertexCount);

    /// <summary>
    /// Transforms the graph so that every vertex has in-degree and out-degree at most 2.
    /// The source vertex in the original graph maps to OriginalToTransformed[source] in the new graph.
    /// </summary>
    public static TransformResult Transform(DirectedGraph original)
    {
        int n = original.VertexCount;

        // Phase 1: Calculate how many cycle vertices each original vertex needs.
        // Each vertex needs one cycle vertex per incoming + outgoing neighbor.
        // For simplicity, we give each vertex max(1, inDeg + outDeg) cycle vertices.
        var inDegree = new int[n];
        var outDegree = new int[n];
        for (int v = 0; v < n; v++)
        {
            outDegree[v] = original.GetEdges(v).Count;
            foreach (var e in original.GetEdges(v))
                inDegree[e.To]++;
        }

        // cycleSize[v] = number of cycle vertices for original vertex v
        var cycleSize = new int[n];
        var cycleStart = new int[n]; // starting index in new graph
        int totalVertices = 0;
        for (int v = 0; v < n; v++)
        {
            cycleSize[v] = Math.Max(1, inDegree[v] + outDegree[v]);
            cycleStart[v] = totalVertices;
            totalVertices += cycleSize[v];
        }

        var transformed = new DirectedGraph(totalVertices);

        // Phase 2: Build the zero-weight cycle for each original vertex.
        for (int v = 0; v < n; v++)
        {
            int size = cycleSize[v];
            int start = cycleStart[v];
            for (int i = 0; i < size; i++)
            {
                int from = start + i;
                int to = start + (i + 1) % size;
                if (from != to) // skip self-loop for size=1
                {
                    transformed.AddEdge(from, to, 0.0);
                    transformed.AddEdge(to, from, 0.0); // bidirectional cycle
                }
            }
        }

        // Phase 3: Add original edges.
        // For each original vertex v, the first outDeg[v] cycle vertices handle outgoing edges,
        // and the next inDeg[v] handle incoming edges.
        var outSlot = new int[n]; // next available outgoing slot for each vertex
        var inSlot = new int[n]; // next available incoming slot for each vertex
        for (int v = 0; v < n; v++)
            inSlot[v] = outDegree[v]; // incoming slots start after outgoing

        for (int u = 0; u < n; u++)
        {
            foreach (var edge in original.GetEdges(u))
            {
                int fromCycleVertex = cycleStart[u] + outSlot[u];
                int toCycleVertex = cycleStart[edge.To] + inSlot[edge.To];
                transformed.AddEdge(fromCycleVertex, toCycleVertex, edge.Weight);
                outSlot[u]++;
                inSlot[edge.To]++;
            }
        }

        return new TransformResult(transformed, cycleStart, n);
    }

    /// <summary>
    /// Extracts distances for original vertices from the transformed graph's distance array.
    /// For each original vertex v, takes the distance to cycleStart[v] (any cycle vertex has the same distance
    /// since they're connected by zero-weight edges).
    /// </summary>
    public static double[] ExtractOriginalDistances(double[] transformedDistances, TransformResult transform)
    {
        var result = new double[transform.OriginalVertexCount];
        for (int v = 0; v < transform.OriginalVertexCount; v++)
            result[v] = transformedDistances[transform.OriginalToTransformed[v]];
        return result;
    }
}
