using System.Runtime.CompilerServices;

namespace jformat;

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

    public static string RemovedPrefix(this string @this, string prefix) => @this.StartsWith(prefix) ? @this[prefix.Length..] : @this;
    public static string RemovedSuffix(this string @this, string suffix) => @this.EndsWith(suffix) ? @this[..^suffix.Length] : @this;

    public static HashSet<T> Subtract<T>(this HashSet<T> @this, HashSet<T> other)
    {
        var clone = @this.ToHashSet();
        clone.ExceptWith(other);
        return clone;
    }

    public static IEnumerable<T> Subtract<T>(this IEnumerable<T> @this, IEnumerable<T> other)
    {
        var @out = @this.ToList();
        List<T> other_list = [.. other]; // clone into list
        foreach (var item in other)
        {
            if (other.Contains(item))
            {
                @out.Remove(item);
                other_list.Remove(item);
            }
        }
        return @out;
    }

    public static IEnumerable<T> NonDistinct<T>(this IEnumerable<T> @this)
    {
        var unique = @this.Distinct().ToHashSet();

        // TODO
        HashSet<T> dupes = [.. @this.Subtract(unique).Distinct()];

        return dupes;
    }

    public static bool HasDuplicates<T>(this IEnumerable<T> e) => e.NonDistinct().Any();
    public static bool HasDuplicates<T>(this IEnumerable<T> @this, out IEnumerable<T> @out)
    {
        @out = @this.NonDistinct();
        return @out.Any();
    }
}
