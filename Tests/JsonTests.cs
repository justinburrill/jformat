using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JFormat;
namespace Tests
{
    public class JsonTests
    {
        [Fact]
        public void JsonFormatting1()
        {
            string x = @"{}";
            string y = @"{
}";
            string x2 = @"""hey"":{}";
            string y2 = @"""hey"": {
}";
            string x3 = @"{""test"":{""test2"":1}}";
            string y3 = @"{
    ""test"": {
        ""test2"": 1
    }
}";
            Assert.Equal(y, JsonFormatter.FormatString(x));
            Assert.Equal(y2, JsonFormatter.FormatString(x2));
            Assert.Equal(y, JsonFormatter.FormatString(y));
            Assert.Equal(y3, JsonFormatter.FormatString(x3));
        }

        [Fact]
        public void ValidJsonBasics()
        {
            Assert.True(JsonFormatter.IsValidJson(""));
            Assert.True(JsonFormatter.IsValidJson("{}"));
            string b = @"{""key"": ""value""}";
            Assert.True(JsonFormatter.IsValidJson(b));
            string c = @"{""key"": ""value"",}";
            Assert.True(JsonFormatter.IsValidJson(c));


            // value not in quotes
            string A = @"{""key"": value}";
            Assert.False(JsonFormatter.IsValidJson(A));
            // no colon
            string B = @"{""key"" ""value""}";
            Assert.False(JsonFormatter.IsValidJson(B));
            // broken brackets 
            string C = @"""key"" ""value""}";
            Assert.False(JsonFormatter.IsValidJson(C));
        }

        [Fact]
        public void ValidJson1()
        {
            string A = @"{
    ""test"": {
        ""key1"": 1,
        ""key2"": 2,
        ""key3"": 3,
        ""key4"": 4
    }
}";
            Assert.True(JsonFormatter.IsValidJson(A));
        }


    }
}
