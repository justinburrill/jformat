using jformat;

namespace Tests;

public class MathTests
{
    [Fact]
    public void CharUpperCase()
    {
        string before = "abcdefghijklmnopqrstuvwxyz0123456789\nABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string expect = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789\nABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string after = "";
        foreach (char ch in before)
        {
            var result = ch.ToUpper();
            after += result;
        }
        Assert.Equal(expect, after);
    }
    [Fact]
    public void CharLowerCase()
    {
        string before = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890\n";
        string expect = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz1234567890\n";
        string after = "";
        foreach (char ch in before)
        {
            var result = ch.ToLower();
            after += result;
        }
        Assert.Equal(expect, after);
    }

    [Fact]
    public void PrefixAndSuffix()
    {
        Assert.Equal("test", "test.json".RemovedSuffix(".json"));
        Assert.Equal("asdftest", "asdfasdftest".RemovedPrefix("asdf"));
        Assert.Equal("", "123".RemovedPrefix("123"));
        Assert.Equal("", "123".RemovedSuffix("123"));
    }

    [Fact]
    public void NonDistinctSets()
    {
        List<int> first = [1, 2, 3, 4, 5, 1, 2, 3];
        List<int> exp = [1, 2, 3];
        Assert.True(first.HasDuplicates(out IEnumerable<int> dupes));
        Assert.Equal(exp, dupes);

        List<int> second = [1, 2, 1, 2, 1, 2, 3, 4, 5];
        List<int> exp2 = [1, 2];
        Assert.True(second.HasDuplicates(out IEnumerable<int> dupes2));
        Assert.Equal(exp2, dupes2);
    }

    [Fact]
    public void SubtractSets()
    {
        HashSet<int> first = [1, 2, 3];
        HashSet<int> exp = [2, 3];
        Assert.Equal(exp, first.Subtract([1]));
    }
    [Fact]
    public void SubtractLists()
    {
        List<int> first = [1, 2, 3];
        List<int> exp = [2, 3];
        Assert.Equal(exp, first.Subtract([1]));

        List<int> second = [1, 2, 3, 4, 5, 1, 2, 3];
        List<int> exp2 = [1, 2, 3];
        Assert.Equal(exp2, second.Subtract([1, 2, 3, 4, 5]));

        List<int> third = [1, 2, 1, 2, 1, 2];
        List<int> exp3 = [1, 2, 1, 2];
        Assert.Equal(exp3, third.Subtract([1, 2]));
    }
}
