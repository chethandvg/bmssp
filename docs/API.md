# 📚 API Reference

> Complete reference for all public types and methods in the `SortingBarrierSSSP` library.

---

## 📋 Table of Contents

- [Algorithms](#algorithms)
  - [ISsspAlgorithm](#isspalgorithm)
  - [DijkstraAlgorithm](#dijkstraalgorithm)
  - [BmsspAlgorithm](#bmsspalgorithm)
  - [FindPivots](#findpivots)
  - [PartitionDataStructure](#partitiondatastructure)
  - [ConstantDegreeTransform](#constantdegreetransform)
- [Graph](#graph)
  - [DirectedGraph](#directedgraph)
  - [Edge](#edge)
  - [GraphGenerator](#graphgenerator)
- [Data Structures](#data-structures)
  - [BinaryMinHeap](#binaryminheap)
- [Benchmarking](#benchmarking)
  - [BenchmarkRunner](#benchmarkrunner)
  - [BenchmarkResult](#benchmarkresult)
  - [MarkdownReportWriter](#markdownreportwriter)
- [Result Types](#result-types)

---

## Algorithms

### ISsspAlgorithm

```csharp
namespace SortingBarrierSSSP.Algorithms;

public interface ISsspAlgorithm
{
    SsspResult Solve(DirectedGraph graph, int source);
}
```

Common interface implemented by both `DijkstraAlgorithm` and `BmsspAlgorithm`.

| Parameter | Type | Description |
|:----------|:-----|:------------|
| `graph` | `DirectedGraph` | The input graph |
| `source` | `int` | Source vertex index (0-based) |
| **Returns** | `SsspResult` | Distances, predecessors, and metrics |

---

### DijkstraAlgorithm

```csharp
namespace SortingBarrierSSSP.Algorithms;

public sealed class DijkstraAlgorithm : ISsspAlgorithm
{
    public SsspResult Solve(DirectedGraph graph, int source);
}
```

Classic Dijkstra's algorithm using a binary min-heap.

**Complexity:** O(m log n) where m = edges, n = vertices

**Example:**

```csharp
var graph = GraphGenerator.LinearChain(100);
var dijkstra = new DijkstraAlgorithm();
var result = dijkstra.Solve(graph, source: 0);

Console.WriteLine($"Distance to vertex 99: {result.Distances[99]}");
Console.WriteLine($"Heap operations: {result.Metrics.HeapOperations}");
Console.WriteLine($"Time: {result.Metrics.ElapsedTime.TotalMilliseconds:F2}ms");
```

---

### BmsspAlgorithm

```csharp
namespace SortingBarrierSSSP.Algorithms;

public sealed class BmsspAlgorithm : ISsspAlgorithm
{
    public SsspResult Solve(DirectedGraph graph, int source);
}
```

The BMSSP algorithm from Duan, Mao, Mao, Shu, Yin (STOC 2025).

**Complexity:** O(m · log^(2/3)(n))

**Parameters computed internally:**
- `k = ⌊log^(1/3)(n)⌋` — frontier reduction factor
- `t = ⌊log^(2/3)(n)⌋` — recursion depth parameter
- `levels = ⌈log(n) / t⌉` — number of recursive levels

**Example:**

```csharp
var graph = GraphGenerator.RandomSparse(10_000, 20_000, seed: 42);
var bmssp = new BmsspAlgorithm();
var result = bmssp.Solve(graph, source: 0);

Console.WriteLine($"Distance to vertex 9999: {result.Distances[9999]}");
Console.WriteLine($"Heap operations: {result.Metrics.HeapOperations}");
// Expect significantly fewer heap ops than Dijkstra!
```

---

### FindPivots

```csharp
namespace SortingBarrierSSSP.Algorithms;

public static class FindPivots
{
    public static FindPivotsResult Run(
        DirectedGraph graph,
        HashSet<int> sourceSet,
        double[] distances,
        int k);
}
```

Algorithm 2 from the paper. Runs k rounds of Bellman-Ford relaxation to identify pivot vertices.

| Parameter | Type | Description |
|:----------|:-----|:------------|
| `graph` | `DirectedGraph` | The input graph |
| `sourceSet` | `HashSet<int>` | Current frontier vertices S |
| `distances` | `double[]` | Current distance estimates (modified in-place) |
| `k` | `int` | Number of Bellman-Ford rounds |
| **Returns** | `FindPivotsResult` | Pivots, completed vertices, forest info |

---

### PartitionDataStructure

```csharp
namespace SortingBarrierSSSP.Algorithms;

public sealed class PartitionDataStructure
{
    public PartitionDataStructure(int m, double upperBound);

    public int Count { get; }
    public bool IsEmpty { get; }

    public void Insert(int vertex, double value);
    public void BatchPrepend(IEnumerable<(int Vertex, double Value)> items);
    public (List<(int Vertex, double Value)> Entries, double UpperBound) Pull();
    public double MinValue();
}
```

Block-based data structure D from Lemma 3.1. Supports partial sorting.

| Method | Description | Complexity |
|:-------|:------------|:-----------|
| `Insert` | Add/update a vertex's distance | O(log n) |
| `Pull` | Extract M smallest entries | O(M) amortized |
| `BatchPrepend` | Insert multiple behind-frontier items | O(L log L) |
| `MinValue` | Peek at smallest value | O(1) |

**Example:**

```csharp
var ds = new PartitionDataStructure(m: 4, upperBound: 100.0);
ds.Insert(vertex: 0, value: 5.0);
ds.Insert(vertex: 1, value: 3.0);
ds.Insert(vertex: 2, value: 7.0);
ds.Insert(vertex: 3, value: 1.0);
ds.Insert(vertex: 4, value: 9.0);

var (entries, bound) = ds.Pull();
// entries = [(3, 1.0), (1, 3.0), (0, 5.0), (2, 7.0)]  — 4 smallest
// bound = 9.0  — the next smallest value remaining
```

---

### ConstantDegreeTransform

```csharp
namespace SortingBarrierSSSP.Algorithms;

public static class ConstantDegreeTransform
{
    public record TransformResult(
        DirectedGraph Graph,
        int[] OriginalToTransformed,
        int OriginalVertexCount);

    public static TransformResult Transform(DirectedGraph original);
    public static double[] ExtractOriginalDistances(
        double[] transformedDistances,
        TransformResult transform);
}
```

Transforms a graph to constant degree (≤ 2) by replacing each vertex with a zero-weight cycle.

**Example:**

```csharp
var original = GraphGenerator.Complete(10);
var result = ConstantDegreeTransform.Transform(original);

Console.WriteLine($"Original: {original.VertexCount} vertices");
Console.WriteLine($"Transformed: {result.Graph.VertexCount} vertices");
Console.WriteLine($"Max degree: {result.Graph.MaxOutDegree()}");  // ≤ 2

// Run SSSP on transformed graph, then extract original distances
var sssp = new DijkstraAlgorithm().Solve(result.Graph, result.OriginalToTransformed[0]);
double[] originalDistances = ConstantDegreeTransform.ExtractOriginalDistances(
    sssp.Distances, result);
```

---

## Graph

### DirectedGraph

```csharp
namespace SortingBarrierSSSP.Graph;

public sealed class DirectedGraph
{
    public DirectedGraph(int vertexCount);

    public int VertexCount { get; }
    public int EdgeCount { get; }

    public void AddEdge(int from, int to, double weight);
    public IReadOnlyList<Edge> GetEdges(int v);
    public int OutDegree(int v);
    public int MaxOutDegree();
}
```

Adjacency-list directed weighted graph. Vertices are indexed 0..VertexCount-1.

| Method | Description | Complexity |
|:-------|:------------|:-----------|
| `AddEdge` | Add a directed edge | O(1) amortized |
| `GetEdges` | Get outgoing edges from a vertex | O(1) |
| `OutDegree` | Number of outgoing edges | O(1) |
| `MaxOutDegree` | Maximum out-degree across all vertices | O(n) |

**Throws:**
- `ArgumentOutOfRangeException` if vertex index is out of range
- `ArgumentOutOfRangeException` if weight is negative

**Example:**

```csharp
var g = new DirectedGraph(4);
g.AddEdge(0, 1, 2.5);
g.AddEdge(0, 2, 1.0);
g.AddEdge(1, 3, 3.0);
g.AddEdge(2, 3, 4.0);

Console.WriteLine($"Vertices: {g.VertexCount}");  // 4
Console.WriteLine($"Edges: {g.EdgeCount}");        // 4
Console.WriteLine($"Degree of 0: {g.OutDegree(0)}"); // 2
```

---

### Edge

```csharp
namespace SortingBarrierSSSP.Graph;

public readonly record struct Edge(int To, double Weight);
```

A 16-byte value type representing a directed edge.

| Field | Type | Description |
|:------|:-----|:------------|
| `To` | `int` | Destination vertex index |
| `Weight` | `double` | Non-negative edge weight |

---

### GraphGenerator

```csharp
namespace SortingBarrierSSSP.Graph;

public static class GraphGenerator
{
    public static DirectedGraph LinearChain(int n, double weight = 1.0);
    public static DirectedGraph Cycle(int n, double weight = 1.0);
    public static DirectedGraph Grid(int rows, int cols, int seed = 42);
    public static DirectedGraph Star(int n, int seed = 42);
    public static DirectedGraph Complete(int n, int seed = 42);
    public static DirectedGraph RandomSparse(int n, int extraEdges, int seed = 42);
}
```

Generates test graphs for benchmarking and testing.

| Generator | Vertices | Edges | Description |
|:----------|:---------|:------|:------------|
| `LinearChain(n)` | n | n-1 | Straight line 0→1→2→...→n-1 |
| `Cycle(n)` | n | n | Ring 0→1→2→...→n-1→0 |
| `Grid(r, c)` | r×c | ~2rc | Directed grid (right + down) |
| `Star(n)` | n | n-1 | Hub vertex 0 → all others |
| `Complete(n)` | n | n(n-1) | All-pairs directed edges |
| `RandomSparse(n, e)` | n | n-1+e | Random tree + e extra edges |

**Example:**

```csharp
// Create a random sparse graph with 10,000 vertices and ~20,000 edges
var graph = GraphGenerator.RandomSparse(10_000, 10_000, seed: 42);
Console.WriteLine($"n={graph.VertexCount}, m={graph.EdgeCount}");
// Output: n=10000, m=19999
```

---

## Data Structures

### BinaryMinHeap

```csharp
namespace SortingBarrierSSSP.DataStructures;

public sealed class BinaryMinHeap
{
    public BinaryMinHeap(int capacity);

    public int Count { get; }
    public bool IsEmpty { get; }

    public void Insert(int vertex, double priority);
    public (int Vertex, double Priority) ExtractMin();
    public (int Vertex, double Priority) PeekMin();
    public void DecreaseKey(int vertex, double newPriority);
    public bool Contains(int vertex);
}
```

Array-based binary min-heap with vertex indexing for O(1) `Contains` and O(log n) `DecreaseKey`.

| Method | Description | Complexity |
|:-------|:------------|:-----------|
| `Insert` | Add vertex with priority | O(log n) |
| `ExtractMin` | Remove and return smallest | O(log n) |
| `PeekMin` | View smallest without removing | O(1) |
| `DecreaseKey` | Reduce a vertex's priority | O(log n) |
| `Contains` | Check if vertex is in heap | O(1) |

---

## Benchmarking

### BenchmarkRunner

```csharp
namespace SortingBarrierSSSP.Benchmarking;

public static class BenchmarkRunner
{
    public static BenchmarkSuite RunFullSuite();
}
```

Runs the complete benchmark suite across all graph types and sizes.

### BenchmarkResult

```csharp
namespace SortingBarrierSSSP.Benchmarking;

public record BenchmarkResult(
    string GraphType,
    int VertexCount,
    int EdgeCount,
    SsspMetrics DijkstraMetrics,
    SsspMetrics BmsspMetrics,
    bool DistancesMatch,
    double MaxError);

public record BenchmarkSuite(
    List<BenchmarkResult> Results,
    DateTime Timestamp);
```

### MarkdownReportWriter

```csharp
namespace SortingBarrierSSSP.Benchmarking;

public static class MarkdownReportWriter
{
    public static void Write(BenchmarkSuite suite, string filePath);
}
```

Writes a structured Markdown report to the specified file path.

---

## Result Types

### SsspResult

```csharp
public record SsspResult(double[] Distances, int[] Predecessors, SsspMetrics Metrics);
```

| Field | Type | Description |
|:------|:-----|:------------|
| `Distances` | `double[]` | Distance from source to each vertex. `∞` if unreachable. |
| `Predecessors` | `int[]` | Previous vertex on shortest path. `-1` if source or unreachable. |
| `Metrics` | `SsspMetrics` | Performance counters |

### SsspMetrics

```csharp
public record SsspMetrics(long EdgeRelaxations, long HeapOperations, TimeSpan ElapsedTime);
```

| Field | Type | Description |
|:------|:-----|:------------|
| `EdgeRelaxations` | `long` | Number of edge relaxation operations |
| `HeapOperations` | `long` | Number of heap insert/extract/decrease-key operations |
| `ElapsedTime` | `TimeSpan` | Wall-clock execution time |
