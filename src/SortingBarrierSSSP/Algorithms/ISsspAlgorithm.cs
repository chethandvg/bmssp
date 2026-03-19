using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Algorithms;

/// <summary>
/// Result of running an SSSP algorithm.
/// </summary>
/// <param name="Distances">Shortest distance from source to each vertex. double.PositiveInfinity if unreachable.</param>
/// <param name="Predecessors">Predecessor vertex on shortest path. -1 if source or unreachable.</param>
/// <param name="Metrics">Performance metrics collected during execution.</param>
public record SsspResult(double[] Distances, int[] Predecessors, SsspMetrics Metrics);

/// <summary>
/// Performance metrics for an SSSP algorithm execution.
/// </summary>
public record SsspMetrics(
    long EdgeRelaxations,
    long HeapOperations,
    TimeSpan ElapsedTime);

/// <summary>
/// Common interface for Single-Source Shortest Path algorithms.
/// </summary>
public interface ISsspAlgorithm
{
    /// <summary>
    /// Computes shortest paths from <paramref name="source"/> to all vertices in <paramref name="graph"/>.
    /// </summary>
    SsspResult Solve(DirectedGraph graph, int source);
}
