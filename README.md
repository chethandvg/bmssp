<p align="center">
  <img src="https://img.shields.io/badge/Algorithm-BMSSP-blueviolet?style=for-the-badge" alt="Algorithm: BMSSP"/>
  <img src="https://img.shields.io/badge/NEW-BucketScan_SSSP-00D4AA?style=for-the-badge&logo=lightning&logoColor=white" alt="NEW: BucketScan SSSP"/>
  <img src="https://img.shields.io/badge/Paper-STOC%202025-orange?style=for-the-badge" alt="Paper: STOC 2025"/>
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10.0"/>
  <img src="https://img.shields.io/badge/C%23-13-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C# 13"/>
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="License: MIT"/>
</p>

<h1 align="center">🚀 Breaking the Sorting Barrier for Directed SSSP</h1>

<p align="center">
  <strong>A C# implementation of the first algorithm to break the O(m log n) barrier for<br/>
  Single-Source Shortest Paths in the comparison-addition model.</strong>
</p>

<p align="center">
  Based on <em>"Breaking the Sorting Barrier for Directed Single-Source Shortest Paths"</em><br/>
  by <strong>Ran Duan, Yongqu Mao, Xinrui Mao, Yifan Shu, Runze Yin</strong> — STOC 2025
</p>

---

## 📋 Table of Contents

