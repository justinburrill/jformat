using JFormat;

namespace Tests
{
    public class JsonTests
    {
        [Fact]
        public void ValidArrayBasics()
        {
            Assert.True(JsonFormatter.IsValidArray("[]"));
            Assert.True(JsonFormatter.IsValidArray("[1, 2, 3]"));
            Assert.True(JsonFormatter.IsValidArray(@"[""what's up"", 2, ""another one""]"));
            Assert.True(JsonFormatter.IsValidArray("[true, false, null]"));

            // trailing comma
            Assert.False(JsonFormatter.IsValidArray("[1, 2, 3, ]"));
            Assert.False(JsonFormatter.IsValidArray("["));
        }

        [Fact]
        public void ValidArrayObjs()
        {
            Assert.True(JsonFormatter.IsValidArray(@"[{
    ""wha's up"": {
        ""heyy"": 5
    }
}, {
    ""another thing"": 5,
    ""also"": 1
}]"));
        }

        [Fact]
        public void TokenizeObjectBasics()
        {
            string json = "{}";
            List<string> tokens = ["{", "}"];
            Assert.Equal(tokens, JsonFormatter.TokenizeJsonObj(json));
            string json2 = @"
{
    ""this is my key"": ""my value"",
    ""this is another key"": false
}";
            List<string> tokens2 = ["{", @"""this is my key""", ":", @"""my value""", ",", @"""this is another key""", ":", "false", "}"];
            Assert.Equal(tokens2, JsonFormatter.TokenizeJsonObj(json2));

            string json3 = @"
{
	""key1"": false,
	""key2"": ""test"",
	""key3"": 123
}
";
            List<string> tokens3 = ["{", "\"key1\"", ":", "false", ",", "\"key2\"", ":", "\"test\"", ",", "\"key3\"", ":", "123", "}"];
            Assert.Equal(tokens3, JsonFormatter.TokenizeJsonObj(json3));
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
            Assert.Equal(tokens, JsonFormatter.TokenizeJsonObj(json));
        }

        //        [Fact]
        //        public void JsonFormatting1()
        //        {
        //            string x = @"{}";
        //            string y = "{\r\n\t\r\n}\r\n";
        //            string x2 = @"""hey"":{}";
        //            string y2 = @"""hey"": {
        //}";
        //            string x3 = @"{""test"":{""test2"":1}}";
        //            string y3 = @"{
        //    ""test"": {
        //        ""test2"": 1
        //    }
        //}";
        //            Assert.Equal(y, JsonFormatter.FormatJsonString(x));
        //            Assert.Equal(y2, JsonFormatter.FormatJsonString(x2));
        //            Assert.Equal(y, JsonFormatter.FormatJsonString(y));
        //            Assert.Equal(y3, JsonFormatter.FormatJsonString(x3));
        //        }


        [Fact]
        public void ValidJsonBasics()
        {
            string a = @"""hey"":{}";
            string b = @"{""key"": ""value""}";
            Assert.True(JsonFormatter.IsValidJsonObj("{}"));
            Assert.True(JsonFormatter.IsValidJsonObj(a));
            Assert.True(JsonFormatter.IsValidJsonObj(b));

            // trailing comma not allowed :(
            string c = @"{""key"": ""value"",}";
            Assert.False(JsonFormatter.IsValidJsonObj(c));
            // value not in quotes
            string A = @"{""key"": value}";
            Assert.False(JsonFormatter.IsValidJsonObj(A));
            // no colon
            string B = @"{""key"" ""value""}";
            Assert.False(JsonFormatter.IsValidJsonObj(B));
            // broken brackets 
            string C = @"""key"" ""value""}";
            Assert.False(JsonFormatter.IsValidJsonObj(C));
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
            Assert.True(JsonFormatter.IsValidJsonObj(A));

            string B = @"{
    ""test"": {
        ""key1"": 1,
        ""key2"": 2,
        ""key3"": 3,
        ""key4"": 4
}";
            Assert.False(JsonFormatter.IsValidJsonObj(B)); // missing }
        }



        [Fact]
        public void ValidValues1()
        {
            Assert.True(JsonFormatter.IsValidValue(@"null"));
            Assert.True(JsonFormatter.IsValidValue(@"true"));
            Assert.True(JsonFormatter.IsValidValue(@"false"));
            Assert.True(JsonFormatter.IsValidValue(@"123"));
            Assert.True(JsonFormatter.IsValidValue(@"123.123"));
            Assert.True(JsonFormatter.IsValidValue(@"1e5"));
            Assert.True(JsonFormatter.IsValidValue(@""""""));
            Assert.True(JsonFormatter.IsValidValue(@"""str"""));
            Assert.False(JsonFormatter.IsValidValue(@""));
        }

        [Fact]
        public void TokenizeArrayBasic()
        {
            string input = "[1, 2, 3]";
            List<string> strs = ["1", "2", "3"];
            Assert.Equal(strs, JsonFormatter.TokenizeArray(input));
        }

        [Fact]
        public void TokenizeArrayNested()
        {
            string input = @"[ ""abc"", 123, [1, 2, 3] ]";
            List<string> strs = ["abc", "123", "[1, 2, 3]"];
            Assert.Equal(strs, JsonFormatter.TokenizeArray(input));
        }

        [Fact]
        public void ValidArrays()
        {
            Assert.True(JsonFormatter.IsValidArray(@"{}"));
            Assert.True(JsonFormatter.IsValidArray(@"[1, 2, 3, 4, 5, 6]"));
            Assert.True(JsonFormatter.IsValidArray(@"[[1,2], [1,2]]"));
            Assert.True(JsonFormatter.IsValidArray(@"[{""key"": ""value""}, [1, 2, 3]]"));
        }

    }
}
