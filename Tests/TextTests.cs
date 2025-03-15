using static jformat.JsonFormatter;
namespace Tests;

public class TextTests
{
    [Fact]
    public void ValidNumberBasics()
    {
        Assert.True(IsValidNumber("00"));
        Assert.True(IsValidNumber("1"));
        Assert.True(IsValidNumber("2.0"));
        Assert.True(IsValidNumber("3.10"));
        Assert.True(IsValidNumber("04.1234"));

        Assert.False(IsValidNumber(" "));
        Assert.False(IsValidNumber(""));      // empty str
        Assert.False(IsValidNumber("1.0.0")); // multiple decimals
        Assert.False(IsValidNumber("1a")); // letters
        Assert.False(IsValidNumber("12a1.12e3")); // letters
        Assert.False(IsValidNumber("1ea3")); // letters
    }

    [Fact]
    public void ValidNumberExponents()
    {
        Assert.True(IsValidNumber("4e5"));
        Assert.True(IsValidNumber("5E4"));
        Assert.True(IsValidNumber("213.123e12"));
        Assert.False(IsValidNumber("1.0E1.0")); // decimal in exponents

        Assert.False(IsValidNumber("4.12e3e3")); // multiple exponents
        Assert.False(IsValidNumber("1e"));  // empty exponent
        Assert.False(IsValidNumber("e1"));  // empty mantissa 
    }

    [Fact]
    public void ValidNumberComplicated()
    {
        Assert.True(IsValidNumber("43211.1234132e4132432"));
        Assert.False(IsValidNumber("09432100 .2134e132"));
        Assert.False(IsValidNumber("43100e1324.1234"));
        Assert.False(IsValidNumber("312. 43214132e 3"));
        Assert.False(IsValidNumber(""));
        Assert.False(IsValidNumber("1+1e5"));
        Assert.False(IsValidNumber("-1e-1.0"));
        Assert.False(IsValidNumber("123..2321"));
    }

    [Fact]
    public void ValidNumberSigns()
    {
        Assert.True(IsValidNumber("-1"));
        Assert.True(IsValidNumber("-1e-1"));
        Assert.True(IsValidNumber("1.0e+1"));
        Assert.True(IsValidNumber("5.132e+23"));

        Assert.False(IsValidNumber("-7.1234-e23"));
    }

    [Fact]
    public void ValidNumberLeadingPlus()
    {
        Assert.False(IsValidNumber("+5.132e+23")); // no leading + allowed in mantissa
        Assert.False(IsValidNumber("+1"));
        Assert.True(IsValidNumber("-1.0e+0"));
    }

    [Fact]
    public void ValidBrackets1()
    {
        string b = "()";
        string b2 = "<>";
        string b3 = "[";
        Assert.True(IsValidBrackets(b));
        Assert.True(IsValidBrackets(b2));
        Assert.False(IsValidBrackets(b3));
    }

    [Fact]
    public void ValidBrackets2()
    {
        string b = "({ ]})";
        Assert.False(IsValidBrackets(b));
        string b2 = "([{ [}])]";
        Assert.False(IsValidBrackets(b2));
    }

    [Fact]
    public void ValidBrackets3()
    {
        string b = "({ [{ [((([[{<>}]])))]}]})";
        Assert.True(IsValidBrackets(b));
        string b2 = "({ [{ [((([[{>}]])))]}]})";
        Assert.False(IsValidBrackets(b2));
        string b3 = "({ [{ [((([[{<}]])))]}]})";
        Assert.False(IsValidBrackets(b3));
    }

    [Fact]
    public void ValidBrackets4()
    {
        string b = "()[][][][]{ }{()()<>}";
        Assert.True(IsValidBrackets(b));
        string b2 = "[[{ (})]]";
        Assert.False(IsValidBrackets(b2));
        string b3 = "([{ }][]())<(><)>";
        Assert.False(IsValidBrackets(b3));
    }

