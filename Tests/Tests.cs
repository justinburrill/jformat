using JFormat;
using System.Text;
namespace JFormatTests
{
    public class Tests
    {
        [Fact]
        public void JsonFormatting1()
        {
            string x = JsonFormatter.FormatString(@"""hey"":{}");
            string y = @"""hey"": {
}";
            Assert.Equal(x, y);
        }

        [Fact]
        public void ValidBrackets1()
        {
            string b = "()";
            string b2 = "<>";
            string b3 = "[";
            Assert.True(JsonFormatter.IsValidBrackets(b));
            Assert.True(JsonFormatter.IsValidBrackets(b2));
            Assert.False(JsonFormatter.IsValidBrackets(b3));
        }

        [Fact]
        public void ValidBrackets2()
        {
            string b = "({ ]})";
            Assert.False(JsonFormatter.IsValidBrackets(b));
            string b2 = "([{ [}])]";
            Assert.False(JsonFormatter.IsValidBrackets(b2));
        }

        [Fact]
        public void ValidBrackets3()
        {
            string b = "({ [{ [((([[{<>}]])))]}]})";
            Assert.True(JsonFormatter.IsValidBrackets(b));
            string b2 = "({ [{ [((([[{>}]])))]}]})";
            Assert.False(JsonFormatter.IsValidBrackets(b2));
            string b3 = "({ [{ [((([[{<}]])))]}]})";
            Assert.False(JsonFormatter.IsValidBrackets(b3));
        }

        [Fact]
        public void ValidBrackets4()
        {
            string b = "()[][][][]{ }{()()<>}";
            Assert.True(JsonFormatter.IsValidBrackets(b));
            string b2 = "[[{ (})]]";
            Assert.False(JsonFormatter.IsValidBrackets(b2));
            string b3 = "([{ }][]())<(><)>";
            Assert.False(JsonFormatter.IsValidBrackets(b3));
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