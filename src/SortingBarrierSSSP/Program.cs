using SortingBarrierSSSP.Benchmarking;

namespace SortingBarrierSSSP;

/// <summary>
/// Console application that runs the BMSSP vs Dijkstra benchmark suite
/// and reports results both to the console and a Markdown file.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Breaking the Sorting Barrier for SSSP — C# Implementation  ║");
        Console.WriteLine("║  Duan, Mao, Mao, Shu, Yin (STOC 2025)                      ║");
        Console.WriteLine("║  Dijkstra vs BMSSP Comparison                               ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        var suite = BenchmarkRunner.RunFullSuite(msg => Console.WriteLine($"  {msg}"));

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("                        RESULTS TABLE                         ");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine();

        // Print header
        Console.WriteLine($"{"Graph Type",-25} {"Algo",-10} {"Time(ms)",10} {"Relaxations",12} {"HeapOps",10} {"Match",8}");
        Console.WriteLine(new string('─', 80));

        // Group by graph type for side-by-side comparison
        var grouped = suite.Results
            .GroupBy(r => r.GraphType)
            .OrderBy(g => g.Key);

        int totalTests = 0;
        int matchCount = 0;

        foreach (var group in grouped)
        {
            foreach (var result in group)
            {
                string matchStr = result.Algorithm == "BMSSP"
                    ? (result.DistancesMatch ? "✓" : $"✗ {result.MaxDistanceError:E1}")
                    : "";

                Console.WriteLine(
                    $"{result.GraphType,-25} {result.Algorithm,-10} {result.ElapsedTime.TotalMilliseconds,10:F3} {result.EdgeRelaxations,12} {result.HeapOperations,10} {matchStr,8}");

                if (result.Algorithm == "BMSSP")
                {
                    totalTests++;
                    if (result.DistancesMatch) matchCount++;
                }
            }
            Console.WriteLine();
        }

        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine($"  Correctness: {matchCount}/{totalTests} test cases match Dijkstra");
        Console.WriteLine($"  Match rate:  {(totalTests > 0 ? 100.0 * matchCount / totalTests : 0):F1}%");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");

        if (matchCount < totalTests)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ⚠ WARNING: Some test cases produced different distances!");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ✓ All test cases produce identical distances to Dijkstra.");
            Console.ResetColor();
        }

        // --- Save results to Markdown ---
        string reportPath = DetermineReportPath(args);
        try
        {
            MarkdownReportWriter.Write(suite, reportPath);
            Console.WriteLine();
            Console.WriteLine($"  📄 Results saved to: {reportPath}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠ Failed to save report: {ex.Message}");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Determines the output path for the Markdown report.
    /// Uses the first command-line argument if provided, otherwise defaults to
    /// a 'results' folder relative to the project.
    /// </summary>
    private static string DetermineReportPath(string[] args)
    {
        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            return args[0];

        // Default: write to ../../results/benchmark-results.md (relative to the exe)
        string baseDir = AppContext.BaseDirectory;

        // Navigate up to the solution root (from bin/Debug/net10.0/)
        string? solutionRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", ".."));

        // Fallback: just use current directory
        if (!Directory.Exists(solutionRoot))
            solutionRoot = Directory.GetCurrentDirectory();

        string resultsDir = Path.Combine(solutionRoot, "results");
        return Path.Combine(resultsDir, "benchmark-results.md");
    }
}
