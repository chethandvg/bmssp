using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Tests.Graph;

public class DirectedGraphTests
{
    [Fact]
    public void Constructor_CreatesEmptyGraph()
    {
        var g = new DirectedGraph(5);
        Assert.Equal(5, g.VertexCount);
        Assert.Equal(0, g.EdgeCount);
    }

    [Fact]
    public void Constructor_ZeroVertices_IsValid()
    {
        var g = new DirectedGraph(0);
        Assert.Equal(0, g.VertexCount);
    }

    [Fact]
    public void Constructor_NegativeVertices_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DirectedGraph(-1));
    }

    [Fact]
    public void AddEdge_IncreasesEdgeCount()
    {
        var g = new DirectedGraph(3);
        g.AddEdge(0, 1, 1.5);
        g.AddEdge(1, 2, 2.0);
        Assert.Equal(2, g.EdgeCount);
    }

    [Fact]
    public void AddEdge_NegativeWeight_Throws()
    {
        var g = new DirectedGraph(3);
        Assert.Throws<ArgumentOutOfRangeException>(() => g.AddEdge(0, 1, -1.0));
    }

    [Fact]
    public void AddEdge_InvalidVertex_Throws()
    {
        var g = new DirectedGraph(3);
        Assert.Throws<ArgumentOutOfRangeException>(() => g.AddEdge(3, 0, 1.0));
        Assert.Throws<ArgumentOutOfRangeException>(() => g.AddEdge(0, -1, 1.0));
    }

    [Fact]
    public void GetEdges_ReturnsCorrectEdges()
    {
        var g = new DirectedGraph(3);
        g.AddEdge(0, 1, 1.0);
        g.AddEdge(0, 2, 2.0);

        var edges = g.GetEdges(0);
        Assert.Equal(2, edges.Count);
        Assert.Contains(edges, e => e.To == 1 && Math.Abs(e.Weight - 1.0) < 1e-10);
        Assert.Contains(edges, e => e.To == 2 && Math.Abs(e.Weight - 2.0) < 1e-10);
    }

    [Fact]
    public void GetEdges_NoOutgoing_ReturnsEmpty()
    {
        var g = new DirectedGraph(3);
        g.AddEdge(0, 1, 1.0);
        Assert.Empty(g.GetEdges(2));
    }

    [Fact]
    public void OutDegree_ReturnsCorrectValue()
    {
        var g = new DirectedGraph(3);
        g.AddEdge(0, 1, 1.0);
        g.AddEdge(0, 2, 2.0);
        Assert.Equal(2, g.OutDegree(0));
        Assert.Equal(0, g.OutDegree(1));
    }

    [Fact]
    public void MaxOutDegree_ReturnsCorrectValue()
    {
        var g = new DirectedGraph(4);
        g.AddEdge(0, 1, 1.0);
        g.AddEdge(0, 2, 2.0);
        g.AddEdge(0, 3, 3.0);
        g.AddEdge(1, 2, 1.0);
        Assert.Equal(3, g.MaxOutDegree());
    }

    [Fact]
    public void ZeroWeightEdge_IsAllowed()
    {
        var g = new DirectedGraph(2);
        g.AddEdge(0, 1, 0.0);
        Assert.Equal(0.0, g.GetEdges(0)[0].Weight);
    }
}
