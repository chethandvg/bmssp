using SortingBarrierSSSP.Algorithms;

namespace SortingBarrierSSSP.Tests.Algorithms;

public class PartitionDataStructureTests
{
    [Fact]
    public void Empty_IsEmpty()
    {
        var ds = new PartitionDataStructure(10, 100.0);
        Assert.True(ds.IsEmpty);
        Assert.Equal(0, ds.Count);
    }

    [Fact]
    public void Insert_IncreasesCount()
    {
        var ds = new PartitionDataStructure(10, 100.0);
        ds.Insert(0, 5.0);
        ds.Insert(1, 3.0);
        Assert.Equal(2, ds.Count);
    }

    [Fact]
    public void Insert_DuplicateVertex_KeepsSmaller()
    {
        var ds = new PartitionDataStructure(10, 100.0);
        ds.Insert(0, 5.0);
        ds.Insert(0, 3.0); // smaller, should update
        Assert.Equal(1, ds.Count);
        Assert.Equal(3.0, ds.MinValue());

        ds.Insert(0, 7.0); // larger, should be ignored
        Assert.Equal(3.0, ds.MinValue());
    }

    [Fact]
    public void Pull_ReturnsSmallest_M_Entries()
    {
        var ds = new PartitionDataStructure(2, 100.0);
        ds.Insert(0, 5.0);
        ds.Insert(1, 3.0);
        ds.Insert(2, 7.0);
        ds.Insert(3, 1.0);

        var (entries, bound) = ds.Pull();
        Assert.Equal(2, entries.Count);
        Assert.Equal(3, entries[0].Vertex); // value 1.0
        Assert.Equal(1, entries[1].Vertex); // value 3.0

        // Remaining: 5.0 and 7.0
        Assert.Equal(5.0, bound);
        Assert.Equal(2, ds.Count);
    }

    [Fact]
    public void Pull_Empty_ReturnsEmptyWithUpperBound()
    {
        var ds = new PartitionDataStructure(10, 42.0);
        var (entries, bound) = ds.Pull();
        Assert.Empty(entries);
        Assert.Equal(42.0, bound);
    }

    [Fact]
    public void Pull_AllEntries_ReturnsUpperBound()
    {
        var ds = new PartitionDataStructure(10, 100.0);
        ds.Insert(0, 5.0);
        ds.Insert(1, 3.0);

        var (entries, bound) = ds.Pull();
        Assert.Equal(2, entries.Count);
        Assert.Equal(100.0, bound); // all pulled, bound = upper bound
        Assert.True(ds.IsEmpty);
    }

    [Fact]
    public void BatchPrepend_InsertsMultipleEntries()
    {
        var ds = new PartitionDataStructure(10, 100.0);
        ds.BatchPrepend([(0, 5.0), (1, 3.0), (2, 1.0)]);
        Assert.Equal(3, ds.Count);
        Assert.Equal(1.0, ds.MinValue());
    }

    [Fact]
    public void MinValue_Empty_ReturnsInfinity()
    {
        var ds = new PartitionDataStructure(10, 100.0);
        Assert.Equal(double.PositiveInfinity, ds.MinValue());
    }

    [Fact]
    public void MinValue_ReturnsSmallest()
    {
        var ds = new PartitionDataStructure(10, 100.0);
        ds.Insert(0, 5.0);
        ds.Insert(1, 3.0);
        ds.Insert(2, 7.0);
        Assert.Equal(3.0, ds.MinValue());
    }

    [Fact]
    public void MultipleRounds_PullAndInsert()
    {
        var ds = new PartitionDataStructure(2, 100.0);
        ds.Insert(0, 1.0);
        ds.Insert(1, 2.0);
        ds.Insert(2, 3.0);
        ds.Insert(3, 4.0);

        // Round 1: pull 0,1
        var (r1, b1) = ds.Pull();
        Assert.Equal(2, r1.Count);
        Assert.Equal(3.0, b1);

        // Insert new entries
        ds.Insert(4, 2.5);

        // Round 2: pull 2.5, 3.0
        var (r2, b2) = ds.Pull();
        Assert.Equal(2, r2.Count);
        Assert.Contains(r2, e => e.Vertex == 4);
        Assert.Contains(r2, e => e.Vertex == 2);
    }
}
