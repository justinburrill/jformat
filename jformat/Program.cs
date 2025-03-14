using JFormat;

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

//Console.WriteLine(FormatJsonString("{}"));
var filesToFormat = new List<string>();
//string filepath = "";
var cwd = Directory.GetCurrentDirectory();
if (args.Length == 0)
{
    Console.WriteLine("No files provided.");
    return;
}

foreach (var arg in args)
{
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

foreach (var fp in filesToFormat)
{
    try
    {
        JsonFormatter.FormatByFilepath(fp);
    }
    catch (FileNotFoundException e) { Console.WriteLine(e.Message); }
    catch (Exception e)
    {
        Console.WriteLine(e.StackTrace);
    }
}
