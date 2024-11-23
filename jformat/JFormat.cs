using System.Collections;
using System.Collections.Generic;

namespace JFormat
{
    class JFormat
    {
        static string getFileType(string filename)
        {
            var splits = filename.Split('.');
            if (splits.Length == 1)
            {
                throw new ArgumentException($"invalid filetype {filename}");
            }
            return splits[splits.Length - 1];
        }
        static bool isSupportedFileType(string filename)
        {
            return ((string[])(["json"])).Contains(getFileType(filename));
        }

        static void Main(string[] args)
        {
            //Console.WriteLine(JsonFormatter.FormatJsonString("{}"));
            var filesToFormat = new List<String>();
            //string filepath = "";
            var cwd = Directory.GetCurrentDirectory();

            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "*":
                        filesToFormat = (List<string>)filesToFormat.Concat(Directory.GetFiles(cwd));
                        break;
                    default:
                        if (isSupportedFileType(arg))
                        {
                            filesToFormat.Append(Path.GetFullPath(arg));
                        }
                        else
                        {
                            Console.WriteLine($"can't format {arg}");
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