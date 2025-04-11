using jformat;
using jformat.extensions;

FormatConfig config = new();
var supportedFileTypes = (string[])["json"];

void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("\tjformat [options] [/path/to/input/files]");
    Console.WriteLine("Options:");
    Console.WriteLine(" -o\t[O]utput the formatted json to a seperate file. (By default, the formatted json is sent to stdout)");
    Console.WriteLine(" -i\tFormat [i]n-place and overwrite the input file.");
    Console.WriteLine(" -h\tPrint this message.");
    //Console.WriteLine(" -c\t");
}

void HandleArgumentOption(string arg)
{
    if (arg.StartsWith("--")) { Console.WriteLine("No support for long argument names yet."); PrintUsage(); return; }
    foreach (char ch in arg.RemovedPrefix("-"))
    {
        switch (ch)
        {
            case '?' or 'h':
                PrintUsage();
                break;
            case 'o':
                config.OutputToFile = true;
                break;
            case 'c':
                config.AllowTrailingCommas = true;
                break;
            case 'i':
                config.Overwrite = true;
                break;
            default:
                Console.WriteLine($"Unrecognized flag {arg}.");
                break;
        }
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
        return supportedFileTypes.Contains(GetFileType(filename));
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
    Console.WriteLine("No files provided.");
    PrintUsage();
    return;
}

foreach (string arg in args)
{
    if (arg.StartsWith('-'))
    {
        HandleArgumentOption(arg);
        continue;
    }
    switch (arg)
    {
        // TODO: have actual file globbing
        case "*":
            filesToFormat = (List<string>)filesToFormat.Concat(Directory.GetFiles(cwd));
            break;
        default:
            if (IsSupportedFileType(arg))
            {
                filesToFormat.Add(Path.GetFullPath(arg));
            }
            else
            {
                Console.WriteLine($"Can't format file '{arg}' because that filetype is not supported.");
            }
            break;
    }
}

Console.WriteLine($"Formatting {filesToFormat.Count} files...");

// by default, the formatted output should be sent to stdout
// providing the -o parameter should allow the user to specify a file name to output to.
foreach (string fp in filesToFormat)
{
    try
    {
        JsonFormatter.FormatByFilepath(fp, config);
    }
    catch (FileNotFoundException e) { Console.WriteLine(e.Message); }
    catch (Exception e)
    {
        Console.WriteLine(e.StackTrace);
    }
}
