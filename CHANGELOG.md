# 📋 Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [1.0.0] — 2026-03-19

### 🎉 Initial Release

The first complete implementation of the BMSSP algorithm with full benchmark suite.

### ✨ Added

#### Core Algorithms
- **`DijkstraAlgorithm`** — Classic O(m log n) Dijkstra with binary min-heap
- **`BmsspAlgorithm`** — Full recursive BMSSP implementation with:
  - `FindPivots` — k-step Bellman-Ford pivot selection (Algorithm 2 from paper)
  - `PartitionDataStructure` — Block-based Insert/Pull/BatchPrepend structure (Lemma 3.1)
  - `ConstantDegreeTransform` — Graph transformation to constant in/out-degree
  - Base case mini-Dijkstra for level-0 recursion

#### Data Structures
- **`DirectedGraph`** — Adjacency-list directed weighted graph
- **`Edge`** — Lightweight record struct (To, Weight)
- **`BinaryMinHeap`** — Binary min-heap with Insert, ExtractMin, DecreaseKey, Contains
- **`GraphGenerator`** — 5 graph family generators:
  - `LinearChain` — Straight line: 0→1→2→...→n-1
  - `RandomSparse` — Random spanning tree + extra edges (m ≈ 2n)
  - `Grid` — rows × cols directed grid (right + down edges)
  - `Star` — Hub-and-spoke from vertex 0
  - `Complete` — All-pairs edges (m = n²)

#### Benchmarking
- **`BenchmarkRunner`** — Side-by-side Dijkstra vs BMSSP comparison
- **`MarkdownReportWriter`** — Auto-generates Markdown benchmark reports
- **Console output** — Formatted tables with correctness indicators

#### Testing — 97 Tests
- 11 `DirectedGraphTests` — graph structure validation
- 10 `GraphGeneratorTests` — generator correctness
- 12 `BinaryMinHeapTests` — heap operations
- 8 `DijkstraTests` — Dijkstra correctness
- 18 `BmsspTests` — BMSSP correctness across graph types
- 10 `PartitionDataStructureTests` — partition DS operations
- 4 `ConstantDegreeTransformTests` — degree reduction
- 24 `CorrectnessComparisonTests` — BMSSP vs Dijkstra head-to-head

#### Documentation
- `README.md` — Project overview, architecture, quick start
- `UNDERSTANDING_AND_PLAN.md` — Deep paper analysis + implementation plan
- `CONTRIBUTING.md` — Contribution guidelines
- `CHANGELOG.md` — This file
- `results/benchmark-results.md` — Auto-generated benchmark data
- `results/verdict-explained.md` — Beginner-friendly verdict with examples

### 📊 Benchmark Highlights (v1.0.0)

| Metric | Value |
|:-------|:------|
| Correctness | ✅ 28/28 configs match Dijkstra |
| Largest graph tested | 1,000,000 vertices (Grid) |
| Best heap reduction | 49× fewer ops (RandomSparse, n=500K) |
| Dijkstra speed advantage | 1.4× at n=500K (shrinking with scale) |

---

## [Unreleased]

### 🔮 Planned
- [ ] Fibonacci heap variant for Dijkstra comparison
- [ ] Real-world graph datasets (road networks, social graphs)
- [ ] Memory usage profiling and optimization
- [ ] Parallel BMSSP exploration (leveraging recursive structure)
- [ ] Interactive visualization of algorithm execution
