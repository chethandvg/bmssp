# Breaking the Sorting Barrier for Directed SSSP — Understanding & Implementation Plan

## 1. Paper Understanding

### 1.1 What This Paper Solves

**Problem:** Single-Source Shortest Path (SSSP) on directed graphs with non-negative real edge weights.

Given a directed graph G = (V, E) with n vertices, m edges, a weight function w: E → ℝ≥0, and a source vertex s, find the shortest path distance from s to every other vertex.

**Result:** A deterministic O(m · log²/³(n))-time algorithm in the comparison-addition model — the first to break Dijkstra's O(m + n·log(n)) barrier on sparse graphs (where m = O(n)).

### 1.2 Why Dijkstra Has a Barrier

Dijkstra's algorithm maintains a **priority queue** (frontier) of candidate vertices, always extracting the minimum. This implicitly **sorts all vertices by distance**, requiring Ω(n·log(n)) time — the sorting lower bound.

**Key insight from the paper:** You don't need to sort ALL vertices. If you can reduce the frontier size, you reduce the sorting work.

### 1.3 The Three Core Ideas

#### Idea 1: Frontier Reduction via Pivots (FindPivots)

Instead of maintaining the entire frontier, the algorithm identifies **pivots** — vertices whose shortest-path subtrees contain ≥ k descendants (where k = ⌊log^(1/3)(n)⌋).

**How:** Run k rounds of Bellman-Ford-style relaxation from the frontier S:
- After k steps, any vertex reachable through < k intermediate vertices in the frontier is **complete** (distance found).
- The remaining incomplete vertices must depend on a pivot. Since each pivot "covers" ≥ k vertices, there are at most |Ũ|/k pivots.

This shrinks the frontier by a factor of 1/k = 1/log^(1/3)(n).

#### Idea 2: Divide-and-Conquer via BMSSP

The algorithm uses **Bounded Multi-Source Shortest Path** (BMSSP) — a recursive procedure:

- **Parameters:** level l, upper bound B, source set S (|S| ≤ 2^(l·t))
- **Goal:** Find all vertices with true distance < B whose shortest path visits S
- **Recursion:** log(n)/t levels deep, each level partitions into subproblems of size ~2^t smaller
- **Base case (l=0):** S is a singleton → run mini-Dijkstra for at most k+1 vertices

At each level:
1. Call FindPivots to shrink S → P (pivots)
2. Insert P into data structure D
3. Repeatedly Pull smallest ~M vertices from D, recurse on them
4. Relax edges from completed vertices, insert new discoveries into D
5. Stop when bound B reached or workload exceeds k·2^(l·t)

#### Idea 3: Partition Data Structure

A block-based linked list supporting:
- **Insert(key, value):** O(log(N/M)) amortized — binary search tree over block boundaries
- **BatchPrepend(L):** O(L · log(L/M)) — for vertices discovered "behind" the current frontier
- **Pull():** O(M) — extract the M smallest elements

This avoids full sorting; it only partially orders elements in blocks of size M = 2^((l-1)·t).

### 1.4 Constant-Degree Transformation

The paper assumes constant in/out-degree graphs. Any graph is transformed by replacing each vertex v with a **cycle** of vertices connected by zero-weight edges (one per neighbor), preserving shortest paths. This makes m = O(n).

### 1.5 Complexity Breakdown

| Component | Per-vertex cost | Total |
|-----------|----------------|-------|
| FindPivots (k steps × all levels) | k · (log(n)/t) | O(n · log^(2/3)(n)) |
| Data structure operations | t per insert | O(m · log^(2/3)(n)) |
| Edge relaxations | O(1) amortized | O(m · log^(2/3)(n)) |
| **Total** | | **O(m · log^(2/3)(n))** |

### 1.6 Practical Reality (from Castro et al. 2025)

The experimental analysis paper (arXiv:2511.03007) found:
- **Dijkstra is 3-4× faster** on all tested graphs (up to 10M vertices)
- Crossover point estimated at **n > 10^67** vertices (!)
- Large constant factors from: recursive overhead, FindPivots cost, data structure complexity
- The algorithm's advantage is **purely asymptotic** for astronomical graph sizes

---

## 2. Implementation Plan

### 2.1 Project Structure

