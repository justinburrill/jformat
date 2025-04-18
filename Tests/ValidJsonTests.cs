using jformat;

using static jformat.JsonFormatter;

namespace Tests;

public class ValidJsonTests
{
    [Fact]
    public void DuplicateKey()
    {
        var json = "{\"keyA\":null,\"keyA\":null}";
        Assert.False(IsValidJson(json)); // can't have two of the same key
    }

    [Fact]
    public void NestedDuplicateKeys()
    {
        var json = "{\"keyA\":{\"keyA\":null}, \"KeyB\":{\"keyA\":null}}";
        Assert.True(IsValidJson(json));
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
        Assert.True(IsValidArray(@"[]"));
        Assert.True(IsValidArray(@"[1, 2, 3, 4, 5, 6]"));
        Assert.True(IsValidArray(@"[[1,2], [1,2]]"));
        Assert.True(IsValidArray(@"[{""key"": ""value""}, [1, 2, 3]]"));
    }

    [Fact]
    public void ValidJsonStrings()
    {
        Assert.True(IsValidJson("{}"));
        Assert.True(IsValidJson("{}"));
        Assert.True(IsValidJson("{  }"));
        Assert.True(IsValidJson(" { }"));
        Assert.True(IsValidJson("{ \"a\" : \"b\" }"));
        Assert.True(IsValidJson("{ \"a\" : null }"));
        Assert.True(IsValidJson("{ \"a\" : true }   "));
        Assert.True(IsValidJson("{ \"a\" : false }   "));
        Assert.True(IsValidJson("{ \"a\" : { } }"));
        Assert.True(IsValidJson("[]"));
        Assert.True(IsValidJson("[ ]"));
        Assert.True(IsValidJson(" [ ]"));
        Assert.True(IsValidJson("[1]"));
        Assert.True(IsValidJson("[true]"));
        Assert.True(IsValidJson("[-42]"));
        Assert.True(IsValidJson("[-42, true, false, null]"));
        Assert.True(IsValidJson("{ \"integer\": 1234567890 }"));
        Assert.True(IsValidJson("{ \"real\": -9876.543210 }"));
        Assert.True(IsValidJson("{ \"e\": 0.123456789e-12 }"));
        Assert.True(IsValidJson("{ \"E\": 1.234567890E+34 }"));
        Assert.True(IsValidJson("{ \"\":  23456789012E66 }"));
        Assert.True(IsValidJson("{ \"zero\": 0 }"));
        Assert.True(IsValidJson("{ \"one\": 1 }"));
        Assert.True(IsValidJson(@"{ ""space"": "" "" }"));
        Assert.True(IsValidJson("{ \"quote\": \" \\\" \"}"));
        Assert.True(IsValidJson("{ \"backslash\": \"\\\\\"}"));
        Assert.True(IsValidJson(@"{ ""slash"": ""/ & \/""}"));
        Assert.True(IsValidJson("{ \"alpha\": \"abcdefghijklmnopqrstuvwyz\"}"));
        Assert.True(IsValidJson("{ \"ALPHA\": \"ABCDEFGHIJKLMNOPQRSTUVWYZ\"}"));
        Assert.True(IsValidJson("{ \"digit\": \"0123456789\"}"));
        Assert.True(IsValidJson("{ \"0123456789\": \"digit\"}"));
        Assert.True(IsValidJson("{\"special\": \"`1~!@ $%^&*()_+-={':[,]}|;.</>?\"}"));
        Assert.True(IsValidJson("{\"hex\": \"\u0123\u4567\u89AB\uCDEF\uabcd\uef4A\"}"));
        Assert.True(IsValidJson("{\"true\": true}"));
        Assert.True(IsValidJson("{\"false\": false}"));
        Assert.True(IsValidJson("{\"null\": null}"));
        Assert.True(IsValidJson("{\"array\":[  ]}"));
        Assert.True(IsValidJson("{\"object\":{  }}"));
        Assert.True(IsValidJson("{\"address\": \"50 St. James Street\"}"));
        Assert.True(IsValidJson("{\"url\": \"http://www.JSON.org/\"}"));
        Assert.True(IsValidJson("{\"comment\": \"// /* <!-- --\"}"));
        Assert.True(IsValidJson("{\" #  -- --> */\": \" \"}"));
    }

