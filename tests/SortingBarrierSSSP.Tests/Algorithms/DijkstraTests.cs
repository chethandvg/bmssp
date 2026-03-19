using SortingBarrierSSSP.Algorithms;
using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Tests.Algorithms;

public class DijkstraTests
{
    private readonly DijkstraAlgorithm _dijkstra = new();

    [Fact]
    public void SingleVertex_DistanceIsZero()
    {
        var g = new DirectedGraph(1);
        var result = _dijkstra.Solve(g, 0);
        Assert.Equal(0.0, result.Distances[0]);
        Assert.Equal(-1, result.Predecessors[0]);
    }

    [Fact]
    public void LinearChain_CorrectDistances()
    {
        // 0 →1→ 1 →2→ 2 →3→ 3
        var g = new DirectedGraph(4);
        g.AddEdge(0, 1, 1.0);
        g.AddEdge(1, 2, 2.0);
        g.AddEdge(2, 3, 3.0);

        var result = _dijkstra.Solve(g, 0);
        Assert.Equal(0.0, result.Distances[0]);
        Assert.Equal(1.0, result.Distances[1]);
        Assert.Equal(3.0, result.Distances[2]);
        Assert.Equal(6.0, result.Distances[3]);
    }

    [Fact]
    public void ShortcutPath_FindsShorterRoute()
    {
        // 0 →10→ 1 →10→ 2
        // 0 →5→  2
        var g = new DirectedGraph(3);
        g.AddEdge(0, 1, 10.0);
        g.AddEdge(1, 2, 10.0);
        g.AddEdge(0, 2, 5.0);

        var result = _dijkstra.Solve(g, 0);
        Assert.Equal(5.0, result.Distances[2]);
        Assert.Equal(0, result.Predecessors[2]); // direct edge
    }

    [Fact]
    public void UnreachableVertex_InfiniteDistance()
    {
        var g = new DirectedGraph(3);
        g.AddEdge(0, 1, 1.0);
        // vertex 2 is unreachable

        var result = _dijkstra.Solve(g, 0);
        Assert.Equal(double.PositiveInfinity, result.Distances[2]);
    }

    [Fact]
    public void DiamondGraph_CorrectShortestPath()
    {
        //     1
        //   / | \
        //  1  |  4
        // /   |   \
        // 0   2   3
        // \   |   /
        //  3  |  1
        //   \ | /
        //     2
        var g = new DirectedGraph(4);
        g.AddEdge(0, 1, 1.0);
        g.AddEdge(0, 2, 3.0);
        g.AddEdge(1, 3, 4.0);
        g.AddEdge(2, 3, 1.0);

        var result = _dijkstra.Solve(g, 0);
        Assert.Equal(0.0, result.Distances[0]);
        Assert.Equal(1.0, result.Distances[1]);
        Assert.Equal(3.0, result.Distances[2]);
        Assert.Equal(4.0, result.Distances[3]); // 0→2→3 = 3+1 = 4 < 0→1→3 = 1+4 = 5
    }

    [Fact]
    public void ZeroWeightEdges_Handled()
    {
        var g = new DirectedGraph(3);
        g.AddEdge(0, 1, 0.0);
        g.AddEdge(1, 2, 0.0);

        var result = _dijkstra.Solve(g, 0);
        Assert.Equal(0.0, result.Distances[0]);
        Assert.Equal(0.0, result.Distances[1]);
        Assert.Equal(0.0, result.Distances[2]);
    }

    [Fact]
    public void Grid_AllReachable()
    {
        var g = GraphGenerator.Grid(3, 3, seed: 42);
        var result = _dijkstra.Solve(g, 0);

        for (int i = 0; i < g.VertexCount; i++)
            Assert.True(result.Distances[i] < double.PositiveInfinity, $"Vertex {i} should be reachable");
    }

    [Fact]
    public void Metrics_ArePopulated()
    {
        var g = GraphGenerator.RandomSparse(100, 200, seed: 42);
        var result = _dijkstra.Solve(g, 0);

        Assert.True(result.Metrics.EdgeRelaxations > 0);
        Assert.True(result.Metrics.HeapOperations > 0);
        Assert.True(result.Metrics.ElapsedTime > TimeSpan.Zero);
    }
}
