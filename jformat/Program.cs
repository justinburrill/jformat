using jformat;
using jformat.extensions;

FormatConfig config = new();

void HandleArgument(string arg)
{
    switch (arg.RemovedPrefix("-"))
    {
        case "n":
            config.InPlace = false;
            break;
        case "c":
            config.AllowTrailingCommas = true;
            break;
        default:
            Console.WriteLine($"Unrecognized flag {arg}");
            break;
    }
}

string GetFileType(string filename)
{
    var splits = filename.Split('.');
    return splits.Length == 1 ? throw new ArgumentException($"invalid filetype {filename}") : splits[^1];
}

bool IsSupportedFileType(string filename)
{
    try
    {
        return ((string[])["json"]).Contains(GetFileType(filename));
    }
    catch
    {
        return false;
    }
}

var filesToFormat = new List<string>();
var cwd = Directory.GetCurrentDirectory();
if (args.Length == 0)
{
    // TODO: make it print help here
    Console.WriteLine("No files provided.");
    return;
}

foreach (string arg in args)
{
    if (arg.StartsWith('-'))
    {
        HandleArgument(arg);
        continue;
    }
    switch (arg)
    {
        case "*":
            filesToFormat = (List<string>)filesToFormat.Concat(Directory.GetFiles(cwd));
            break;
        default:
            if (IsSupportedFileType(arg))
            {
                filesToFormat.Append(Path.GetFullPath(arg));
            }
            else
            {
                Console.WriteLine($"Can't format file '{arg}' because that filetype is not supported.");
            }
            break;
    }
}

foreach (string fp in filesToFormat)
{
    try
    {
        JsonFormatter.FormatByFilepath(fp, config.InPlace);
    }
    catch (FileNotFoundException e) { Console.WriteLine(e.Message); }
    catch (Exception e)
    {
        Console.WriteLine(e.StackTrace);
    }
}
