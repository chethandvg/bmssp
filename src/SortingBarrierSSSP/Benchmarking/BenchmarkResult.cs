namespace SortingBarrierSSSP.Benchmarking;

/// <summary>
/// Result of a benchmark comparison between two SSSP algorithms.
/// </summary>
public record BenchmarkResult(
    string GraphType,
    int Vertices,
    int Edges,
    string Algorithm,
    TimeSpan ElapsedTime,
    long EdgeRelaxations,
    long HeapOperations,
    bool DistancesMatch,
    double MaxDistanceError);

/// <summary>
/// Summary of a benchmark suite run.
/// </summary>
public record BenchmarkSuite(
    string Name,
    List<BenchmarkResult> Results);
