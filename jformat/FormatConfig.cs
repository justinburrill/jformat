namespace jformat;

public class FormatConfig
{
    public bool OutputToFile = false;
    public bool AllowTrailingCommas = false;
    private bool _validateOnly = false;
    public bool ValidateOnly { get => _validateOnly; set { _validateOnly = value; OutputToFile = false; Overwrite = false; } }
    private bool _overwrite = false;
    public bool Overwrite { get => _overwrite; set { _overwrite = value; OutputToFile = true; } }
    public string OutputPath = ".";

    public FormatConfig(bool toFile = false, bool allowcommas = false)
    {
        OutputToFile = toFile;
        AllowTrailingCommas = allowcommas;

        if (!OutputToFile && Overwrite) { throw new ArgumentException($"Conflicting arguments: OutputToFile:{OutputToFile} and Overwrite:{Overwrite}"); }
    }
}
