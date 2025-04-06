namespace jformat;

public class FormatConfig
{
    public bool OutputToFile;
    public bool AllowTrailingCommas;
    private bool _overwrite;
    public bool Overwrite { get => _overwrite; set { _overwrite = value; OutputToFile = true; } }
    public string OutputPath = ".";

    public FormatConfig(bool toFile = false, bool allowcommas = false)
    {
        OutputToFile = toFile;
        AllowTrailingCommas = allowcommas;

        if (!OutputToFile && Overwrite) { throw new ArgumentException($"Conflicting arguments: OutputToFile:{OutputToFile} and Overwrite:{Overwrite}"); }
    }
}
