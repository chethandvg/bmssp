namespace SortingBarrierSSSP.Graph;

/// <summary>
/// Represents a directed edge in a weighted graph.
/// </summary>
/// <param name="To">The destination vertex index.</param>
/// <param name="Weight">The non-negative edge weight.</param>
public readonly record struct Edge(int To, double Weight);
