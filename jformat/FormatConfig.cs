namespace jformat;

public class FormatConfig
{
    public bool InPlace;
    public bool AllowTrailingCommas;
    public FormatConfig()
    {
        // defaults
        InPlace = true;
        AllowTrailingCommas = false;
    }
    public FormatConfig(bool inplace, bool allowcommas)
    {
        InPlace = inplace;
        AllowTrailingCommas = allowcommas;
    }
}
