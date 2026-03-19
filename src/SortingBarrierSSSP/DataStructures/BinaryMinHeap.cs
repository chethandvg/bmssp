namespace SortingBarrierSSSP.DataStructures;

/// <summary>
/// A binary min-heap keyed by a double value, supporting DecreaseKey via index tracking.
/// Used by Dijkstra's algorithm and the BMSSP base case.
/// </summary>
public sealed class BinaryMinHeap
{
    private readonly List<(int Vertex, double Key)> _heap = [];
    private readonly Dictionary<int, int> _indexMap = []; // vertex → position in _heap

    public int Count => _heap.Count;

    /// <summary>Returns true if the vertex is currently in the heap.</summary>
    public bool Contains(int vertex) => _indexMap.ContainsKey(vertex);

    /// <summary>Inserts a vertex with given key.</summary>
    public void Insert(int vertex, double key)
    {
        _heap.Add((vertex, key));
        int idx = _heap.Count - 1;
        _indexMap[vertex] = idx;
        BubbleUp(idx);
    }

    /// <summary>Extracts the vertex with the minimum key.</summary>
    public (int Vertex, double Key) ExtractMin()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Heap is empty.");

        var min = _heap[0];
        _indexMap.Remove(min.Vertex);

        int last = _heap.Count - 1;
        if (last > 0)
        {
            _heap[0] = _heap[last];
            _indexMap[_heap[0].Vertex] = 0;
            _heap.RemoveAt(last);
            BubbleDown(0);
        }
        else
        {
            _heap.RemoveAt(0);
        }

        return min;
    }

    /// <summary>Decreases the key of an existing vertex.</summary>
    public void DecreaseKey(int vertex, double newKey)
    {
        if (!_indexMap.TryGetValue(vertex, out int idx))
            throw new InvalidOperationException($"Vertex {vertex} not in heap.");

        if (newKey > _heap[idx].Key)
            throw new InvalidOperationException("New key is larger than current key.");

        _heap[idx] = (vertex, newKey);
        BubbleUp(idx);
    }

    /// <summary>Peeks at the minimum element without removing it.</summary>
    public (int Vertex, double Key) PeekMin()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Heap is empty.");
        return _heap[0];
    }

    private void BubbleUp(int idx)
    {
        while (idx > 0)
        {
            int parent = (idx - 1) / 2;
            if (_heap[idx].Key >= _heap[parent].Key)
                break;
            Swap(idx, parent);
            idx = parent;
        }
    }

    private void BubbleDown(int idx)
    {
        int count = _heap.Count;
        while (true)
        {
            int smallest = idx;
            int left = 2 * idx + 1;
            int right = 2 * idx + 2;

            if (left < count && _heap[left].Key < _heap[smallest].Key)
                smallest = left;
            if (right < count && _heap[right].Key < _heap[smallest].Key)
                smallest = right;

            if (smallest == idx)
                break;

            Swap(idx, smallest);
            idx = smallest;
        }
    }

    private void Swap(int i, int j)
    {
        (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
        _indexMap[_heap[i].Vertex] = i;
        _indexMap[_heap[j].Vertex] = j;
    }
}
