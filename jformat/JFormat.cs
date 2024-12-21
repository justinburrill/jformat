using System.Collections;
using System.Collections.Generic;

namespace JFormat
{
    class JFormat
    {
        static string GetFileType(string filename)
        {
            var splits = filename.Split('.');
            if (splits.Length == 1)
            {
                throw new ArgumentException($"invalid filetype {filename}");
            }
            return splits[^1];
        }

        static bool IsSupportedFileType(string filename)
        {
            try
            {
                return ((string[])(["json"])).Contains(GetFileType(filename));
            }
            catch
            {
                return false;
            }
        }

        static void Main(string[] args)
        {
            //Console.WriteLine(JsonFormatter.FormatJsonString("{}"));
            var filesToFormat = new List<String>();
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
        }
    }
}