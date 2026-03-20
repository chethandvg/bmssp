# 🏗️ Architecture

> Technical deep-dive into the codebase structure, design decisions, and algorithm mapping.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Layer Diagram](#layer-diagram)
- [Component Details](#component-details)
- [Paper-to-Code Mapping](#paper-to-code-mapping)
- [Design Decisions](#design-decisions)
- [Data Flow](#data-flow)

---

## Overview

The project is organized into a single .NET solution with two projects:

| Project | Type | Purpose |
|:--------|:-----|:--------|
| `SortingBarrierSSSP` | Console App | Core algorithms (Dijkstra, BMSSP, BucketScan), data structures, benchmarking |
| `SortingBarrierSSSP.Tests` | xUnit Test | 119 unit + integration tests |

### Design Principles

- **🎯 Faithfulness** — Code structure mirrors the paper's algorithm descriptions
- **🔍 Transparency** — Every operation is counted (edge relaxations, heap ops)
- **🧪 Testability** — Common `ISsspAlgorithm` interface enables oracle-based testing
- **📐 Simplicity** — Prefer clarity over micro-optimization (this is a research implementation)

---

## Layer Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTATION LAYER                      │
│                                                              │
│  Program.cs              MarkdownReportWriter.cs             │
│  ┌──────────────────┐    ┌──────────────────────────┐       │
│  │ Console output    │    │ Auto-generates            │       │
│  │ with formatted    │    │ benchmark-results.md      │       │
│  │ tables + colors   │    │ with tables + analysis    │       │
│  └──────────────────┘    └──────────────────────────┘       │
├─────────────────────────────────────────────────────────────┤
│                      BENCHMARKING LAYER                      │
│                                                              │
│  BenchmarkRunner.cs          BenchmarkResult.cs              │
│  ┌──────────────────────┐    ┌──────────────────────┐       │
│  │ RunFullSuite()        │    │ record BenchmarkResult │       │
│  │ RunComparison()       │    │ record BenchmarkSuite  │       │
│  │ CompareDistances()    │    │                        │       │
│  └──────────────────────┘    └──────────────────────┘       │
├─────────────────────────────────────────────────────────────┤
│                       ALGORITHM LAYER                        │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐   │
│  │              ISsspAlgorithm (interface)                │   │
│  │              SsspResult, SsspMetrics (records)         │   │
│  └─────────────────────┬────────────────────────────────┘   │
│                        │                                     │
│         ┌──────────────┼──────────────┐                     │
│         ▼              ▼              ▼                     │
│  ┌──────────────┐ ┌─────────────┐ ┌─────────────────────┐  │
│  │   Dijkstra   │ │  BucketScan │ │        BMSSP        │  │
│  │  Algorithm   │ │  Algorithm  │ │                     │  │
│  │              │ │             │ │  ┌────────────────┐  │  │
│  │  Solve()     │ │  Bucket     │ │  │ FindPivots     │  │  │
│  │  ↓           │ │  queue +    │ │  │ k-step BF      │  │  │
│  │  Extract-min │ │  mini-heaps │ │  └────────────────┘  │  │
│  │  → relax     │ │             │ │                     │  │
│  │  → repeat    │ │  δ = W/K    │ │  ┌────────────────┐  │  │
│  │              │ │  K=log(n)/2 │ │  │  PartitionData │  │  │
│  │  Uses:       │ │             │ │  │  Structure     │  │  │
│  │  BinaryMin   │ │  Uses:      │ │  └────────────────┘  │  │
│  │  Heap        │ │  BinaryMin  │ │                     │  │
│  │              │ │  Heap +     │ │  ┌────────────────┐  │  │
│  │              │ │  SortedSet  │ │  │  BaseCase()    │  │  │
│  │              │ │  Dict       │ │  │  mini-Dijkstra │  │  │
│  └──────────────┘ └─────────────┘ │  └────────────────┘  │  │
│                                    └─────────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐   │
│  │         ConstantDegreeTransform (utility)              │   │
│  │         Transform() / ExtractOriginalDistances()       │   │
│  └──────────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────────┤
│                    DATA STRUCTURE LAYER                       │
│                                                              │
│  ┌──────────────────┐  ┌──────────┐  ┌──────────────────┐  │
│  │  DirectedGraph    │  │  Edge    │  │  BinaryMinHeap   │  │
│  │  (adjacency list) │  │  (record │  │  (array-based)   │  │
│  │                   │  │  struct) │  │                  │  │
│  │  AddEdge()        │  │  To      │  │  Insert()        │  │
│  │  GetEdges()       │  │  Weight  │  │  ExtractMin()    │  │
│  │  OutDegree()      │  │          │  │  DecreaseKey()   │  │
│  │  MaxOutDegree()   │  │          │  │  Contains()      │  │
│  └──────────────────┘  └──────────┘  └──────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  GraphGenerator                                        │   │
│  │  LinearChain / RandomSparse / Grid / Star / Complete   │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

---

## Component Details

### 📁 Algorithms/

#### `ISsspAlgorithm.cs`

The common interface that both Dijkstra and BMSSP implement:

```csharp
public interface ISsspAlgorithm
{
    SsspResult Solve(DirectedGraph graph, int source);
}

public record SsspResult(double[] Distances, int[] Predecessors, SsspMetrics Metrics);
public record SsspMetrics(long EdgeRelaxations, long HeapOperations, TimeSpan ElapsedTime);
```

> **Why an interface?** This enables oracle-based testing — run both algorithms on the
> same graph and assert their `Distances` arrays match.

#### `DijkstraAlgorithm.cs`

Classic Dijkstra's algorithm with a binary min-heap. Serves as:
- ✅ The **correctness oracle** — Dijkstra is well-proven, so we trust its output
- 📊 The **performance baseline** — all BMSSP timings are compared against Dijkstra

#### `BmsspAlgorithm.cs` ⭐

The core BMSSP algorithm (Algorithm 3 from the paper). Key methods:

| Method | Paper Reference | Description |
|:-------|:----------------|:------------|
| `Solve()` | Top-level call | Initializes distances, computes k/t/levels, calls `Bmssp()` |
| `Bmssp()` | Algorithm 3 | Recursive procedure: FindPivots → Pull → Recurse → Relax |
| `BaseCase()` | Algorithm 1 | Level-0: mini-Dijkstra processing ≤ k+1 vertices |

#### `FindPivots.cs`

Algorithm 2 from the paper. Performs k rounds of Bellman-Ford-style relaxation to:
1. Discover set W (vertices reachable within k hops)
2. Build a shortest-path forest F within W
3. Identify pivots P ⊆ S (vertices with subtree size ≥ k)

#### `PartitionDataStructure.cs`

Simplified implementation of the block-based data structure from Lemma 3.1.
Uses .NET's `SortedSet<T>` internally (backed by a red-black tree).

| Operation | Description |
|:----------|:------------|
| `Insert(vertex, value)` | Add or update a vertex's distance estimate |
| `Pull()` | Extract the M smallest entries + compute separation bound |
| `BatchPrepend(items)` | Insert multiple items discovered "behind" the frontier |

#### `BucketScanAlgorithm.cs` ⚡ NEW

A novel hybrid algorithm combining Dial's bucket queue with Dijkstra's correctness guarantee:

| Component | Purpose |
|:----------|:--------|
| `Dictionary<int, List<(double, int)>>` | Bucket queue: maps bucket index → vertex list |
| `SortedSet<int>` | Tracks non-empty bucket indices for O(log B) next-bucket lookup |
| `BinaryMinHeap` | Mini-heap for intra-bucket Dijkstra (correctness guarantee) |

Key formula: `delta = maxEdgeWeight / K` where `K = max(2, ⌊log₂(n)/2⌋)`.
See [`docs/BUCKETSCAN.md`](BUCKETSCAN.md) for full documentation.

#### `ConstantDegreeTransform.cs`

Transforms any graph to constant degree (max 2) by replacing each vertex with a
zero-weight cycle. Not used in the default benchmark (adds overhead for small graphs)
but included for completeness and tested for correctness.

### 📁 DataStructures/

#### `BinaryMinHeap.cs`

Array-based binary min-heap supporting:
- `Insert(vertex, priority)` — O(log n)
- `ExtractMin()` — O(log n)
- `DecreaseKey(vertex, newPriority)` — O(log n)
- `Contains(vertex)` — O(1) via index map

Used by Dijkstra and BMSSP's base case.

### 📁 Graph/

#### `DirectedGraph.cs`

Adjacency-list representation. Each vertex stores a `List<Edge>` of outgoing edges.
Thread-safe for reads after construction.

#### `Edge.cs`

```csharp
public readonly record struct Edge(int To, double Weight);
```

A 16-byte value type — no heap allocation per edge.

#### `GraphGenerator.cs`

Generates 5 families of test graphs:

| Generator | Structure | Edge Count | Use Case |
|:----------|:----------|:-----------|:---------|
| `LinearChain(n)` | 0→1→2→...→n-1 | n-1 | Worst-case path length |
| `RandomSparse(n, extra)` | Random tree + extra edges | ~2n | Realistic sparse graphs |
| `Grid(rows, cols)` | Right + down edges | ~2n | Regular structure |
| `Star(n)` | 0→{1,2,...,n-1} | n-1 | Single-hop dominance |
| `Complete(n)` | All pairs | n(n-1) | Dense graphs |

### 📁 Benchmarking/

#### `BenchmarkRunner.cs`

Orchestrates the full benchmark suite:
1. Iterates over graph configurations (type × size)
2. For each: generates graph → runs Dijkstra → runs BMSSP → compares distances
3. Collects results into a `BenchmarkSuite`

#### `MarkdownReportWriter.cs`

Writes a structured Markdown report with:
- System info (runtime, OS, CPU)
- Summary statistics
- Detailed results table
- Performance comparison table
- Correctness verification

---

## Paper-to-Code Mapping

| Paper Concept | Code Location | Notes |
|:--------------|:--------------|:------|
| Algorithm 1 (Base Case) | `BmsspAlgorithm.BaseCase()` | Mini-Dijkstra, ≤ k+1 vertices |
| Algorithm 2 (FindPivots) | `FindPivots.Run()` | k rounds Bellman-Ford + forest analysis |
| Algorithm 3 (BMSSP) | `BmsspAlgorithm.Bmssp()` | Recursive: FindPivots → Pull → Recurse → Relax |
| Lemma 3.1 (Data Structure D) | `PartitionDataStructure` | Insert / Pull / BatchPrepend |
| Section 2 (Constant Degree) | `ConstantDegreeTransform` | Vertex → zero-weight cycle |
| k = ⌊log^(1/3)(n)⌋ | `BmsspAlgorithm.Solve()` | `_k = Math.Floor(Math.Pow(logN, 1.0/3.0))` |
| t = ⌊log^(2/3)(n)⌋ | `BmsspAlgorithm.Solve()` | `_t = Math.Floor(Math.Pow(logN, 2.0/3.0))` |
| Bound B | `Bmssp(level, bound, sourceSet)` | Upper bound parameter passed recursively |
| Frontier S | `sourceSet` parameter | HashSet of active source vertices |
| Pivot set P | `fpResult.Pivots` | Vertices with subtree size ≥ k |
| Completed set U | `completedU` | Vertices whose shortest paths are finalized |

---

## Design Decisions

### 1. Simplified Partition Data Structure

> **Decision:** Use `SortedSet<(double, int)>` instead of actual block-based linked list.
>
> **Rationale:** The paper's block structure is complex (blocks of size M with a BST index).
> Our simplified version preserves the algorithmic *flow* (Insert/Pull/BatchPrepend) while
> using .NET's built-in red-black tree. This trades some theoretical efficiency for
> implementation clarity — appropriate for a research/educational implementation.

### 2. No Constant-Degree Transform in Benchmarks

> **Decision:** Run BMSSP directly on original graphs, skip the constant-degree transform.
>
> **Rationale:** The transform expands the graph (each vertex becomes a cycle), adding
> overhead that hurts practical performance. The algorithm is correct regardless of degree.
> The transform is implemented and tested separately for completeness.

### 3. HashSet for Vertex Sets

> **Decision:** Use `HashSet<int>` for frontier S, pivot set P, and completed set U.
>
> **Rationale:** O(1) membership test is critical for the inner loops. The GC pressure
> from HashSet allocations is a known cost, but acceptable for a research implementation.
> A production version could use bitsets or pooled arrays.

### 4. `double` for Distances

> **Decision:** Use `double` (IEEE 754 64-bit) for edge weights and distances.
>
> **Rationale:** Matches the paper's real-valued weights. Comparison tolerance of 1e-9
> handles floating-point accumulation errors. `double.PositiveInfinity` naturally
> represents unreachable vertices.

### 5. Warm-Up Runs for Small Graphs

> **Decision:** Run each algorithm once before timing (for graphs ≤ 1,000 vertices).
>
> **Rationale:** JIT compilation on .NET can skew first-run timings. Warm-up ensures
> the measured run reflects steady-state performance. Skipped for large graphs to
> avoid doubling execution time.

---

## Data Flow

```
                    User runs: dotnet run
                         │
                         ▼
                    Program.Main()
                         │
                         ▼
               BenchmarkRunner.RunFullSuite()
                         │
                         ▼
          ┌──────────────────────────────┐
          │  For each (graphType, size): │
          │                              │
          │  1. GraphGenerator.XXX(size) │──→ DirectedGraph
          │                              │
          │  2. DijkstraAlgorithm.Solve()│──→ SsspResult (reference)
          │                              │
          │  3. BmsspAlgorithm.Solve()   │──→ SsspResult
          │         │                    │
          │         ├─ FindPivots.Run()  │
          │         ├─ PartitionDS ops   │
          │         ├─ Recursive Bmssp() │
          │         └─ BaseCase()        │
          │                              │
          │  4. BucketScanAlgorithm      │──→ SsspResult
          │         .Solve()             │
          │         │                    │
          │         ├─ Compute δ, K      │
          │         ├─ Bucket inserts    │
          │         └─ Mini-heap Dijkstra│
          │                              │
          │  5. CompareDistances()       │──→ (match: bool, maxError: double)
          │                              │
          │  6. Collect BenchmarkResult  │
          └──────────────────────────────┘
                         │
                         ▼
               BenchmarkSuite (all results)
                         │
              ┌──────────┴──────────┐
              ▼                     ▼
    Console.WriteLine()    MarkdownReportWriter.Write()
    (formatted tables)     (results/benchmark-results.md)
```
