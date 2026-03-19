using SortingBarrierSSSP.Algorithms;
using SortingBarrierSSSP.Graph;

namespace SortingBarrierSSSP.Tests.Algorithms;

public class ConstantDegreeTransformTests
{
    [Fact]
    public void Transform_LinearChain_PreservesDistances()
    {
        var g = GraphGenerator.LinearChain(5);
        var transform = ConstantDegreeTransform.Transform(g);
        var dijkstra = new DijkstraAlgorithm();

        // Solve on original
        var originalResult = dijkstra.Solve(g, 0);

        // Solve on transformed
        int transformedSource = transform.OriginalToTransformed[0];
        var transformedResult = dijkstra.Solve(transform.Graph, transformedSource);

        // Extract and compare
        var extractedDist = ConstantDegreeTransform.ExtractOriginalDistances(
            transformedResult.Distances, transform);

        for (int i = 0; i < 5; i++)
        {
            Assert.True(Math.Abs(originalResult.Distances[i] - extractedDist[i]) < 1e-9,
                $"Vertex {i}: original={originalResult.Distances[i]}, extracted={extractedDist[i]}");
        }
    }

    [Fact]
    public void Transform_ReducesDegree()
    {
        // High-degree graph: star with 10 leaves
        var g = GraphGenerator.Star(11, seed: 42);
        Assert.Equal(10, g.MaxOutDegree()); // vertex 0 has degree 10

        var transform = ConstantDegreeTransform.Transform(g);
        // The transformed graph should have max out-degree ≤ 3
        // (2 for cycle edges + 1 for original edge)
        Assert.True(transform.Graph.MaxOutDegree() <= 3,
            $"Max out-degree should be ≤ 3, got {transform.Graph.MaxOutDegree()}");
    }

    [Fact]
    public void Transform_PreservesMapping()
    {
        var g = new DirectedGraph(3);
        g.AddEdge(0, 1, 1.0);
        g.AddEdge(1, 2, 2.0);

        var transform = ConstantDegreeTransform.Transform(g);
        Assert.Equal(3, transform.OriginalVertexCount);
        Assert.Equal(3, transform.OriginalToTransformed.Length);

        // Each original vertex should map to a valid vertex in the transformed graph
        for (int i = 0; i < 3; i++)
        {
            Assert.True(transform.OriginalToTransformed[i] >= 0);
            Assert.True(transform.OriginalToTransformed[i] < transform.Graph.VertexCount);
        }
    }

    [Fact]
    public void Transform_Complete_PreservesDistances()
    {
        var g = GraphGenerator.Complete(5, seed: 42);
        var transform = ConstantDegreeTransform.Transform(g);
        var dijkstra = new DijkstraAlgorithm();

        var originalResult = dijkstra.Solve(g, 0);
        int transformedSource = transform.OriginalToTransformed[0];
        var transformedResult = dijkstra.Solve(transform.Graph, transformedSource);
        var extractedDist = ConstantDegreeTransform.ExtractOriginalDistances(
            transformedResult.Distances, transform);

        for (int i = 0; i < 5; i++)
        {
            Assert.True(Math.Abs(originalResult.Distances[i] - extractedDist[i]) < 1e-9,
                $"Vertex {i}: original={originalResult.Distances[i]}, extracted={extractedDist[i]}");
        }
    }
}