    [Fact]
    public void ValidStringsWithQuotes()
    {
        // I don't want this to work! all quotes should be escaped
        //string str1 = "\"[\"**/bin\",\"**/bower_components\",\"**/jspm_packages\",\"**/node_modules\",\" \\\"   **/obj\",\"**/platforms\"]\"";
        //Assert.True(IsValidString(str1));
        //string str2 = @""" "" "" a "" a "" ,,,,, ; ; ; ""    """"""   """;
        //Assert.True(IsValidString(str2));
        //Assert.True(IsValidString(@""""""));

        Assert.False(IsValidString(""));
        Assert.False(IsValidString("\"\"\"")); // single unescaped quote in the string (no good)
        Assert.False(IsValidString("\"\"\"\"")); // double unescaped quote in the string (no good)
        Assert.True(IsValidString("\" \\\" \"")); // escaped quote in the string (good)
        Assert.False(IsValidString("\"")); // quote is not ended
        Assert.False(IsValidString(@"""")); // quote is not ended
        Assert.True(IsValidString(@"""""")); // unescaped quotes
        Assert.True(IsValidString(@""" \"" """)); // valid escaped quote
    }

    [Fact]
    public void ValidStringsWithEscapes()
    {
        // only valid escapes ", b, n, \, etc
        string str1 = "\"  \\b \\n \\\\ \\u1234 \\u4321  \"";
        Assert.True(IsValidString(str1));
        string str2 = "\" \b \"";
        Assert.False(IsValidString(str2));
        string str3 = "\" \n \"";
        Assert.False(IsValidString(str3));
        string str4 = "\" \\ \"";
        Assert.False(IsValidString(str4)); // unescaped backlash
        string str5 = "\" \\\n \"";
        Assert.False(IsValidString(str5)); // no newlines!!!
        string str6 = "\" \\\\ \"";
        Assert.True(IsValidString(str6)); // escaped backslashed
        Assert.False(IsValidString("\"\\\"")); //single backslash unescaped
    }

    [Fact]
    public void ValidHexStrings()
    {
        Assert.True(IsValidHexDigits("abcdef43214123412434723169acbdcebcedbdeACDBCDBABFEEFEF21312"));
        Assert.False(IsValidHexDigits("ABCDEFG132412342342344"));
        Assert.False(IsValidHexDigits("\n123\n"));
    }

    [Fact]
    public void ValidStringsWithEscape()
    {
        Assert.True(IsValidEscape('b'));
        Assert.True(IsValidEscape('n'));
        Assert.True(IsValidEscape('f'));
        Assert.True(IsValidEscape('b'));
        Assert.True(IsValidEscape('\\'));
        Assert.True(IsValidEscape('/'));
        Assert.True(IsValidEscape("ua3fe"));
        Assert.True(IsValidEscape("u0000"));
        Assert.True(IsValidEscape("u9999"));
        Assert.True(IsValidEscape("uffff"));
        Assert.True(IsValidEscape('"'));

        Assert.False(IsValidEscape('a')); // not a escape sequence
        Assert.False(IsValidEscape('m'));
        Assert.False(IsValidEscape('1'));
        Assert.False(IsValidEscape("u123")); // too short
        Assert.False(IsValidEscape("u1")); // too short
    }

    [Fact]
    public void RemoveWhitespaceTests()
    {
        Assert.Equal("abc", RemoveWhitespace(" a b c "));
        string s = @"a

b                   c";
        Assert.Equal("abc", RemoveWhitespace(s));
        string s2 = "{\"my key\": null}";
        Assert.Equal("{\"my key\":null}", RemoveWhitespace(s2));
        string s3 = @"{ ""my text \""in quotes\""\"""": false }";
        Assert.Equal(@"{""my text \""in quotes\""\"""":false}", RemoveWhitespace(s3));
        string s4 = @"[1, ""a b"", 3]";
        string s4_after = @"[1,""a b"",3]";
        Assert.Equal(s4_after, RemoveWhitespace(s4));
        string s5 = @"[ 123, [ ""heyyy"", ""test words"" ]]";
        string s5_after = @"[123,[""heyyy"",""test words""]]";
        Assert.Equal(s5_after, RemoveWhitespace(s5));
    }
}