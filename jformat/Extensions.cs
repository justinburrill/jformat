namespace jformat.extensions;

public static class Extensions
{
    public static char ToUpper(this char @this)
    {
        int ord = @this;
        return ord is >= 97 and <= 122 ? (char)(ord - 32) : @this;
    }
    public static char ToLower(this char @this)
    {
        int ord = @this;
        return ord is >= 65 and <= 90 ? (char)(ord + 32) : @this;
    }

    public static string RemovedPrefix(this string @this, string prefix)
    {
        return @this.StartsWith(prefix) ? @this[prefix.Length..] : @this;
    }
    public static string RemovedSuffix(this string @this, string suffix)
    {
        return @this.EndsWith(suffix) ? @this[..^suffix.Length] : @this;
    }
}
