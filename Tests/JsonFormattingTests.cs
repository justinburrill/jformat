using static jformat.JsonFormatter;

namespace Tests;
public class JsonFormattingTests
{

    [Fact]
    public void FormatWithBSInStrings()
    {
        var with_commas = @"{""my key,,,,"": null}";
        Assert.True(IsValidJson(with_commas));
        Assert.True(IsValidJson(FormatJsonString(with_commas)));
        var with_brackets = @"{""((((([[[[{{{{{{[[[{{((("": null}";
        Assert.True(IsValidJson(with_brackets));
        Assert.True(IsValidJson(FormatJsonString(with_brackets)));
    }

    [Fact]
    public void FormatExternalFiles1()
    {
        string json = File.ReadAllText("./examples/test4.json");
        Assert.True(IsValidJson(json));
        Assert.True(IsValidJson(FormatJsonString(json)));
    }
    [Fact]
    public void FormatExternalFiles2()
    {
        string json = File.ReadAllText("./examples/navigation.json");
        Assert.True(IsValidJson(json));
        Assert.True(IsValidJson(FormatJsonString(json)));
    }
}