- [✨ Highlights](#-highlights)
- [📖 What Is This?](#-what-is-this)
- [🏗️ Architecture](#️-architecture)
- [🚀 Quick Start](#-quick-start)
- [🧪 Running Tests](#-running-tests)
- [📊 Benchmark Results](#-benchmark-results)
- [📂 Project Structure](#-project-structure)
- [🔬 How the Algorithm Works](#-how-the-algorithm-works)
- [📝 Documentation](#-documentation)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)
- [🙏 Acknowledgments](#-acknowledgments)

---

## ✨ Highlights

| Metric | Result |
|:-------|:-------|
| ✅ **Correctness** | 29/29 benchmark configs match Dijkstra (100%) |
| ✅ **Test Coverage** | 119 unit tests — all passing |
| ✅ **Heap Reduction (BMSSP)** | Up to **49× fewer** heap operations than Dijkstra |
| ⚡ **NEW: BucketScan** | Beats Dijkstra in **both speed (1.34×) and heap ops (2×)** |
| ✅ **Scale Tested** | Graphs up to **1,000,000 vertices** |
| 📐 **Complexity** | BMSSP: O(m · log^(2/3)(n)) · BucketScan: O(m + n · log(n/B)) |

> **TL;DR:** BMSSP is a *theoretical breakthrough* (49× fewer heap ops), but Dijkstra remains faster in wall-clock time. Our **new BucketScan algorithm** bridges this gap — it's **faster than Dijkstra** on medium-to-large graphs while also using **2× fewer heap operations**.
> See the [⚡ BucketScan docs](docs/BUCKETSCAN.md) for full details and [📊 Performance Dashboard](docs/PERFORMANCE.md) for benchmarks.

---

## 📖 What Is This?

For over **50 years**, Dijkstra's algorithm (1956) with a priority queue has been the gold
standard for Single-Source Shortest Paths (SSSP). Its time complexity of O(m log n) was
widely believed to be optimal in the comparison-addition model — because finding shortest
paths requires *sorting* vertices by distance, and sorting takes Ω(n log n) time.

In 2025, **Duan, Mao, Mao, Shu, and Yin** proved this belief wrong. Their BMSSP algorithm
achieves **O(m · log^(2/3)(n))** time by cleverly avoiding full sorting — using recursive
decomposition, pivot selection, and a partition data structure that only *partially* sorts
vertices.

This repository is a **faithful C# implementation** of that algorithm, with:

- 🔄 Side-by-side comparison against Dijkstra
- ✅ Comprehensive correctness verification (97 tests)
- 📊 Benchmarks from 10 to 1,000,000 vertices
- 📝 Detailed analysis and beginner-friendly explanation

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Program.cs (Entry)                        │
│            Runs benchmarks → Console + Markdown              │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────┐  ┌──────────────────────┐ ┌───────────┐  │
│  │   Dijkstra   │  │       BMSSP          │ │ BucketScan│  │
│  │  Algorithm   │  │  ┌──────┐ ┌───────┐  │ │ Algorithm │  │
│  │              │  │  │Find  │ │Parti- │  │ │           │  │
│  │  O(m log n)  │  │  │Pivots│ │tion   │  │ │Bucket     │  │
│  │              │  │  │      │ │Data   │  │ │queue +    │  │
│  │  Uses:       │  │  │k-step│ │Struct │  │ │mini-heaps │  │
│  │  BinaryMin   │  │  │BF    │ │       │  │ │           │  │
│  │  Heap        │  │  └──────┘ └───────┘  │ │O(m+n·     │  │
│  │              │  │  O(m·log^(2/3)(n))   │ │log(n/B))  │  │
│  └──────┬───────┘  └──────────┬───────────┘ └─────┬─────┘  │
│         │                     │                    │        │
│         ▼                     ▼                    ▼        │
│  ┌──────────────────────────────────────────────────────┐   │
│  │              ISsspAlgorithm Interface                 │   │
│  │  Solve(graph, source) → SsspResult                   │   │
│  │  (Distances[], Predecessors[], Metrics)               │   │
│  └──────────────────────────────────────────────────────┘   │
│         │                     │                    │        │
│         ▼                     ▼                    ▼        │
│  ┌──────────────────────────────────────────────────────┐   │
│  │         DirectedGraph (Adjacency List)                │   │
│  │         Edge (record struct: To, Weight)              │   │
│  │         GraphGenerator (5 graph families)             │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

### Build & Run

```bash
# Clone the repository
git clone https://github.com/your-username/breaking-the-sorting-barrier.git
cd breaking-the-sorting-barrier/Implementation

# Build the solution
dotnet build

# Run the benchmark suite
dotnet run --project src/SortingBarrierSSSP
```

### Expected Output

```
╔══════════════════════════════════════════════════════════════╗
║  Breaking the Sorting Barrier for SSSP — C# Implementation  ║
║  Duan, Mao, Mao, Shu, Yin (STOC 2025)                      ║
║  Dijkstra vs BMSSP Comparison                               ║
╚══════════════════════════════════════════════════════════════╝

  Running LinearChain n=10...
    Dijkstra: 0.01ms | BMSSP: 0.04ms | ✓ MATCH
  Running RandomSparse n=100000...
    Dijkstra: 187.12ms | BMSSP: 303.15ms | ✓ MATCH
  ...

  ✓ All test cases produce identical distances to Dijkstra.
  📄 Results saved to: results/benchmark-results.md
```

Results are automatically saved to [`results/benchmark-results.md`](results/benchmark-results.md).

---

## 🧪 Running Tests

```bash
# Run all 119 tests
dotnet test

# Run specific test categories
dotnet test --filter "FullyQualifiedName~CorrectnessComparisonTests"
dotnet test --filter "FullyQualifiedName~BmsspTests"
dotnet test --filter "FullyQualifiedName~BucketScanTests"
dotnet test --filter "FullyQualifiedName~DijkstraTests"
dotnet test --filter "FullyQualifiedName~DirectedGraphTests"
```

### Test Categories

| Category | Tests | What It Verifies |
|:---------|------:|:-----------------|
| `DirectedGraphTests` | 11 | Graph data structure (add edges, degrees, validation) |
| `GraphGeneratorTests` | 10 | Graph generators (linear, grid, star, complete, random) |
| `BinaryMinHeapTests` | 12 | Priority queue (insert, extract-min, decrease-key) |
| `DijkstraTests` | 8 | Dijkstra correctness (chains, diamonds, grids, unreachable) |
| `BmsspTests` | 18 | BMSSP correctness (all graph types, multiple seeds) |
| `BucketScanTests` | 22 | BucketScan correctness (all graph types, heap reduction, metrics) |
| `PartitionDataStructureTests` | 10 | Partition DS (insert, pull, batch-prepend, duplicates) |
| `ConstantDegreeTransformTests` | 4 | Degree reduction (preserves distances, reduces degree) |
| `CorrectnessComparisonTests` | 24 | BMSSP vs Dijkstra head-to-head (including 50-graph stress test) |
| **Total** | **119** | |

---

## 📊 Benchmark Results

> Full results: [`results/benchmark-results.md`](results/benchmark-results.md)
> Beginner-friendly analysis: [`results/verdict-explained.md`](results/verdict-explained.md)
> ⚡ BucketScan algorithm: [`docs/BUCKETSCAN.md`](docs/BUCKETSCAN.md)
> 📊 Performance dashboard: [`docs/PERFORMANCE.md`](docs/PERFORMANCE.md)

### 🏆 Correctness: 29/29 — Perfect Match

All three algorithms produce **identical shortest-path distances** across all 29 test
configurations, covering 5 graph families up to 1,000,000 vertices.

### ⚡ NEW: BucketScan — Faster AND Fewer Heap Ops

```
RandomSparse — Speed Comparison (n = 1,000,000, m = 2,999,997):

  Dijkstra:   2,921 ms  ██████████████████████████████████████████████░░░░
  BMSSP:      4,387 ms  ████████████████████████████████████████████████████████████████████░░
  BucketScan: 2,176 ms  ██████████████████████████████████░░░░░░░░░░░░░░░░  ← 🏆 1.34× faster!
```

```
RandomSparse — Heap Operations (n = 1,000,000):

  Dijkstra:   2,200,229  ████████████████████████████████████████████████░░
  BMSSP:         38,297  █░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░  ← 57× fewer
  BucketScan: 1,087,905  ████████████████████████░░░░░░░░░░░░░░░░░░░░░░░░  ← 2× fewer
```

### ⚡ Heap Operations: BMSSP Dominates

BMSSP uses dramatically fewer priority-queue operations — this is the "sorting barrier" being broken:

```
RandomSparse (n = 500,000 vertices, m = 1,499,997 edges):

  Dijkstra heap ops:  1,100,457  ████████████████████████████████████████████████░░
  BMSSP heap ops:        22,496  █░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░

  → BMSSP uses 49× fewer heap operations! 🎯
```

### ⏱️ Wall-Clock Time: Dijkstra Still Faster

Despite fewer heap ops, BMSSP's recursive overhead makes it slower in practice:

```
RandomSparse — Time Comparison:

  n=1,000    Dijkstra ██░░░░░░░░  1.1ms    BMSSP █████░░░░░  3.0ms     (2.7× slower)
  n=10,000   Dijkstra ████░░░░░░  15ms     BMSSP ██████████  37ms      (2.4× slower)
  n=100,000  Dijkstra ██████░░░░  187ms    BMSSP ████████░░  303ms     (1.6× slower)
  n=500,000  Dijkstra ████████░░  1,278ms  BMSSP ██████████  1,829ms   (1.4× slower)
                                                              ↑
                                                     Gap is closing!
```

### 📈 The Trend

| Scale | BMSSP vs Dijkstra (Speed) | BMSSP Heap Ops | ⚡ BucketScan vs Dijkstra (Speed) | BucketScan Heap Ops |
|------:|:---|:---|:---|:---|
| n = 1K | 2.7× slower | 6.9× fewer | ~equal | 1.8× fewer |
| n = 10K | 2.4× slower | 16× fewer | **1.09× faster** 🏆 | 1.9× fewer |
| n = 100K | 1.6× slower | 30× fewer | **1.01× faster** 🏆 | 2.0× fewer |
| n = 500K | 1.4× slower | **49× fewer** | **1.29× faster** 🏆 | 2.0× fewer |
| n = 1M | 1.5× slower | 57× fewer | **1.34× faster** 🏆 | 2.0× fewer |

BucketScan wins on **both** speed and heap ops for medium-to-large graphs!

---

## 📂 Project Structure

```
Implementation/
│
├── 📄 README.md                          ← You are here
├── 📄 LICENSE                            ← MIT License
├── 📄 CHANGELOG.md                       ← Version history
├── 📄 CONTRIBUTING.md                    ← How to contribute
├── 📄 UNDERSTANDING_AND_PLAN.md          ← Deep dive: paper analysis + implementation plan
├── 📄 SortingBarrierSSSP.sln            ← Visual Studio solution
│
├── 📁 src/SortingBarrierSSSP/           ← Main project
│   ├── Program.cs                        ← Entry point — runs benchmarks
│   ├── SortingBarrierSSSP.csproj         ← Project file (.NET 10, C# 13)
│   │
│   ├── 📁 Algorithms/
│   │   ├── ISsspAlgorithm.cs             ← Common interface + result records
│   │   ├── DijkstraAlgorithm.cs          ← Classic Dijkstra with binary heap
│   │   ├── BmsspAlgorithm.cs             ← ⭐ Core BMSSP algorithm (recursive)
│   │   ├── BucketScanAlgorithm.cs        ← ⚡ NEW: Hybrid bucket + mini-heap SSSP
│   │   ├── FindPivots.cs                 ← Pivot selection (k-step Bellman-Ford)
│   │   ├── PartitionDataStructure.cs     ← Block-based partial-sort structure
│   │   └── ConstantDegreeTransform.cs    ← Graph → constant-degree transform
│   │
│   ├── 📁 DataStructures/
│   │   └── BinaryMinHeap.cs              ← Min-heap for Dijkstra + BMSSP base case
│   │
│   ├── 📁 Graph/
│   │   ├── DirectedGraph.cs              ← Adjacency-list directed graph
│   │   ├── Edge.cs                       ← Edge record struct (To, Weight)
│   │   └── GraphGenerator.cs             ← 5 graph generators for benchmarking
│   │
│   └── 📁 Benchmarking/
│       ├── BenchmarkRunner.cs            ← Runs Dijkstra vs BMSSP comparisons
│       ├── BenchmarkResult.cs            ← Result + suite record types
│       └── MarkdownReportWriter.cs       ← Writes results to Markdown file
│
├── 📁 tests/SortingBarrierSSSP.Tests/   ← Test project (97 tests)
│   ├── SortingBarrierSSSP.Tests.csproj   ← xUnit test project
│   ├── GlobalUsings.cs                   ← global using Xunit;
│   │
│   ├── 📁 Algorithms/                   ← Algorithm-specific tests
│   │   ├── BmsspTests.cs                 ← 18 tests for BMSSP
│   │   ├── BucketScanTests.cs            ← 22 tests for BucketScan
│   │   ├── DijkstraTests.cs              ← 8 tests for Dijkstra
│   │   ├── ConstantDegreeTransformTests.cs
│   │   └── PartitionDataStructureTests.cs
│   │
│   ├── 📁 Correctness/                  ← Head-to-head comparison tests
│   │   └── CorrectnessComparisonTests.cs ← 24 tests (BMSSP vs Dijkstra)
│   │
│   ├── 📁 DataStructures/
│   │   └── BinaryMinHeapTests.cs         ← 12 tests for min-heap
│   │
│   └── 📁 Graph/
│       ├── DirectedGraphTests.cs         ← 11 tests for graph structure
│       └── GraphGeneratorTests.cs        ← 10 tests for generators
│
└── 📁 results/                          ← Benchmark output
    ├── benchmark-results.md              ← Raw benchmark data (auto-generated)
    ├── bucket-scan-explained.md          ← BucketScan technical derivation
    └── verdict-explained.md              ← Beginner-friendly analysis
```

---

## 🔬 How the Algorithm Works

### The Problem

Given a directed graph with weighted edges and a source vertex, find the shortest path
from the source to every other vertex. This is the **Single-Source Shortest Path (SSSP)** problem.

### Why Dijkstra Has a "Sorting Barrier"

Dijkstra's algorithm maintains a priority queue of candidate vertices, always extracting
the minimum. This implicitly **sorts all vertices by distance**, requiring Ω(n log n) time.

### How BMSSP Breaks the Barrier

Instead of sorting all vertices, BMSSP uses three key ideas:

#### 1. 🎯 FindPivots — Shrink the Frontier

Run k rounds of Bellman-Ford relaxation to identify "pivot" vertices — cities whose
shortest-path subtrees contain ≥ k descendants. This reduces the frontier by a factor
of 1/k = 1/log^(1/3)(n).

```
Before FindPivots:  S = {A, B, C, D, E, F, G, H}  (8 vertices to process)
After FindPivots:   P = {B, F}                      (2 pivots — 4× reduction!)
                    W = {A, C, D, E, G, H}          (already completed)
```

#### 2. 🔄 Recursive Decomposition (BMSSP)

Divide the problem into smaller subproblems across ⌈log(n)/t⌉ levels:

```
Level 3:  BMSSP(l=3, B=∞, S={source})
              │
              ├── FindPivots → shrink S to P
              ├── Pull smallest M entries from partition DS
              │
Level 2:      ├── BMSSP(l=2, B₁, S₁)
              │       ├── FindPivots → shrink further
              │       ├── Pull from DS
Level 1:      │       ├── BMSSP(l=1, B₂, S₂)
              │       │       └── ...
Level 0:      │       │           └── BaseCase: mini-Dijkstra (≤ k+1 vertices)
              │       └── Relax edges, insert new discoveries
              └── Relax edges, insert new discoveries
```

#### 3. 📦 Partition Data Structure — Avoid Full Sorting

Instead of a fully-sorted priority queue, use a block-based structure that only
*partially* sorts elements:

| Operation | Priority Queue (Dijkstra) | Partition DS (BMSSP) |
|:----------|:--------------------------|:---------------------|
| Insert | O(log n) | O(log(N/M)) |
| Extract-Min | O(log n) | O(M) amortized |
| Batch Insert | O(k log n) | O(k log(k/M)) |

### Parameters

```
k = ⌊log^(1/3)(n)⌋    — frontier reduction factor
t = ⌊log^(2/3)(n)⌋    — recursion depth parameter
levels = ⌈log(n) / t⌉  — number of recursive levels
```

| n | k | t | levels |
|---:|---:|---:|---:|
| 100 | 1 | 3 | 3 |
| 1,000 | 2 | 4 | 3 |
| 100,000 | 2 | 5 | 4 |
| 1,000,000 | 2 | 6 | 4 |

---

## 📝 Documentation

| Document | Description |
|:---------|:------------|
| [📄 README.md](README.md) | This file — project overview and quick start |
| [⚡ BucketScan Algorithm](docs/BUCKETSCAN.md) | **NEW** — Full BucketScan algorithm docs with Mermaid diagrams |
| [📊 Performance Dashboard](docs/PERFORMANCE.md) | **NEW** — Visual 3-way benchmark comparison |
| [📊 Benchmark Results](results/benchmark-results.md) | Raw benchmark data with tables |
| [📖 Verdict Explained](results/verdict-explained.md) | Beginner-friendly analysis with examples |
| [🔬 BucketScan Explained](results/bucket-scan-explained.md) | Technical algorithm derivation |
| [🔬 Understanding & Plan](UNDERSTANDING_AND_PLAN.md) | Deep paper analysis + implementation plan |
| [🏗️ Architecture](docs/ARCHITECTURE.md) | Codebase structure and design decisions |
| [📋 Changelog](CHANGELOG.md) | Version history and changes |
| [🤝 Contributing](CONTRIBUTING.md) | How to contribute to this project |
| [📄 License](LICENSE) | MIT License |

---

## 🤝 Contributing

Contributions are welcome! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

Areas where contributions would be especially valuable:

- 🔧 **Performance optimization** — reduce constant factors in BMSSP
- 📊 **Additional graph generators** — road networks, social graphs, power-law
- 🧪 **Larger-scale testing** — test with graphs > 1M vertices (requires >8GB RAM)
- 📝 **Documentation** — improve explanations, add diagrams
- 🔬 **Comparison with other implementations** — benchmark against C/C++/Python versions

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- **Paper Authors:** Ran Duan, Yongqu Mao, Xinrui Mao, Yifan Shu, Runze Yin
  — *"Breaking the Sorting Barrier for Directed Single-Source Shortest Paths"* (STOC 2025)
- **Edsger W. Dijkstra** — for the original algorithm (1956) that stood as the benchmark for 69 years
- **Castro et al.** — for their experimental analysis (arXiv:2511.03007) providing practical context
- **Community implementations** — C, Python, Go, and C++ versions that served as references

---

<p align="center">
  <strong>⭐ If you found this useful, consider giving it a star!</strong>
</p>

<p align="center">
  <sub>Built with ❤️ and C# 13 on .NET 10</sub>
</p>
