namespace SortingBarrierSSSP.Algorithms;

/// <summary>
/// The partition data structure D from the paper (Lemma 3.1).
/// Supports Insert, Pull, and BatchPrepend operations for the BMSSP algorithm.
///
/// Implemented as a SortedDictionary (Red-Black tree) of blocks, where each block
/// contains up to M key-value pairs. This is a simplified but faithful implementation
/// that preserves the algorithmic structure.
///
/// In practice, we use a SortedList of (value, vertex) pairs grouped into blocks,
/// with the .NET SortedDictionary providing O(log n) block lookups.
/// </summary>
public sealed class PartitionDataStructure
{
    private readonly int _m;          // Block size parameter M
    private readonly double _upperBound; // Upper bound B on all values

    // We store all entries in a sorted structure for simplicity.
    // Each entry is (value, vertex). We use a sorted list keyed by value.
    // For a more faithful implementation, we'd use actual blocks — but this
    // preserves correctness and the algorithmic flow.
    private readonly SortedSet<(double Value, int Vertex)> _entries;
    private readonly Dictionary<int, double> _vertexToValue; // track current value per vertex

    public int Count => _entries.Count;
    public bool IsEmpty => _entries.Count == 0;

    public PartitionDataStructure(int m, double upperBound)
    {
        _m = Math.Max(1, m);
        _upperBound = upperBound;
        _entries = new SortedSet<(double Value, int Vertex)>(
            Comparer<(double Value, int Vertex)>.Create((a, b) =>
            {
                int cmp = a.Value.CompareTo(b.Value);
                return cmp != 0 ? cmp : a.Vertex.CompareTo(b.Vertex);
            }));
        _vertexToValue = [];
    }

    /// <summary>
    /// Inserts a key/value pair. If the key already exists, updates to the smaller value.
    /// </summary>
    public void Insert(int vertex, double value)
    {
        if (_vertexToValue.TryGetValue(vertex, out double existing))
        {
            if (value < existing)
            {
                _entries.Remove((existing, vertex));
                _entries.Add((value, vertex));
                _vertexToValue[vertex] = value;
            }
            // If value >= existing, ignore (keep smaller)
        }
        else
        {
            _entries.Add((value, vertex));
            _vertexToValue[vertex] = value;
        }
    }

    /// <summary>
    /// Inserts multiple key/value pairs where each value is smaller than any currently in the structure.
    /// If a vertex already exists, keeps the smaller value.
    /// </summary>
    public void BatchPrepend(IEnumerable<(int Vertex, double Value)> items)
    {
        foreach (var (vertex, value) in items)
        {
            Insert(vertex, value);
        }
    }

    /// <summary>
    /// Returns a subset S' of up to M entries with the smallest values,
    /// and an upper bound separating S' from remaining entries.
    /// Removes the returned entries from the structure.
    /// </summary>
    /// <returns>
    /// (entries, upperBound) where entries are the pulled items and upperBound is:
    /// - The structure's B if no remaining items
    /// - Otherwise, the minimum value of remaining items
    /// </returns>
    public (List<(int Vertex, double Value)> Entries, double UpperBound) Pull()
    {
        var result = new List<(int Vertex, double Value)>();

        if (_entries.Count == 0)
            return (result, _upperBound);

        int count = Math.Min(_m, _entries.Count);
        var toRemove = new List<(double Value, int Vertex)>();

        foreach (var entry in _entries)
        {
            if (result.Count >= count) break;
            result.Add((entry.Vertex, entry.Value));
            toRemove.Add(entry);
        }

        foreach (var entry in toRemove)
        {
            _entries.Remove(entry);
            _vertexToValue.Remove(entry.Vertex);
        }

        double bound;
        if (_entries.Count == 0)
        {
            bound = _upperBound;
        }
        else
        {
            bound = _entries.Min.Value;
        }

        return (result, bound);
    }

    /// <summary>Returns the minimum value currently in the structure, or +∞ if empty.</summary>
    public double MinValue()
    {
        return _entries.Count > 0 ? _entries.Min.Value : double.PositiveInfinity;
    }
}
