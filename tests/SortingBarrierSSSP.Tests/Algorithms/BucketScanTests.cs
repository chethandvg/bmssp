using SortingBarrierSSSP.Algorithms;
using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Tests.Algorithms;

public class BucketScanTests
{
    private readonly BucketScanAlgorithm _bucketScan = new();
    private readonly DijkstraAlgorithm _dijkstra = new();
    private const double Tolerance = 1e-9;

    private void AssertDistancesMatch(DirectedGraph graph, int source)
    {
        var expected = _dijkstra.Solve(graph, source);
        var actual = _bucketScan.Solve(graph, source);

        Assert.Equal(expected.Distances.Length, actual.Distances.Length);
        for (int i = 0; i < expected.Distances.Length; i++)
        {
            if (double.IsPositiveInfinity(expected.Distances[i]))
            {
                Assert.True(double.IsPositiveInfinity(actual.Distances[i]),
                    $"Vertex {i}: expected ∞, got {actual.Distances[i]}");
            }
            else
            {
                Assert.True(Math.Abs(expected.Distances[i] - actual.Distances[i]) < Tolerance,
                    $"Vertex {i}: expected {expected.Distances[i]}, got {actual.Distances[i]}");
            }
        }
    }

    [Fact]
    public void SingleVertex_DistanceIsZero()
    {
        var g = new DirectedGraph(1);
        var result = _bucketScan.Solve(g, 0);
        Assert.Equal(0.0, result.Distances[0]);
    }

