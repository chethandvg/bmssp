using SortingBarrierSSSP.Algorithms;
using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Benchmarking;

/// <summary>
/// Runs side-by-side benchmarks of Dijkstra vs BMSSP on various graph types.
/// </summary>
public static class BenchmarkRunner
{
    private const double Tolerance = 1e-9;

    /// <summary>
    /// Runs a single comparison: Dijkstra vs BMSSP on the given graph.
    /// </summary>
    public static (BenchmarkResult Dijkstra, BenchmarkResult Bmssp) RunComparison(
        DirectedGraph graph, int source, string graphType)
    {
        var dijkstra = new DijkstraAlgorithm();
        var bmssp = new BmsspAlgorithm();

        // Warm up (small graphs)
        if (graph.VertexCount <= 1000)
        {
            dijkstra.Solve(graph, source);
            bmssp.Solve(graph, source);
        }

        var dijkstraResult = dijkstra.Solve(graph, source);
        var bmsspResult = bmssp.Solve(graph, source);

        // Compare distances
        var (match, maxError) = CompareDistances(dijkstraResult.Distances, bmsspResult.Distances);

        var dijkstraBench = new BenchmarkResult(
            graphType, graph.VertexCount, graph.EdgeCount,
            "Dijkstra", dijkstraResult.Metrics.ElapsedTime,
            dijkstraResult.Metrics.EdgeRelaxations, dijkstraResult.Metrics.HeapOperations,
            match, maxError);

        var bmsspBench = new BenchmarkResult(
            graphType, graph.VertexCount, graph.EdgeCount,
            "BMSSP", bmsspResult.Metrics.ElapsedTime,
            bmsspResult.Metrics.EdgeRelaxations, bmsspResult.Metrics.HeapOperations,
            match, maxError);

        return (dijkstraBench, bmsspBench);
    }

    /// <summary>
    /// Runs the full benchmark suite across multiple graph types and sizes.
    /// </summary>
    public static BenchmarkSuite RunFullSuite(Action<string>? log = null)
    {
        var results = new List<BenchmarkResult>();
        log ??= _ => { };

        // Test configurations: (name, generator, sizes)
        // Test configurations: (name, generator, sizes)
        var configs = new (string Name, Func<int, DirectedGraph> Generator, int[] Sizes)[]
        {
            ("LinearChain", n => GraphGenerator.LinearChain(n), [10, 100, 1000, 5000, 10_000]),
            ("RandomSparse", n => GraphGenerator.RandomSparse(n, n * 2, seed: 42),
                [10, 100, 1000, 5000, 10_000, 50_000, 100_000, 500_000, 1_000_000]),
            ("Grid", n => { int side = (int)Math.Sqrt(n); return GraphGenerator.Grid(side, side, seed: 42); },
                [100, 900, 2500, 10_000, 40_000, 90_000, 250_000, 1_000_000]),
            ("Star", n => GraphGenerator.Star(n, seed: 42), [10, 100, 1000, 10_000]),
            ("Complete", n => GraphGenerator.Complete(n, seed: 42), [10, 50, 100]),
        };

        foreach (var (name, generator, sizes) in configs)
        {
            foreach (int size in sizes)
            {
                log($"Running {name} n={size}...");
                try
                {
                    var graph = generator(size);
                    var (d, b) = RunComparison(graph, 0, $"{name}(n={size})");
                    results.Add(d);
                    results.Add(b);

                    string matchStr = d.DistancesMatch ? "✓ MATCH" : $"✗ MISMATCH (err={d.MaxDistanceError:E2})";
                    log($"  Dijkstra: {d.ElapsedTime.TotalMilliseconds:F2}ms | BMSSP: {b.ElapsedTime.TotalMilliseconds:F2}ms | {matchStr}");
                }
                catch (Exception ex)
                {
                    log($"  ERROR: {ex.Message}");
                }
            }
        }

        return new BenchmarkSuite("Full Comparison Suite", results);
    }

    /// <summary>
    /// Compares two distance arrays for equality within floating-point tolerance.
    /// </summary>
    public static (bool Match, double MaxError) CompareDistances(double[] a, double[] b)
    {
        if (a.Length != b.Length)
            return (false, double.PositiveInfinity);

        double maxError = 0;
        bool match = true;

        for (int i = 0; i < a.Length; i++)
        {
            if (double.IsPositiveInfinity(a[i]) && double.IsPositiveInfinity(b[i]))
                continue;

            if (double.IsPositiveInfinity(a[i]) != double.IsPositiveInfinity(b[i]))
            {
                match = false;
                maxError = double.PositiveInfinity;
                continue;
            }

            double err = Math.Abs(a[i] - b[i]);
            maxError = Math.Max(maxError, err);
            if (err > Tolerance)
                match = false;
        }

        return (match, maxError);
    }
}