```
SortingBarrierSSSP/
├── SortingBarrierSSSP.sln
├── src/
│   └── SortingBarrierSSSP/
│       ├── SortingBarrierSSSP.csproj
│       ├── Program.cs                      # Console entry point with benchmarks
│       ├── Graph/
│       │   ├── DirectedGraph.cs            # Adjacency list representation
│       │   ├── Edge.cs                     # Edge record (To, Weight)
│       │   └── GraphGenerator.cs           # Random, grid, road-like generators
│       ├── Algorithms/
│       │   ├── ISsspAlgorithm.cs           # Common interface
│       │   ├── DijkstraAlgorithm.cs        # Classic Dijkstra with binary heap
│       │   ├── BmsspAlgorithm.cs           # Main BMSSP orchestrator
│       │   ├── FindPivots.cs               # FindPivots sub-routine
│       │   ├── PartitionDataStructure.cs   # Block-based D with Insert/Pull/BatchPrepend
│       │   └── ConstantDegreeTransform.cs  # Graph → constant-degree graph transform
│       ├── DataStructures/
│       │   ├── BinaryMinHeap.cs            # For Dijkstra and base case
│       │   ├── Block.cs                    # Linked-list block for partition DS
│       │   └── RedBlackTree.cs             # Self-balancing BST for block boundaries
│       └── Benchmarking/
│           ├── BenchmarkRunner.cs          # Runs both algorithms, collects metrics
│           └── BenchmarkResult.cs          # Timing, edge relaxations, memory stats
├── tests/
│   └── SortingBarrierSSSP.Tests/
│       ├── SortingBarrierSSSP.Tests.csproj
│       ├── Graph/
│       │   ├── DirectedGraphTests.cs
│       │   └── GraphGeneratorTests.cs
│       ├── Algorithms/
│       │   ├── DijkstraTests.cs            # Correctness tests for Dijkstra
│       │   ├── BmsspTests.cs               # Correctness tests for BMSSP
│       │   ├── FindPivotsTests.cs          # Unit tests for pivot selection
│       │   ├── PartitionDSTests.cs         # Unit tests for data structure D
│       │   └── ConstantDegreeTransformTests.cs
│       ├── DataStructures/
│       │   ├── BinaryMinHeapTests.cs
│       │   └── RedBlackTreeTests.cs
│       ├── Correctness/
│       │   ├── CorrectnessComparisonTests.cs  # BMSSP vs Dijkstra on same graphs
│       │   ├── EdgeCaseTests.cs               # Single vertex, disconnected, zero-weight
│       │   └── StressTests.cs                 # Large random graphs
│       └── Performance/
│           └── PerformanceComparisonTests.cs  # Timing benchmarks with metrics
└── docs/
    └── UNDERSTANDING_AND_PLAN.md           # This file
```

### 2.2 Implementation Phases

#### Phase 1: Foundation (Graph + Dijkstra baseline)
**Files:** DirectedGraph, Edge, GraphGenerator, ISsspAlgorithm, DijkstraAlgorithm, BinaryMinHeap
**Tests:** All Graph tests, DijkstraTests, BinaryMinHeapTests
**Goal:** Working Dijkstra as the correctness oracle

#### Phase 2: Data Structures for BMSSP
**Files:** Block, RedBlackTree, PartitionDataStructure
**Tests:** RedBlackTreeTests, PartitionDSTests
**Goal:** Verified Insert/Pull/BatchPrepend operations

#### Phase 3: FindPivots
**Files:** FindPivots
**Tests:** FindPivotsTests
**Goal:** Correct pivot selection with k-step Bellman-Ford relaxation

#### Phase 4: Constant-Degree Transform
**Files:** ConstantDegreeTransform
**Tests:** ConstantDegreeTransformTests
**Goal:** Transform any graph to constant in/out-degree ≤ 2, preserving shortest paths

#### Phase 5: BMSSP Algorithm
**Files:** BmsspAlgorithm (base case + recursive case)
**Tests:** BmsspTests
**Goal:** Full BMSSP producing correct shortest-path distances

#### Phase 6: Correctness Verification
**Tests:** CorrectnessComparisonTests, EdgeCaseTests, StressTests
**Goal:** BMSSP matches Dijkstra on thousands of random graphs

#### Phase 7: Benchmarking & Comparison
**Files:** BenchmarkRunner, BenchmarkResult, Program.cs
**Tests:** PerformanceComparisonTests
**Goal:** Side-by-side timing, operation counts, scaling analysis

### 2.3 Key Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Graph representation | Adjacency list (List<Edge>[]) | Simple, supports constant-degree transform |
| Priority queue | Binary min-heap | Good practical performance, simple to implement |
| Red-Black Tree | SortedDictionary<double, Block> | .NET built-in, O(log n) operations |
| Floating-point ties | Tuple (distance, hop count, vertex id) | Matches paper's lexicographic tie-breaking |
| Constant-degree transform | Explicit cycle expansion | Required by paper; makes m = O(n) |
| Target framework | .NET 10 / C# 13 | Latest, matches project conventions |

### 2.4 Interface Design

