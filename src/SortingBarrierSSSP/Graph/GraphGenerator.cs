namespace SortingBarrierSSSP.Graph;

/// <summary>
/// Generates various types of directed graphs for testing and benchmarking.
/// </summary>
public static class GraphGenerator
{
    /// <summary>
    /// Creates a random sparse directed graph.
    /// Ensures all vertices are reachable from vertex 0 by first creating a random spanning tree.
    /// </summary>
    public static DirectedGraph RandomSparse(int n, int extraEdges, double maxWeight = 100.0, int? seed = null)
    {
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var graph = new DirectedGraph(n);

        // Create a random spanning tree to ensure connectivity from vertex 0
        var perm = Enumerable.Range(0, n).ToArray();
        Shuffle(perm, rng);
        // perm[0] is the root; add edges along the permutation
        for (int i = 1; i < n; i++)
        {
            double w = rng.NextDouble() * maxWeight;
            graph.AddEdge(perm[i - 1], perm[i], w);
        }

        // Add extra random edges
        for (int i = 0; i < extraEdges; i++)
        {
            int from = rng.Next(n);
            int to = rng.Next(n);
            if (from == to) continue;
            double w = rng.NextDouble() * maxWeight;
            graph.AddEdge(from, to, w);
        }

        return graph;
    }

    /// <summary>
    /// Creates a directed grid graph (rows × cols) with edges going right and down.
    /// Vertex index = row * cols + col.
    /// </summary>
    public static DirectedGraph Grid(int rows, int cols, double maxWeight = 10.0, int? seed = null)
    {
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var graph = new DirectedGraph(rows * cols);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int v = r * cols + c;
                if (c + 1 < cols)
                    graph.AddEdge(v, v + 1, rng.NextDouble() * maxWeight);
                if (r + 1 < rows)
                    graph.AddEdge(v, v + cols, rng.NextDouble() * maxWeight);
            }
        }

        return graph;
    }

    /// <summary>
    /// Creates a simple linear chain: 0 → 1 → 2 → ... → n-1.
    /// </summary>
    public static DirectedGraph LinearChain(int n, double weight = 1.0)
    {
        var graph = new DirectedGraph(n);
        for (int i = 0; i < n - 1; i++)
            graph.AddEdge(i, i + 1, weight);
        return graph;
    }

    /// <summary>
    /// Creates a directed cycle: 0 → 1 → 2 → ... → n-1 → 0.
    /// </summary>
    public static DirectedGraph Cycle(int n, double weight = 1.0)
    {
        var graph = new DirectedGraph(n);
        for (int i = 0; i < n; i++)
            graph.AddEdge(i, (i + 1) % n, weight);
        return graph;
    }

    /// <summary>
    /// Creates a complete directed graph (all pairs have edges).
    /// </summary>
    public static DirectedGraph Complete(int n, double maxWeight = 10.0, int? seed = null)
    {
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var graph = new DirectedGraph(n);
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                if (i != j)
                    graph.AddEdge(i, j, rng.NextDouble() * maxWeight);
        return graph;
    }

    /// <summary>
    /// Creates a star graph: vertex 0 has edges to all others.
    /// </summary>
    public static DirectedGraph Star(int n, double maxWeight = 10.0, int? seed = null)
    {
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var graph = new DirectedGraph(n);
        for (int i = 1; i < n; i++)
            graph.AddEdge(0, i, rng.NextDouble() * maxWeight);
        return graph;
    }

    private static void Shuffle(int[] array, Random rng)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