    [Fact]
    public void TwoVertices_SimpleEdge()
    {
        var g = new DirectedGraph(2);
        g.AddEdge(0, 1, 5.0);
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void LinearChain_MatchesDijkstra()
    {
        AssertDistancesMatch(GraphGenerator.LinearChain(10), 0);
    }

    [Fact]
    public void LinearChain_Large_MatchesDijkstra()
    {
        AssertDistancesMatch(GraphGenerator.LinearChain(100), 0);
    }

    [Fact]
    public void Cycle_MatchesDijkstra()
    {
        AssertDistancesMatch(GraphGenerator.Cycle(10), 0);
    }

    [Fact]
    public void Star_MatchesDijkstra()
    {
        AssertDistancesMatch(GraphGenerator.Star(20, seed: 42), 0);
    }

    [Fact]
    public void Grid_Small_MatchesDijkstra()
    {
        AssertDistancesMatch(GraphGenerator.Grid(3, 3, seed: 42), 0);
    }

    [Fact]
    public void Grid_Medium_MatchesDijkstra()
    {
        AssertDistancesMatch(GraphGenerator.Grid(5, 5, seed: 42), 0);
    }

    [Fact]
    public void Complete_Small_MatchesDijkstra()
    {
        AssertDistancesMatch(GraphGenerator.Complete(10, seed: 42), 0);
    }

    [Fact]
    public void RandomSparse_Small_MatchesDijkstra()
    {
        AssertDistancesMatch(GraphGenerator.RandomSparse(20, 40, seed: 42), 0);
    }

    [Fact]
    public void RandomSparse_Medium_MatchesDijkstra()
    {
        AssertDistancesMatch(GraphGenerator.RandomSparse(100, 200, seed: 42), 0);
    }

    [Fact]
    public void RandomSparse_Large_MatchesDijkstra()
    {
        AssertDistancesMatch(GraphGenerator.RandomSparse(500, 1000, seed: 42), 0);
    }

    [Fact]
    public void UnreachableVertices_MatchesDijkstra()
    {
        var g = new DirectedGraph(5);
        g.AddEdge(0, 1, 1.0);
        g.AddEdge(1, 2, 2.0);
        // vertices 3 and 4 unreachable
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void ZeroWeightEdges_MatchesDijkstra()
    {
        var g = new DirectedGraph(4);
        g.AddEdge(0, 1, 0.0);
        g.AddEdge(1, 2, 0.0);
        g.AddEdge(2, 3, 1.0);
        g.AddEdge(0, 3, 0.5);
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void DiamondGraph_MatchesDijkstra()
    {
        var g = new DirectedGraph(4);
        g.AddEdge(0, 1, 1.0);
        g.AddEdge(0, 2, 3.0);
        g.AddEdge(1, 3, 4.0);
        g.AddEdge(2, 3, 1.0);
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void ParallelPaths_MatchesDijkstra()
    {
        var g = new DirectedGraph(5);
        g.AddEdge(0, 1, 1.0);
        g.AddEdge(1, 4, 10.0);
        g.AddEdge(0, 2, 2.0);
        g.AddEdge(2, 4, 5.0);
        g.AddEdge(0, 3, 3.0);
        g.AddEdge(3, 4, 1.0);
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void Metrics_ArePopulated()
    {
        var g = GraphGenerator.RandomSparse(50, 100, seed: 42);
        var result = _bucketScan.Solve(g, 0);
        Assert.True(result.Metrics.ElapsedTime >= TimeSpan.Zero);
        Assert.True(result.Metrics.EdgeRelaxations > 0);
    }

    [Fact]
    public void HeapOps_FewerThanDijkstra()
    {
        var g = GraphGenerator.RandomSparse(200, 400, seed: 42);
        var dijkResult = _dijkstra.Solve(g, 0);
        var bktResult = _bucketScan.Solve(g, 0);
        Assert.True(bktResult.Metrics.HeapOperations <= dijkResult.Metrics.HeapOperations,
            $"BucketScan heap ops ({bktResult.Metrics.HeapOperations}) should be ≤ Dijkstra ({dijkResult.Metrics.HeapOperations})");
    }

    [Theory]
    [InlineData(42)]
    [InlineData(123)]
    [InlineData(999)]
    [InlineData(2025)]
    public void RandomSparse_MultipleSeeds_MatchesDijkstra(int seed)
    {
        AssertDistancesMatch(GraphGenerator.RandomSparse(50, 100, seed: seed), 0);
    }

    // ==========================================================================
    // Robustness tests for adversarial weight distributions
    // (Addresses Copilot feedback: "Fragility to Input Distribution")
    // ==========================================================================

    [Fact]
    public void HighlySkewedWeights_MatchesDijkstra()
    {
        // One outlier edge at 1M, rest at ~1. Tests adaptive delta for skewed distributions.
        var rng = new Random(42);
        int n = 200;
        var g = new DirectedGraph(n);
        // Spanning tree with weight 1
        for (int i = 1; i < n; i++)
            g.AddEdge(rng.Next(i), i, 1.0);
        // Extra edges with weight ~1
        for (int i = 0; i < n; i++)
            g.AddEdge(rng.Next(n), rng.Next(n), rng.NextDouble() * 2.0 + 0.5);
        // One outlier edge
        g.AddEdge(0, n - 1, 1_000_000.0);
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void ClusteredWeights_MatchesDijkstra()
    {
        // All weights in narrow range [0.1, 0.5]. Tests balanced-distribution path.
        var rng = new Random(42);
        int n = 200;
        var g = new DirectedGraph(n);
        for (int i = 1; i < n; i++)
            g.AddEdge(rng.Next(i), i, rng.NextDouble() * 0.4 + 0.1);
        for (int i = 0; i < n * 2; i++)
            g.AddEdge(rng.Next(n), rng.Next(n), rng.NextDouble() * 0.4 + 0.1);
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void IdenticalWeights_MatchesDijkstra()
    {
        // All edges have weight exactly 1.0. Tests identical-weight detection.
        int n = 100;
        var g = new DirectedGraph(n);
        var rng = new Random(42);
        for (int i = 1; i < n; i++)
            g.AddEdge(rng.Next(i), i, 1.0);
        for (int i = 0; i < n; i++)
            g.AddEdge(rng.Next(n), rng.Next(n), 1.0);
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void BimodalWeights_MatchesDijkstra()
    {
        // 50% edges weight ~1, 50% weight ~1000. Tests bimodal/histogram path.
        var rng = new Random(42);
        int n = 200;
        var g = new DirectedGraph(n);
        for (int i = 1; i < n; i++)
        {
            double w = rng.NextDouble() < 0.5
                ? rng.NextDouble() * 2.0 + 0.5
                : rng.NextDouble() * 500 + 500;
            g.AddEdge(rng.Next(i), i, w);
        }
        for (int i = 0; i < n * 2; i++)
        {
            double w = rng.NextDouble() < 0.5
                ? rng.NextDouble() * 2.0 + 0.5
                : rng.NextDouble() * 500 + 500;
            g.AddEdge(rng.Next(n), rng.Next(n), w);
        }
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void LargeWeightRange_MatchesDijkstra()
    {
        // Weights from 0.001 to 1,000,000 (9 orders of magnitude).
        var rng = new Random(42);
        int n = 200;
        var g = new DirectedGraph(n);
        for (int i = 1; i < n; i++)
        {
            double w = Math.Pow(10, rng.NextDouble() * 9 - 3); // 10^(-3) to 10^6
            g.AddEdge(rng.Next(i), i, w);
        }
        for (int i = 0; i < n * 2; i++)
        {
            double w = Math.Pow(10, rng.NextDouble() * 9 - 3);
            g.AddEdge(rng.Next(n), rng.Next(n), w);
        }
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void ExponentialWeights_MatchesDijkstra()
    {
        // Exponential distribution (common in real networks). Tests skewed path.
        var rng = new Random(42);
        int n = 200;
        var g = new DirectedGraph(n);
        for (int i = 1; i < n; i++)
            g.AddEdge(rng.Next(i), i, -Math.Log(1.0 - rng.NextDouble()) + 0.01);
        for (int i = 0; i < n * 2; i++)
            g.AddEdge(rng.Next(n), rng.Next(n), -Math.Log(1.0 - rng.NextDouble()) + 0.01);
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void PowerLawWeights_MatchesDijkstra()
    {
        // Pareto/power-law weights (social network-like). Tests skewed→harmonic path.
        var rng = new Random(42);
        int n = 200;
        var g = new DirectedGraph(n);
        for (int i = 1; i < n; i++)
        {
            double u = rng.NextDouble();
            double w = 0.1 / Math.Pow(u, 1.0 / 1.5); // Pareto(alpha=1.5, xm=0.1)
            g.AddEdge(rng.Next(i), i, w);
        }
        for (int i = 0; i < n * 2; i++)
        {
            double u = rng.NextDouble();
            double w = 0.1 / Math.Pow(u, 1.0 / 1.5);
            g.AddEdge(rng.Next(n), rng.Next(n), w);
        }
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void VeryTinyWeights_MatchesDijkstra()
    {
        // All weights very small (1e-10 range). Tests numerical stability.
        var rng = new Random(42);
        int n = 100;
        var g = new DirectedGraph(n);
        for (int i = 1; i < n; i++)
            g.AddEdge(rng.Next(i), i, rng.NextDouble() * 1e-10 + 1e-12);
        for (int i = 0; i < n; i++)
            g.AddEdge(rng.Next(n), rng.Next(n), rng.NextDouble() * 1e-10 + 1e-12);
        AssertDistancesMatch(g, 0);
    }

    [Fact]
    public void MixedZeroAndLargeWeights_MatchesDijkstra()
    {
        // Mix of zero-weight and very large weight edges.
        var rng = new Random(42);
        int n = 100;
        var g = new DirectedGraph(n);
        for (int i = 1; i < n; i++)
        {
            double w = rng.NextDouble() < 0.3 ? 0.0 : rng.NextDouble() * 10000;
            g.AddEdge(rng.Next(i), i, w);
        }
        for (int i = 0; i < n; i++)
        {
            double w = rng.NextDouble() < 0.3 ? 0.0 : rng.NextDouble() * 10000;
            g.AddEdge(rng.Next(n), rng.Next(n), w);
        }
        AssertDistancesMatch(g, 0);
    }
}
