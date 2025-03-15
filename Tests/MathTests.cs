using jformat.extensions;

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
}
