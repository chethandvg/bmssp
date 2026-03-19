namespace SortingBarrierSSSP.Graph;

/// <summary>
/// A directed weighted graph stored as an adjacency list.
/// Vertices are indexed 0..VertexCount-1.
/// </summary>
public sealed class DirectedGraph
{
    private readonly List<Edge>[] _adjacency;

    public int VertexCount { get; }
    public int EdgeCount { get; private set; }

    public DirectedGraph(int vertexCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(vertexCount);
        VertexCount = vertexCount;
        _adjacency = new List<Edge>[vertexCount];
        for (int i = 0; i < vertexCount; i++)
            _adjacency[i] = [];
    }

    /// <summary>Adds a directed edge from <paramref name="from"/> to <paramref name="to"/> with given weight.</summary>
    public void AddEdge(int from, int to, double weight)
    {
        ValidateVertex(from);
        ValidateVertex(to);
        ArgumentOutOfRangeException.ThrowIfNegative(weight);
        _adjacency[from].Add(new Edge(to, weight));
        EdgeCount++;
    }

    /// <summary>Returns all outgoing edges from vertex <paramref name="v"/>.</summary>
    public IReadOnlyList<Edge> GetEdges(int v)
    {
        ValidateVertex(v);
        return _adjacency[v];
    }

    /// <summary>Returns the out-degree of vertex <paramref name="v"/>.</summary>
    public int OutDegree(int v)
    {
        ValidateVertex(v);
        return _adjacency[v].Count;
    }

    /// <summary>Returns the maximum out-degree across all vertices.</summary>
    public int MaxOutDegree()
    {
        int max = 0;
        for (int i = 0; i < VertexCount; i++)
            max = Math.Max(max, _adjacency[i].Count);
        return max;
    }

    private void ValidateVertex(int v)
    {
        if (v < 0 || v >= VertexCount)
            throw new ArgumentOutOfRangeException(nameof(v), $"Vertex {v} out of range [0, {VertexCount})");
    }
}
