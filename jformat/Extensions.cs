namespace JFormat;

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
}
