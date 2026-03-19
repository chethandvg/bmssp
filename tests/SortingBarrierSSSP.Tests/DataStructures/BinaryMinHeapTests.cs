using SortingBarrierSSSP.DataStructures;

namespace SortingBarrierSSSP.Tests.DataStructures;

public class BinaryMinHeapTests
{
    [Fact]
    public void Empty_CountIsZero()
    {
        var heap = new BinaryMinHeap();
        Assert.Equal(0, heap.Count);
    }

    [Fact]
    public void Insert_IncreasesCount()
    {
        var heap = new BinaryMinHeap();
        heap.Insert(0, 5.0);
        heap.Insert(1, 3.0);
        Assert.Equal(2, heap.Count);
    }

    [Fact]
    public void ExtractMin_ReturnsSmallest()
    {
        var heap = new BinaryMinHeap();
        heap.Insert(0, 5.0);
        heap.Insert(1, 3.0);
        heap.Insert(2, 7.0);

        var min = heap.ExtractMin();
        Assert.Equal(1, min.Vertex);
        Assert.Equal(3.0, min.Key);
    }

    [Fact]
    public void ExtractMin_ReturnsInOrder()
    {
        var heap = new BinaryMinHeap();
        heap.Insert(0, 5.0);
        heap.Insert(1, 3.0);
        heap.Insert(2, 7.0);
        heap.Insert(3, 1.0);
        heap.Insert(4, 9.0);

        Assert.Equal(1.0, heap.ExtractMin().Key);
        Assert.Equal(3.0, heap.ExtractMin().Key);
        Assert.Equal(5.0, heap.ExtractMin().Key);
        Assert.Equal(7.0, heap.ExtractMin().Key);
        Assert.Equal(9.0, heap.ExtractMin().Key);
    }

    [Fact]
    public void DecreaseKey_UpdatesPriority()
    {
        var heap = new BinaryMinHeap();
        heap.Insert(0, 5.0);
        heap.Insert(1, 3.0);
        heap.Insert(2, 7.0);

        heap.DecreaseKey(2, 1.0);
        var min = heap.ExtractMin();
        Assert.Equal(2, min.Vertex);
        Assert.Equal(1.0, min.Key);
    }

    [Fact]
    public void DecreaseKey_LargerValue_Throws()
    {
        var heap = new BinaryMinHeap();
        heap.Insert(0, 5.0);
        Assert.Throws<InvalidOperationException>(() => heap.DecreaseKey(0, 10.0));
    }

    [Fact]
    public void DecreaseKey_NonexistentVertex_Throws()
    {
        var heap = new BinaryMinHeap();
        heap.Insert(0, 5.0);
        Assert.Throws<InvalidOperationException>(() => heap.DecreaseKey(99, 1.0));
    }

    [Fact]
    public void Contains_ReturnsCorrectly()
    {
        var heap = new BinaryMinHeap();
        heap.Insert(0, 5.0);
        Assert.True(heap.Contains(0));
        Assert.False(heap.Contains(1));

        heap.ExtractMin();
        Assert.False(heap.Contains(0));
    }

    [Fact]
    public void PeekMin_DoesNotRemove()
    {
        var heap = new BinaryMinHeap();
        heap.Insert(0, 5.0);
        heap.Insert(1, 3.0);

        var peek = heap.PeekMin();
        Assert.Equal(1, peek.Vertex);
        Assert.Equal(2, heap.Count); // not removed
    }

    [Fact]
    public void ExtractMin_Empty_Throws()
    {
        var heap = new BinaryMinHeap();
        Assert.Throws<InvalidOperationException>(() => heap.ExtractMin());
    }

    [Fact]
    public void PeekMin_Empty_Throws()
    {
        var heap = new BinaryMinHeap();
        Assert.Throws<InvalidOperationException>(() => heap.PeekMin());
    }

    [Fact]
    public void LargeHeap_MaintainsOrder()
    {
        var heap = new BinaryMinHeap();
        var rng = new Random(42);
        var values = new double[1000];
        for (int i = 0; i < 1000; i++)
        {
            values[i] = rng.NextDouble() * 1000;
            heap.Insert(i, values[i]);
        }

        double prev = double.NegativeInfinity;
        while (heap.Count > 0)
        {
            var (_, key) = heap.ExtractMin();
            Assert.True(key >= prev, $"Heap order violated: {key} < {prev}");
            prev = key;
        }
    }
}
