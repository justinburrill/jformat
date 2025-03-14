using JFormat;

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
}
