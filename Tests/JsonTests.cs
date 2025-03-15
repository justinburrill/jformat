using jformat;

using static jformat.JsonFormatter;

namespace Tests;

public class JsonTests
{
    [Fact]
    public void ValidArrayBasics()
    {
        Assert.True(IsValidArray("[]"));
        Assert.True(IsValidArray("[1, 2, 3]"));
        Assert.True(IsValidArray(@"[""what's up"", 2, ""another one""]"));
        Assert.True(IsValidArray("[true, false, null]"));

        // trailing comma
        Assert.False(IsValidArray("[1, 2, 3, ]"));
        Assert.False(IsValidArray("["));
    }

    [Fact]
    public void ValidArrayObjs()
    {
        Assert.True(JsonFormatter.IsValidArray(@"[{
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
    public void TokenizeObjectBasics()
    {
        string json = "{}";
        List<string> tokens = ["{", "}"];
        Assert.Equal(tokens, TokenizeJsonObj(json));
        string json2 = @"
{
    ""this is my key"": ""my value"",
    ""this is another key"": false
}";
        List<string> tokens2 = ["{", @"""this is my key""", ":", @"""my value""", ",", @"""this is another key""", ":", "false", "}"];
        Assert.Equal(tokens2, TokenizeJsonObj(json2));
        string json3 = @"
{
	""key1"": false,
	""key2"": ""test"",
	""key3"": 123
}
";
        List<string> tokens3 = ["{", "\"key1\"", ":", "false", ",", "\"key2\"", ":", "\"test\"", ",", "\"key3\"", ":", "123", "}"];
        Assert.Equal(tokens3, TokenizeJsonObj(json3));
    }

    [Fact]
    public void TokenizeObjects2()
    {
        string json = @"

  {
    ""an array"": [
      1,
      2,
      3
    ],
    ""other"": true
  }
";
        List<string> strs = ["{", "\"an array\"", ":", "[1,2,3]", ",", "\"other\"", ":", "true", "}"];
        Assert.Equal(strs, TokenizeJsonObj(json));
    }

    [Fact]
    public void TokenizeObjectNested()
    {
        string json = @"
{
	""myobj"": {
		""key1"": false,
		""key2"": ""hey""
	},
	""after"": true
}
";
        List<string> tokens = ["{", "\"myobj\"", ":", "{", "\"key1\"", ":", "false", ",", "\"key2\"", ":", "\"hey\"", "}", ",", "\"after\"", ":", "true", "}"];
        Assert.Equal(tokens, TokenizeJsonObj(json));
    }

    [Fact]
    public void ValidJsonBasics()
    {
        string b = @"{""key"": ""value""}";
        Assert.True(IsValidJson("{}"));
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
    }

    [Fact]
    public void ValidJsonFilesExternal()
    {
        string json = File.ReadAllText("./examples/navigation.json");
    }

    [Fact]
    public void InvalidJsonObjsBasics()
    {
        string a = @"""hey"":{}";
        Assert.False(IsValidJsonObj(a));
        // trailing comma not allowed :(
        string c = @"{""key"": ""value"",}";
        Assert.False(IsValidJsonObj(c));
        // value not in quotes
        string A = @"{""key"": value}";
        Assert.False(IsValidJsonObj(A));
        // no colon
        string B = @"{""key"" ""value""}";
        Assert.False(IsValidJsonObj(B));
        // broken brackets 
        string C = @"""key"" ""value""}";
        Assert.False(IsValidJsonObj(C));
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
        Assert.True(IsValidJsonObj(A));

        string B = @"{
    ""test"": {
        ""key1"": 1,
        ""key2"": 2,
        ""key3"": 3,
        ""key4"": 4
}";
        Assert.False(IsValidJsonObj(B)); // missing }
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
    public void TokenizeArrayBasic()
    {
        string empty = "[]";
        string input = "[1, 2, 3]";
        List<string> strs = ["1", "2", "3"];
        Assert.Equal(strs, TokenizeArray(input));
        Assert.Equal([], TokenizeArray(empty));
    }

    [Fact]
    public void TokenizeArrayNested()
    {
        string input = @"[ ""abc"", 123, [1, 2, 3] ]";
        List<string> strs = ["\"abc\"", "123", "[1,2,3]"];
        Assert.Equal(strs, JsonFormatter.TokenizeArray(input));
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
