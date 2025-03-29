using jformat;

using static jformat.JsonFormatter;

namespace Tests;
public class JsonTokens
{

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

        string json3 = @"{""k{ey}[[key]][\"""": ""v{a[l[[u1}}}e""}";
        List<string> tokens3 = ["{", "\"k{ey}[[key]][\\\"\"", ":", "\"v{a[l[[u1}}}e\"", "}"];
        Assert.Equal(tokens3, TokenizeJsonObj(json3));
        // i don't do it like this anymore
        //string json4 = "[1,2,3]";
        //List<string> tokens4 = ["[", "1", ",", "2", ",", "3", "]"];
        //Assert.Equal(tokens4, TokenizeJsonObj(json4));
    }

    [Fact]
    public void TokenizeObjectWithNestedArrayWithNestedQuotes()
    {
        string json = "{\"key1\":[\"\"\"],\"key2\":null}";
        List<string> tokens = ["{", "\"key1\"", ":", "[\"\"\"]", ",", "\"key2\"", ":", "null", "}"];
        Assert.Equal(tokens, TokenizeJsonObj(json));
    }

    [Fact]
    public void TokenizeObjects2()
    {
        string json3 = @"
{
	""key1"": false,
	""key2"": ""test"",
	""key3"": 123
}
";
        List<string> tokens3 = ["{", "\"key1\"", ":", "false", ",", "\"key2\"", ":", "\"test\"", ",", "\"key3\"", ":", "123", "}"];
        Assert.Equal(tokens3, TokenizeJsonObj(json3));
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
    public void TokenizeObjectWithBS()
    {
        string json = @"{""key with , commas"":""value,,, with commas""}";
        List<string> tokens = ["{", "\"key with , commas\"", ":", "\"value,,, with commas\"", "}"];
        Assert.Equal(tokens, TokenizeJsonObj(json));
    }

    [Fact]
    public void TokenizeArrayBasic()
    {
        string empty = "[]";
        string input = "[1, 2, 3]";
        List<string> strs = ["1", "2", "3"];
        Assert.Equal(strs, ExtractArrayElements(input));
        Assert.Equal([], ExtractArrayElements(empty));
    }

    [Fact]
    public void TokenizeArrayNested()
    {
        string input = @"[ ""abc"", 123, [1, 2, 3] ]";
        List<string> strs = ["\"abc\"", "123", "[1,2,3]"];
        Assert.Equal(strs, JsonFormatter.ExtractArrayElements(input));
    }
}