    [Fact]
    public void InvalidJsonStrings()
    {
        Assert.False(IsValidJson("{"));
        Assert.False(IsValidJson("{ 3 : 4 }"));
        Assert.False(IsValidJson("{ 3 : tru }"));
        Assert.False(IsValidJson("{ \"a : false }"));
        Assert.False(IsValidJson("{\"Extra value after close\": true} \"misplaced quoted value\""));
        Assert.False(IsValidJson("{\"Illegal expression\": 1 + 2}"));
        Assert.False(IsValidJson("{\"Illegal invocation\": alert()}"));
        Assert.False(IsValidJson("{\"Numbers cannot have leading zeroes\": 013}"));
        Assert.False(IsValidJson("{\"Numbers cannot be hex\": 0x14}"));
        Assert.False(IsValidJson("[\"Illegal backslash escape: \x15\"]"));
        Assert.False(IsValidJson("[\naked]"));
        Assert.False(IsValidJson("[\"Illegal backslash escape: \017\"]"));
        Assert.False(IsValidJson("{\"Missing colon\" null}"));
        Assert.False(IsValidJson("[\"Unclosed array\""));
        Assert.False(IsValidJson("{\"Double colon\":: null}"));
        Assert.False(IsValidJson("{\"Comma instead of colon\", null}"));
        Assert.False(IsValidJson("[\"Colon instead of comma\": false]"));
        Assert.False(IsValidJson("[\"Bad value\", truth]"));
        Assert.False(IsValidJson("['single quote']"));
        Assert.False(IsValidJson("[\"\ttab\tcharacter\tin\tstring\t\"]"));
        Assert.False(IsValidJson("[\"line\n...[78]"));
        Assert.False(IsValidJson("[\"line\\n...[79]"));
        Assert.False(IsValidJson("[0e]"));
        Assert.False(IsValidJson("{unquoted_key: \"keys must be quoted\"}"));
        Assert.False(IsValidJson("[0e+]"));
        Assert.False(IsValidJson("[0e+-1]"));
        Assert.False(IsValidJson("{\"Comma instead if closing brace\": true,"));
        Assert.False(IsValidJson("[\"mismatch\"}"));
        Assert.False(IsValidJson("[\"extra comma\",]"));
        Assert.False(IsValidJson("[\"double extra comma\",,]"));
        Assert.False(IsValidJson("[   , \"<-- missing value\"]"));
        Assert.False(IsValidJson("[\"Comma after the close\"],"));
        Assert.False(IsValidJson("[\"Extra close\"]]"));
        Assert.False(IsValidJson("{\"Extra comma\": true,}"));
    }

    [Fact]
    public void JsonStringsWithCtrlChars()
    {
        var valid = @"{
  ""tab"": "" \t "",
  ""newline"": "" \n "",
  ""carriage_return"": "" \r "",
  ""backslash"": "" \\ "",
  ""quote"": "" \"" ""
}";
        Assert.True(IsValidJson(valid));
    }

    // the following ranges of characters are not allowed:
    // u0000 to u001f, as well as u007f, u0080 to u009f, u00ad, u0600 to u0605, u200b, u200b to u200d, u2028 to u2029, u2060, and ufeff
    [Fact]
    public void InvalidJsonStringsWithCtrlChars()
    {
        // inexhaustive list
        Assert.False(IsValidJson("\"control_char_1\": \"\u0000\""));
        Assert.False(IsValidJson("\"control_char_2\": \"\u0001\""));
        Assert.False(IsValidJson("\"control_char_3\": \"\u0002\""));
        Assert.False(IsValidJson("\"control_char_4\": \"\u0003\""));
        Assert.False(IsValidJson("\"control_char_5\": \"\u0004\""));
        Assert.False(IsValidJson("\"control_char_8\": \"\u0007\""));
        Assert.False(IsValidJson("\"control_char_9\": \"\u0008\""));
        Assert.False(IsValidJson("\"control_char_10\": \"\u0009\""));
        Assert.False(IsValidJson("\"control_char_11\": \"\u000A\""));
        Assert.False(IsValidJson("\"control_char_22\": \"\u0015\""));
        Assert.False(IsValidJson("\"control_char_23\": \"\u0016\""));
        Assert.False(IsValidJson("\"control_char_25\": \"\u0018\""));
        Assert.False(IsValidJson("\"control_char_29\": \"\u001C\""));
        Assert.False(IsValidJson("\"control_char_30\": \"\u001D\""));
        Assert.False(IsValidJson("\"control_char_30\": \"\u001D\""));
    }

    [Fact]
    public void InvalidJsonStringsWithUnicodeCtrlChars()
    {
        var iter = Enumerable.Range(0x0, 0x1f)
                             .Concat(Enumerable.Range(0x80, 0x9f))
                             .Concat(Enumerable.Range(0x600, 0x605))
                             .Concat(Enumerable.Range(0x200b, 0x200d));
        foreach (int num in iter.Concat([0x7f, 0xad, 0x200b, 0xfeff]))
        {
            var str = num.ToString("x4");
            Assert.False(IsValidString(str));
        }
    }
}
