using System.Text;

namespace SortingBarrierSSSP.Benchmarking;

/// <summary>
/// Writes benchmark results to a Markdown file for analysis.
/// </summary>
public static class MarkdownReportWriter
{
    /// <summary>
    /// Writes the full benchmark suite results to a Markdown file.
    /// </summary>
    public static void Write(BenchmarkSuite suite, string filePath)
    {
        var sb = new StringBuilder();

        sb.AppendLine("# Breaking the Sorting Barrier — Benchmark Results");
        sb.AppendLine();
        sb.AppendLine($"**Generated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"**Suite:** {suite.Name}");
        sb.AppendLine($"**Runtime:** .NET {Environment.Version}");
        sb.AppendLine($"**OS:** {Environment.OSVersion}");
        sb.AppendLine($"**Machine:** {Environment.MachineName} ({Environment.ProcessorCount} logical cores)");
        sb.AppendLine();

        // --- Summary ---
        var bmsspResults = suite.Results.Where(r => r.Algorithm == "BMSSP").ToList();
        int totalTests = bmsspResults.Count;
        int matchCount = bmsspResults.Count(r => r.DistancesMatch);

        sb.AppendLine("## Summary");
        sb.AppendLine();
        sb.AppendLine($"- **Total test configurations:** {totalTests}");
        sb.AppendLine($"- **Correctness (BMSSP matches Dijkstra):** {matchCount}/{totalTests} ({(totalTests > 0 ? 100.0 * matchCount / totalTests : 0):F1}%)");
        sb.AppendLine($"- **Status:** {(matchCount == totalTests ? "✅ ALL PASS" : "❌ SOME MISMATCHES")}");
        sb.AppendLine();

        // --- Detailed Results Table ---
        sb.AppendLine("## Detailed Results");
        sb.AppendLine();
        sb.AppendLine("| Graph Type | V | E | Algorithm | Time (ms) | Edge Relaxations | Heap Ops | Match |");
        sb.AppendLine("|:-----------|----:|----:|:----------|----------:|-----------------:|---------:|:-----:|");

        var grouped = suite.Results
            .GroupBy(r => r.GraphType)
            .OrderBy(g => g.Key);

        foreach (var group in grouped)
        {
            foreach (var r in group)
            {
                string matchStr = r.Algorithm == "BMSSP"
                    ? (r.DistancesMatch ? "✅" : $"❌ err={r.MaxDistanceError:E2}")
                    : "—";

                sb.AppendLine(
                    $"| {r.GraphType} | {r.Vertices} | {r.Edges} | {r.Algorithm} " +
                    $"| {r.ElapsedTime.TotalMilliseconds:F3} | {r.EdgeRelaxations:N0} | {r.HeapOperations:N0} | {matchStr} |");
            }
        }

        sb.AppendLine();

        // --- Performance Comparison ---
        sb.AppendLine("## Performance Comparison (BMSSP vs Dijkstra)");
        sb.AppendLine();
        sb.AppendLine("| Graph Type | V | Dijkstra (ms) | BMSSP (ms) | Speedup | Dijkstra Relaxations | BMSSP Relaxations | Relaxation Ratio |");
        sb.AppendLine("|:-----------|----:|--------------:|-----------:|--------:|---------------------:|------------------:|-----------------:|");

        var pairs = suite.Results
            .GroupBy(r => r.GraphType)
            .Where(g => g.Count() == 2)
            .Select(g => (
                Dijkstra: g.First(r => r.Algorithm == "Dijkstra"),
                Bmssp: g.First(r => r.Algorithm == "BMSSP")))
            .OrderBy(p => p.Dijkstra.Vertices);

        foreach (var (d, b) in pairs)
        {
            double speedup = b.ElapsedTime.TotalMilliseconds > 0
                ? d.ElapsedTime.TotalMilliseconds / b.ElapsedTime.TotalMilliseconds
                : double.PositiveInfinity;
            double relaxRatio = d.EdgeRelaxations > 0
                ? (double)b.EdgeRelaxations / d.EdgeRelaxations
                : 0;

            string speedupStr = speedup >= 1.0
                ? $"{speedup:F2}x faster"
                : $"{1.0 / speedup:F2}x slower";

            sb.AppendLine(
                $"| {d.GraphType} | {d.Vertices} " +
                $"| {d.ElapsedTime.TotalMilliseconds:F3} | {b.ElapsedTime.TotalMilliseconds:F3} " +
                $"| {speedupStr} | {d.EdgeRelaxations:N0} | {b.EdgeRelaxations:N0} | {relaxRatio:F2}x |");
        }

        sb.AppendLine();

        // --- Correctness Verification ---
        sb.AppendLine("## Correctness Verification");
        sb.AppendLine();

        if (matchCount == totalTests)
        {
            sb.AppendLine("✅ **All BMSSP results match Dijkstra's output within floating-point tolerance (1e-9).**");
            sb.AppendLine();
            sb.AppendLine("This confirms that the BMSSP algorithm correctly computes single-source shortest paths");
            sb.AppendLine("for all tested graph types and sizes.");
        }
        else
        {
            sb.AppendLine("❌ **Some BMSSP results do NOT match Dijkstra's output.**");
            sb.AppendLine();
            sb.AppendLine("Mismatched configurations:");
            sb.AppendLine();
            foreach (var r in bmsspResults.Where(r => !r.DistancesMatch))
            {
                sb.AppendLine($"- **{r.GraphType}** (V={r.Vertices}, E={r.Edges}): max error = {r.MaxDistanceError:E3}");
            }
        }

        sb.AppendLine();

        // --- Algorithm Notes ---
        sb.AppendLine("## Algorithm Notes");
        sb.AppendLine();
        sb.AppendLine("- **Dijkstra**: Classic O(m log n) with binary min-heap. The gold-standard baseline.");
        sb.AppendLine("- **BMSSP**: Implementation of the algorithm from *\"Breaking the Sorting Barrier for");
        sb.AppendLine("  Directed Single-Source Shortest Paths\"* (Duan, Mao, Mao, Shu, Yin — STOC 2025).");
        sb.AppendLine("  Theoretical complexity: O(m · log^(2/3)(n)) in the comparison-addition model.");
        sb.AppendLine();
        sb.AppendLine("### Key Observations");
        sb.AppendLine();
        sb.AppendLine("The BMSSP algorithm's theoretical advantage (log^(2/3)(n) vs log(n)) is asymptotic.");
        sb.AppendLine("For practical graph sizes (n < 10^6), the constant factors and overhead of the recursive");
        sb.AppendLine("decomposition, FindPivots, and partition data structure may outweigh the theoretical gain.");
        sb.AppendLine("The primary value of this implementation is **correctness verification** — confirming that");
        sb.AppendLine("the algorithm produces identical shortest-path distances to Dijkstra across diverse graph families.");
        sb.AppendLine();

        // Write to file
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(filePath, sb.ToString());
    }
}
