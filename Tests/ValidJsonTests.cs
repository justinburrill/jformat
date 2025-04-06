using jformat;

using static jformat.JsonFormatter;

namespace Tests;

public class ValidJsonTests
{
    [Fact]
    public void ValidJsonDuplicateKey()
    {
        var json = "{\"keyA\":null,\"keyA\":null}";
        Assert.False(IsValidJson(json)); // can't have two of the same key
    }

    [Fact]
    public void ValidArrayBasics()
    {
        Assert.True(IsValidArray("[]"));
        Assert.False(IsValidArray("[\"one\" \"two\"]"));
        Assert.True(IsValidArray("[1, 2, 3]"));
        Assert.True(IsValidArray(@"[""what's up"", 2, ""another one""]"));
        Assert.True(IsValidArray("[true, false, null]"));

        // trailing comma
        Assert.False(IsValidArray("[1, 2, 3, ]"));
        Assert.False(IsValidArray("["));
    }

    [Fact]
    public void ValidArraysNested()
    {
        Assert.True(IsValidArray("[[[[[[[[[[]]]]]]]]]]"));
        Assert.True(IsValidArray("[[[[],[]],[true,false,\"other\"],[null,null],[1,2,3,4,5,6,\"seven\"]]]"));
        Assert.False(IsValidArray("[[[[[[[[[]]]]]]]]]]"));
    }

    [Fact]
    public void ValidArrayObjs()
    {
        Assert.True(IsValidArray(@"[{
    ""wha's up"": {
        ""heyy"": 5
    }
},
{
    ""another thing"": 5,
    ""also"": 1
}
]
            "));
    }

    [Fact]
    public void ValidJsonBasics()
    {
        Assert.True(IsValidJson("{}"));
        Assert.False(IsValidJson("{{}}"));
        string b = @"{""key"": ""value""}";
        Assert.True(IsValidJson(b));

        string a = @"[1,2,3]";
        Assert.True(IsValidJson(a));
    }

    [Fact]
    public void ValidJsonFiles()
    {
        string str1 = File.ReadAllText("./examples/test1.json");
        string str2 = File.ReadAllText("./examples/test2.json");
        Assert.True(IsValidJson(str1));
        Assert.True(IsValidJson(str2));
    }

    [Fact]
    public void ValidJsonFilesWithBS()
    {
        string str3 = File.ReadAllText("./examples/test3.json");
        Assert.True(IsValidJson(str3));
        string str4 = File.ReadAllText("./examples/test4.json");
        Assert.True(IsValidJson(str4));
    }

    [Fact]
    public void InvalidJsonObjsBasics()
    {
        string a = @"""hey"":{}";
        Assert.False(IsValidObject(a));
        // trailing comma not allowed :(
        string c = @"{""key"": ""value"",}";
        Assert.False(IsValidObject(c));
        // value not in quotes
        string A = @"{""key"": value}";
        Assert.False(IsValidObject(A));
        // no colon
        string B = @"{""key"" ""value""}";
        Assert.False(IsValidObject(B));
        // broken brackets 
        string C = @"""key"" ""value""}";
        Assert.False(IsValidObject(C));
    }

    [Fact]
    public void InvalidJsonTricky()
    {
        Assert.False(IsValidJson(""));
        Assert.False(IsValidJson("{[}"));
        Assert.False(IsValidJson("{}}"));
        Assert.False(IsValidJson("{{}}"));
        Assert.False(IsValidJson("[[{{}}]]"));
        Assert.False(IsValidJson(@"{
  123: ""This key is not a string"",
  ""validKey"": ""This is valid""
}"));
        Assert.False(IsValidJson(@"{
  ""title"": ""Invalid Example with a quote: ""This is unescaped"" and more text.""
}"));
    }

    [Fact]
    public void ValidJsonNested()
    {
        string A = @"{
    ""test"": {
        ""key1"": 1,
        ""key2"": 2,
        ""key3"": 3,
        ""key4"": 4
    }
}";
        Assert.True(IsValidObject(A));

        string B = @"{
    ""test"": {
        ""key1"": 1,
        ""key2"": 2,
        ""key3"": 3,
        ""key4"": 4
    
}";
        Assert.False(IsValidObject(B)); // missing }
    }

    [Fact]
    public void ValidValues1()
    {
        Assert.True(IsValidValue(@"null"));
        Assert.True(IsValidValue(@"true"));
        Assert.True(IsValidValue(@"false"));
        Assert.True(IsValidValue(@"123"));
        Assert.True(IsValidValue(@"123.123"));
        Assert.True(IsValidValue(@"1e5"));
        Assert.True(IsValidValue(@""""""));
        Assert.True(IsValidValue(@"""str1"""));
        Assert.False(IsValidValue(@""));
    }

    [Fact]
    public void ValidArrays()
    {
        Assert.True(JsonFormatter.IsValidArray(@"[]"));
        Assert.True(JsonFormatter.IsValidArray(@"[1, 2, 3, 4, 5, 6]"));
        Assert.True(JsonFormatter.IsValidArray(@"[[1,2], [1,2]]"));
        Assert.True(JsonFormatter.IsValidArray(@"[{""key"": ""value""}, [1, 2, 3]]"));
    }
}
