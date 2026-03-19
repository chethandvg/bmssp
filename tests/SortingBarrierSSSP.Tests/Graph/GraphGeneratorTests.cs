using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Tests.Graph;

public class GraphGeneratorTests
{
    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public void LinearChain_CorrectStructure(int n)
    {
        var g = GraphGenerator.LinearChain(n);
        Assert.Equal(n, g.VertexCount);
        Assert.Equal(n - 1, g.EdgeCount);
        for (int i = 0; i < n - 1; i++)
        {
            var edges = g.GetEdges(i);
            Assert.Single(edges);
            Assert.Equal(i + 1, edges[0].To);
        }
        Assert.Empty(g.GetEdges(n - 1));
    }

    [Theory]
    [InlineData(5)]
    [InlineData(20)]
    public void Cycle_CorrectStructure(int n)
    {
        var g = GraphGenerator.Cycle(n);
        Assert.Equal(n, g.VertexCount);
        Assert.Equal(n, g.EdgeCount);
        for (int i = 0; i < n; i++)
        {
            var edges = g.GetEdges(i);
            Assert.Single(edges);
            Assert.Equal((i + 1) % n, edges[0].To);
        }
    }

    [Fact]
    public void Grid_CorrectVertexCount()
    {
        var g = GraphGenerator.Grid(3, 4, seed: 42);
        Assert.Equal(12, g.VertexCount);
        // 3 rows × 4 cols: each interior vertex has 2 edges (right + down)
        // Right edges: 3 × 3 = 9, Down edges: 2 × 4 = 8
        Assert.Equal(17, g.EdgeCount);
    }

    [Fact]
    public void Star_CorrectStructure()
    {
        var g = GraphGenerator.Star(5, seed: 42);
        Assert.Equal(5, g.VertexCount);
        Assert.Equal(4, g.EdgeCount);
        Assert.Equal(4, g.OutDegree(0));
        for (int i = 1; i < 5; i++)
            Assert.Equal(0, g.OutDegree(i));
    }

    [Fact]
    public void Complete_CorrectEdgeCount()
    {
        var g = GraphGenerator.Complete(5, seed: 42);
        Assert.Equal(5, g.VertexCount);
        Assert.Equal(20, g.EdgeCount); // 5 × 4 = 20
    }

    [Theory]
    [InlineData(50, 100)]
    [InlineData(100, 200)]
    public void RandomSparse_HasCorrectVertexCount(int n, int extra)
    {
        var g = GraphGenerator.RandomSparse(n, extra, seed: 42);
        Assert.Equal(n, g.VertexCount);
        // At least n-1 spanning tree edges
        Assert.True(g.EdgeCount >= n - 1);
    }

    [Fact]
    public void RandomSparse_Deterministic_WithSeed()
    {
        var g1 = GraphGenerator.RandomSparse(50, 100, seed: 123);
        var g2 = GraphGenerator.RandomSparse(50, 100, seed: 123);
        Assert.Equal(g1.EdgeCount, g2.EdgeCount);
        for (int i = 0; i < 50; i++)
        {
            var e1 = g1.GetEdges(i);
            var e2 = g2.GetEdges(i);
            Assert.Equal(e1.Count, e2.Count);
        }
    }

    [Fact]
    public void AllEdgeWeights_NonNegative()
    {
        var g = GraphGenerator.RandomSparse(100, 200, seed: 42);
        for (int i = 0; i < g.VertexCount; i++)
            foreach (var e in g.GetEdges(i))
                Assert.True(e.Weight >= 0, $"Negative weight: {e.Weight}");
    }
}
