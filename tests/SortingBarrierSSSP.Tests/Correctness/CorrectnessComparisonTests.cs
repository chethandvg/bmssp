using SortingBarrierSSSP.Algorithms;
using SortingBarrierSSSP.Benchmarking;
using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Tests.Correctness;

/// <summary>
/// Comprehensive correctness tests: BMSSP must produce the same distances as Dijkstra
/// across a wide variety of graph types and sizes.
/// These are the "authenticity verification" tests.
/// </summary>
public class CorrectnessComparisonTests
{
    private readonly DijkstraAlgorithm _dijkstra = new();
    private readonly BmsspAlgorithm _bmssp = new();
    private const double Tolerance = 1e-9;

    private void AssertDistancesMatch(DirectedGraph graph, int source, string context)
    {
        var expected = _dijkstra.Solve(graph, source);
        var actual = _bmssp.Solve(graph, source);

        var (match, maxError) = BenchmarkRunner.CompareDistances(
            expected.Distances, actual.Distances);

        Assert.True(match,
            $"[{context}] Distance mismatch! Max error: {maxError:E3}. " +
            $"First mismatch at vertex {FindFirstMismatch(expected.Distances, actual.Distances)}");
    }

    private static int FindFirstMismatch(double[] a, double[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            if (double.IsPositiveInfinity(a[i]) != double.IsPositiveInfinity(b[i]))
                return i;
            if (!double.IsPositiveInfinity(a[i]) && Math.Abs(a[i] - b[i]) > 1e-9)
                return i;
        }
        return -1;
    }

    // === Linear chains ===

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(200)]
    public void LinearChain_AllSizes(int n) =>
        AssertDistancesMatch(GraphGenerator.LinearChain(n), 0, $"LinearChain(n={n})");

    // === Cycles ===

    [Theory]
    [InlineData(3)]
    [InlineData(10)]
    [InlineData(50)]
    public void Cycle_AllSizes(int n) =>
        AssertDistancesMatch(GraphGenerator.Cycle(n), 0, $"Cycle(n={n})");

    // === Grid graphs ===

    [Theory]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(3, 10)]
    public void Grid_VariousDimensions(int rows, int cols) =>
        AssertDistancesMatch(GraphGenerator.Grid(rows, cols, seed: 42), 0,
            $"Grid({rows}×{cols})");

    // === Stars ===

    [Theory]
    [InlineData(5)]
    [InlineData(20)]
    [InlineData(100)]
    public void Star_AllSizes(int n) =>
        AssertDistancesMatch(GraphGenerator.Star(n, seed: 42), 0, $"Star(n={n})");

    // === Complete graphs ===

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void Complete_AllSizes(int n) =>
        AssertDistancesMatch(GraphGenerator.Complete(n, seed: 42), 0, $"Complete(n={n})");

    // === Random sparse graphs with multiple seeds ===

    [Theory]
    [InlineData(20, 40, 1)]
    [InlineData(50, 100, 42)]
    [InlineData(100, 200, 123)]
    [InlineData(200, 500, 999)]
    [InlineData(500, 1000, 2025)]
    public void RandomSparse_VariousSizesAndSeeds(int n, int extra, int seed) =>
        AssertDistancesMatch(GraphGenerator.RandomSparse(n, extra, seed: seed), 0,
            $"RandomSparse(n={n}, extra={extra}, seed={seed})");

    // === Edge cases ===

    [Fact]
    public void SingleVertex_NoEdges()
    {
        var g = new DirectedGraph(1);
        AssertDistancesMatch(g, 0, "SingleVertex");
    }

    [Fact]
    public void DisconnectedGraph()
    {
        var g = new DirectedGraph(5);
        g.AddEdge(0, 1, 1.0);
        g.AddEdge(1, 2, 2.0);
        // 3, 4 unreachable
        AssertDistancesMatch(g, 0, "Disconnected");
    }

    [Fact]
    public void AllZeroWeights()
    {
        var g = new DirectedGraph(4);
        g.AddEdge(0, 1, 0.0);
        g.AddEdge(1, 2, 0.0);
        g.AddEdge(0, 2, 0.0);
        g.AddEdge(2, 3, 0.0);
        AssertDistancesMatch(g, 0, "AllZeroWeights");
    }

    [Fact]
    public void VeryLargeWeights()
    {
        var g = new DirectedGraph(3);
        g.AddEdge(0, 1, 1e15);
        g.AddEdge(1, 2, 1e15);
        g.AddEdge(0, 2, 1.5e15);
        AssertDistancesMatch(g, 0, "VeryLargeWeights");
    }

    [Fact]
    public void MixedSmallAndLargeWeights()
    {
        var g = new DirectedGraph(4);
        g.AddEdge(0, 1, 0.001);
        g.AddEdge(1, 2, 1000.0);
        g.AddEdge(0, 2, 500.0);
        g.AddEdge(2, 3, 0.001);
        AssertDistancesMatch(g, 0, "MixedWeights");
    }

    [Fact]
    public void DenseRandomGraph()
    {
        // n=30, ~870 edges (complete-ish)
        var g = GraphGenerator.RandomSparse(30, 800, seed: 42);
        AssertDistancesMatch(g, 0, "DenseRandom(n=30)");
    }

    // === Stress test with multiple random seeds ===

    [Fact]
    public void StressTest_50RandomGraphs()
    {
        for (int seed = 0; seed < 50; seed++)
        {
            var g = GraphGenerator.RandomSparse(30, 60, seed: seed);
            AssertDistancesMatch(g, 0, $"Stress(seed={seed})");
        }
    }
}
