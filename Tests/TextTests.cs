using JFormat;
using System.Text;
namespace Tests
{
    public class TextTests
    {

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
        public void RemoveWhitespace()
        {
            Assert.Equal("abc", JsonFormatter.RemoveWhitespace(" a b c "));
            string s = @"a

b                   c";
            Assert.Equal("abc", JsonFormatter.RemoveWhitespace(s));
        }
    }
}