```csharp
// Common interface for both algorithms
public interface ISsspAlgorithm
{
    SsspResult Solve(DirectedGraph graph, int source);
}

public record SsspResult(
    double[] Distances,       // Distance from source to each vertex
    int[] Predecessors,       // Shortest-path tree
    SsspMetrics Metrics       // Operation counts, timing
);

public record SsspMetrics(
    long EdgeRelaxations,
    long HeapOperations,
    TimeSpan ElapsedTime,
    long MemoryBytes
);
```

### 2.5 Algorithm Parameters

```csharp
// From the paper (Section 3)
int k = (int)Math.Floor(Math.Pow(Math.Log2(n), 1.0 / 3.0));  // ⌊log^(1/3)(n)⌋
int t = (int)Math.Floor(Math.Pow(Math.Log2(n), 2.0 / 3.0));  // ⌊log^(2/3)(n)⌋
int levels = (int)Math.Ceiling(Math.Log2(n) / t);             // ⌈log(n)/t⌉
```

For small graphs (n < 1000), k and t are tiny (k=1-2, t=2-4), so the algorithm degenerates to something close to Dijkstra — this is expected and matches the experimental findings.

### 2.6 Test Strategy

#### Correctness Tests (most important)
1. **Oracle comparison:** For every test graph, run both Dijkstra and BMSSP, assert distances match within ε = 1e-9
2. **Known answers:** Hand-crafted graphs with known shortest paths
3. **Edge cases:** Single vertex, no edges, disconnected components, zero-weight edges, self-loops
4. **Stress tests:** Random graphs (100-10000 vertices), verify BMSSP = Dijkstra

#### Component Tests
1. **FindPivots:** Verify pivot set size ≤ |W|/k, all non-pivot vertices are complete
2. **PartitionDS:** Insert/Pull/BatchPrepend maintain sorted block invariants
3. **ConstantDegreeTransform:** Transformed graph has max degree 2, preserves distances

#### Performance Tests
1. **Scaling:** Measure time for n = 100, 1K, 10K, 100K vertices
2. **Operation counts:** Compare edge relaxations, heap ops between algorithms
3. **Memory:** Track allocations for both algorithms

### 2.7 Expected Results

Based on the experimental literature:
- **Correctness:** BMSSP should produce identical distances to Dijkstra ✓
- **Performance:** Dijkstra will be 3-10× faster for all practical graph sizes
- **Operation counts:** BMSSP will have fewer "sorting" operations but more total work due to FindPivots overhead
- **Scaling trend:** The gap should narrow (slightly) as n increases, but won't cross over for any testable n

This is the expected and scientifically correct outcome — the paper's contribution is **theoretical**, not practical.

---

## 3. Additional Research Gathered

### 3.1 Existing Implementations
| Language | Repository | Notes |
|----------|-----------|-------|
| C | [danalec/DMMSY-SSSP](https://github.com/danalec/DMMSY-SSSP) | Optimized C99, claims 20,000× speedup on tree-like graphs (with AVX-512) |
| Python | [hparreao/BMSSP-Python](https://github.com/hparreao/BMSSP-Python) | Faithful implementation with tests, good reference |
| Go | [localrivet/bmssp](https://github.com/localrivet/bmssp) | Package with simple API |
| C++ | Castro et al. (arXiv:2511.03007) | Academic implementation with detailed benchmarks |

### 3.2 Key Findings from Experimental Papers
- Castro et al. (2025): Dijkstra 3-4× faster on all tested scenarios (up to 10M vertices). Crossover at ~10^67 vertices.
- The C implementation (DMMSY-SSSP) claims dramatic speedups but these appear to be on specific tree-like graphs with compiler auto-vectorization, not general directed graphs.

### 3.3 What's Still Open (from overview_to_check.txt)
1. **Hidden constants:** Making the algorithm competitive for practical graph sizes
2. **Dynamic graphs:** Adapting to real-time edge weight changes
3. **GPU parallelization:** The clustered structure could enable parallel processing

---

## 4. Verification Criteria (Authenticity)

To verify the algorithm's **authenticity** (that our implementation correctly implements the paper):

1. **Correctness:** BMSSP produces the same distances as Dijkstra on ALL test graphs
2. **Complexity behavior:** Operation count should scale as O(m · log^(2/3)(n)), not O(m · log(n))
3. **Pivot properties:** |P| ≤ |W|/k after FindPivots (Lemma 3.2 of the paper)
4. **Base case:** At level 0, processes at most k+1 vertices per call
5. **Recursion depth:** At most ⌈log(n)/t⌉ levels
6. **Workload bound:** |U| ≤ 4k · 2^(l·t) at each BMSSP call (Lemma 3.6)
