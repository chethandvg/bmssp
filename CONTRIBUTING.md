# 🤝 Contributing

Thank you for your interest in contributing to this project! Whether you're fixing a bug,
improving performance, adding tests, or enhancing documentation — every contribution matters.

---

## 📋 Table of Contents

- [🏁 Getting Started](#-getting-started)
- [🔧 Development Setup](#-development-setup)
- [📐 Code Style](#-code-style)
- [🧪 Testing Guidelines](#-testing-guidelines)
- [📝 Commit Messages](#-commit-messages)
- [🔀 Pull Request Process](#-pull-request-process)
- [💡 Areas for Contribution](#-areas-for-contribution)
- [🐛 Reporting Bugs](#-reporting-bugs)

---

## 🏁 Getting Started

1. **Fork** the repository
2. **Clone** your fork locally
3. **Create a branch** for your change: `git checkout -b feature/your-feature-name`
4. **Make changes**, add tests, verify everything builds and passes
5. **Submit a Pull Request** with a clear description

---

## 🔧 Development Setup

### Prerequisites

| Tool | Version | Required |
|:-----|:--------|:---------|
| .NET SDK | 10.0+ | ✅ Yes |
| Git | 2.x+ | ✅ Yes |
| IDE | VS 2022 / VS Code / Rider | Recommended |

### Build & Test

```bash
# Build the entire solution
dotnet build

# Run all 97 tests
dotnet test

# Run benchmarks
dotnet run --project src/SortingBarrierSSSP

# Run specific test category
dotnet test --filter "FullyQualifiedName~BmsspTests"
```

### Solution Structure

```
SortingBarrierSSSP.sln
├── src/SortingBarrierSSSP/         ← Main library + console app
└── tests/SortingBarrierSSSP.Tests/ ← xUnit test project
```

---

## 📐 Code Style

This project follows modern C# conventions:

### ✅ Do

- Use **C# 13** features: records, pattern matching, collection expressions, file-scoped namespaces
- Use **`readonly record struct`** for lightweight value types (e.g., `Edge`)
- Use **`sealed`** on classes not designed for inheritance
- Add **XML doc comments** (`///`) on all public types and methods
- Use **meaningful names** — `FindPivots`, not `FP`; `edgeRelaxations`, not `er`
- Keep methods **short and focused** — one method, one responsibility
- Use **`ArgumentOutOfRangeException.ThrowIfNegative()`** for guard clauses

### ❌ Don't

- Don't use `var` when the type isn't obvious from the right-hand side
- Don't add `using` directives that are already in `GlobalUsings.cs`
- Don't modify algorithm logic without understanding the paper (see `UNDERSTANDING_AND_PLAN.md`)
- Don't break existing tests — all 97 must pass before submitting

### Example: Good Code Style

```csharp
/// <summary>
/// Represents a directed edge in a weighted graph.
/// </summary>
/// <param name="To">The destination vertex index.</param>
/// <param name="Weight">The non-negative edge weight.</param>
public readonly record struct Edge(int To, double Weight);
```

---

## 🧪 Testing Guidelines

### Test Categories

| Category | Location | Purpose |
|:---------|:---------|:--------|
| Unit tests | `tests/.../Algorithms/` | Test individual components in isolation |
| Integration tests | `tests/.../Correctness/` | Compare BMSSP vs Dijkstra end-to-end |
| Data structure tests | `tests/.../DataStructures/` | Verify heap, graph, generator behavior |

### Rules

1. **Every new feature must have tests** — no exceptions
2. **Correctness tests are king** — if BMSSP doesn't match Dijkstra, it's a bug
3. **Use `[Theory]` with `[InlineData]`** for parameterized tests
4. **Test edge cases** — empty graphs, single vertex, disconnected, zero-weight edges
5. **Use deterministic seeds** for random graph tests (`seed: 42`)
6. **Tolerance for floating-point comparison**: `1e-9`

### Adding a New Test

```csharp
[Fact]
public void MyNewFeature_DoesExpectedThing()
{
    // Arrange
    var graph = GraphGenerator.LinearChain(10);
    var algorithm = new BmsspAlgorithm();

    // Act
    var result = algorithm.Solve(graph, source: 0);

    // Assert
    Assert.Equal(9.0, result.Distances[9], precision: 9);
}
```

### Running Tests

```bash
# All tests (should complete in < 30 seconds)
dotnet test --verbosity minimal

# Specific category
dotnet test --filter "Category=Correctness"

# With detailed output
dotnet test --verbosity detailed
```

---

## 📝 Commit Messages

Follow the [Conventional Commits](https://www.conventionalcommits.org/) format:

```
<type>(<scope>): <description>

[optional body]
```

### Types

| Type | When to Use |
|:-----|:------------|
| `feat` | New feature or capability |
| `fix` | Bug fix |
| `perf` | Performance improvement |
| `test` | Adding or updating tests |
| `docs` | Documentation changes |
| `refactor` | Code restructuring (no behavior change) |
| `chore` | Build, config, or tooling changes |

### Examples

```
feat(algorithms): add Fibonacci heap variant for Dijkstra
fix(bmssp): correct edge relaxation in FindPivots step 3
perf(partition-ds): use Span<T> for Pull operation
test(correctness): add road network graph tests
docs(readme): add performance comparison chart
```

---

## 🔀 Pull Request Process

1. **Ensure all tests pass**: `dotnet test` must show 0 failures
2. **Ensure build succeeds**: `dotnet build` with 0 warnings, 0 errors
3. **Update documentation** if you changed public APIs or behavior
4. **Write a clear PR description** explaining:
   - What changed and why
   - How you tested it
   - Any performance impact
5. **Keep PRs focused** — one feature/fix per PR

### PR Template

```markdown
## What Changed
Brief description of the change.

## Why
Motivation and context.

## How Tested
- [ ] All 97 existing tests pass
- [ ] Added N new tests for this change
- [ ] Ran benchmarks (if performance-related)

## Checklist
- [ ] Code follows project style guidelines
- [ ] XML doc comments added for public APIs
- [ ] CHANGELOG.md updated (if user-facing change)
```

---

## 💡 Areas for Contribution

Here are some ideas for meaningful contributions:

### 🟢 Good First Issues

- Add more graph generators (e.g., Barabási–Albert power-law graphs)
- Improve console output formatting
- Add a `--verbose` flag to the benchmark runner
- Write additional edge-case tests

### 🟡 Medium Difficulty

- Implement a Fibonacci heap and compare with binary heap Dijkstra
- Add memory usage tracking to `SsspMetrics`
- Create a CSV export option for benchmark results
- Add graph visualization output (DOT/Graphviz format)

### 🔴 Advanced

- Optimize `FindPivots` to use iterative DFS instead of recursive (stack overflow risk on large graphs)
- Implement the full block-based partition data structure from the paper (currently simplified)
- Add parallel BMSSP — the recursive structure is naturally parallelizable
- Benchmark against real-world graph datasets (DIMACS, SNAP)
- Port to use `Span<T>` and `stackalloc` for reduced GC pressure

---

## 🐛 Reporting Bugs

If you find a bug, please open an issue with:

1. **Description** — What went wrong?
2. **Steps to reproduce** — How can we trigger it?
3. **Expected behavior** — What should happen?
4. **Actual behavior** — What actually happened?
5. **Environment** — .NET version, OS, graph size

### Correctness Bug Template

If BMSSP produces different distances than Dijkstra:

```markdown
## Correctness Bug

**Graph:** RandomSparse(n=1000, extra=2000, seed=XYZ)
**Source vertex:** 0
**Mismatched vertex:** 42
**Dijkstra distance:** 15.7293
**BMSSP distance:** 15.7294
**Difference:** 0.0001

**Steps to reproduce:**
var graph = GraphGenerator.RandomSparse(1000, 2000, seed: XYZ);
var dijkstra = new DijkstraAlgorithm().Solve(graph, 0);
var bmssp = new BmsspAlgorithm().Solve(graph, 0);
// Compare dijkstra.Distances[42] vs bmssp.Distances[42]
```

---

<p align="center">
  <strong>Thank you for contributing! 🎉</strong>
</p>
