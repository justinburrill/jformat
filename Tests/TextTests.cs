using JFormat;
using System.Text;
namespace Tests
{
    public class TextTests
    {
        [Fact]
        public void ValidNumberBasics()
        {
            Assert.True(JsonFormatter.IsValidNumber("00"));
            Assert.True(JsonFormatter.IsValidNumber("1"));
            Assert.True(JsonFormatter.IsValidNumber("2.0"));
            Assert.True(JsonFormatter.IsValidNumber("3.10"));
            Assert.True(JsonFormatter.IsValidNumber("04.1234"));

            Assert.False(JsonFormatter.IsValidNumber(" "));
            Assert.False(JsonFormatter.IsValidNumber(""));      // empty str
            Assert.False(JsonFormatter.IsValidNumber("1.0.0")); // multiple decimals
            Assert.False(JsonFormatter.IsValidNumber("1a")); // letters
            Assert.False(JsonFormatter.IsValidNumber("12a1.12e3")); // letters
            Assert.False(JsonFormatter.IsValidNumber("1ea3")); // letters
        }

        [Fact]
        public void ValidNumberExponents()
        {
            Assert.True(JsonFormatter.IsValidNumber("4e5"));
            Assert.True(JsonFormatter.IsValidNumber("5E4"));
            Assert.True(JsonFormatter.IsValidNumber("213.123e12"));
            Assert.False(JsonFormatter.IsValidNumber("1.0E1.0")); // decimal in exponents

            Assert.False(JsonFormatter.IsValidNumber("4.12e3e3")); // multiple exponents
            Assert.False(JsonFormatter.IsValidNumber("1e"));  // empty exponent
            Assert.False(JsonFormatter.IsValidNumber("e1"));  // empty mantissa 
        }

        [Fact]
        public void ValidNumberComplicated()
        {
            Assert.True(JsonFormatter.IsValidNumber("43211.1234132e4132432"));
            Assert.False(JsonFormatter.IsValidNumber("09432100 .2134e132"));
            Assert.False(JsonFormatter.IsValidNumber("43100e1324.1234"));
            Assert.False(JsonFormatter.IsValidNumber("312. 43214132e 3"));
            Assert.False(JsonFormatter.IsValidNumber(""));
            Assert.False(JsonFormatter.IsValidNumber("1+1e5"));
            Assert.False(JsonFormatter.IsValidNumber("-1e-1.0"));
            Assert.False(JsonFormatter.IsValidNumber("123..2321"));
        }

        [Fact]
        public void ValidNumberSigns()
        {
            Assert.True(JsonFormatter.IsValidNumber("-1"));
            Assert.True(JsonFormatter.IsValidNumber("-1e-1"));
            Assert.True(JsonFormatter.IsValidNumber("1.0e+1"));
            Assert.True(JsonFormatter.IsValidNumber("5.132e+23"));

            Assert.False(JsonFormatter.IsValidNumber("-7.1234-e23"));
        }

        [Fact]
        public void ValidNumberLeadingPlus()
        {
            Assert.False(JsonFormatter.IsValidNumber("+5.132e+23")); // no leading + allowed in mantissa
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
        public void RemoveWhitespace()
        {
            Assert.Equal("abc", JsonFormatter.RemoveWhitespace(" a b c "));
            string s = @"a

b                   c";
            Assert.Equal("abc", JsonFormatter.RemoveWhitespace(s));
            string s2 = "{\"my key\": null}";
            Assert.Equal("{\"my key\":null}", JsonFormatter.RemoveWhitespace(s2));
            string s3 = @"{ ""my text \""in quotes\""\"""": false }";
            Assert.Equal(@"{""my text \""in quotes\""\"""":false}", JsonFormatter.RemoveWhitespace(s3));
            string s4 = @"[1, ""a b"", 3]";
            string s4_after = @"[1,""a b"",3]";
            Assert.Equal(s4_after, JsonFormatter.RemoveWhitespace(s4));
            string s5 = @"[ 123, [ ""heyyy"", ""test words"" ]]";
            string s5_after = @"[123,[""heyyy"",""test words""]]";
            Assert.Equal(s5_after, JsonFormatter.RemoveWhitespace(s5));
        }
    }
}