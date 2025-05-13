using System.Text;

using jformat;

// TODO: output colouring?

FormatConfig config = new();
var supportedFileTypes = (string[])["json"];
string defaultFileName = "JSON";
bool pipedin = Console.IsInputRedirected;
bool pipedout = Console.IsOutputRedirected;

void fail() { Environment.Exit(1); }

// TODO: give options for just removing whitespace
void PrintUsage()
{
    if (pipedout) { fail(); } // fail here if the output is supposed to go somewhere
    Console.WriteLine("Usage:");
    Console.WriteLine("\tjformat [options] [/path/to/input/files]");
    Console.WriteLine("Options:");
    Console.WriteLine(" -o\t[O]utput the formatted json to a seperate file. (By default, the formatted json is sent to stdout)");
    Console.WriteLine(" -i\tFormat [i]n-place and overwrite the input file.");
    Console.WriteLine(" -v\t[V]alidate only, don't format the json.");
    Console.WriteLine(" -h\tPrint this message.");
    //Console.WriteLine(" -c\t");
}

// usually don't want extra messages if the input is being redirected away
void log(string s)
{
    if (!pipedout)
    {
        Console.WriteLine($"{s}");
    }
}

// TODO
// by default, the formatted output should be sent to stdout
// providing the -o parameter should allow the user to specify a file name to output to.
void HandleArgumentOption(string arg)
{
    if (arg.StartsWith("--")) { log("No support for long argument names yet."); PrintUsage(); return; }
    foreach (char ch in arg.RemovedPrefix("-"))
    {
        switch (ch)
        {
            case '?' or 'h':
                PrintUsage();
                break;
            case 'v':
                config.ValidateOnly = true;
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
                log($"Unrecognized flag {arg}.");
                fail();
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
    catch (ArgumentException err)
    {
        log(err.Message);
        return false;
    }
}

var filesToFormat = new List<string>();
var cwd = Directory.GetCurrentDirectory();

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
                log($"Can't format file '{arg}' because that filetype is not supported.");
                fail();
            }
            break;
    }
}

if (pipedin)
{
    StringBuilder input = new();
    using StreamReader s = new(Console.OpenStandardInput(), Console.InputEncoding);
    string? line;
    while ((line = s.ReadLine()) != null)
    {
        input.AppendFormat(" {0}", line!);
    }
    config.OutputPath = defaultFileName; // piped input has no filename
    string formatted = JsonFormatter.FormatJsonString(input.ToString());
    JsonFormatter.PerformAction(formatted, config);
}
else
{
    if (args.Length == 0)
    {
        log("No files provided.");
        PrintUsage();
        return;
    }

    log($"Processing {filesToFormat.Count} files...");

    foreach (string fp in filesToFormat)
    {
        try
        {
            config.OutputPath = fp;
            string formatted = JsonFormatter.FormatByFilepath(fp);
            JsonFormatter.PerformAction(formatted, config);
        }
        catch (FileNotFoundException e) { log(e.Message); fail(); }
        catch (Exception e)
        {
            log("Uncaught error:");
            log(e.Message);
            log(e.StackTrace!);
            fail();
        }
    }
}

