using System.Collections;
using System.Collections.Generic;

namespace JFormat
{
    class JFormat
    {
        static string getFileType(string filename) { return filename.Split('.')[1]; }
        static bool isSupportedFileType(string filename)
        {
            return ((string[])(["json"])).Contains(getFileType(filename));
        }

        static void Main(string[] args)
        {
            Console.WriteLine(JsonFormatter.FormatString("{}"));
            //var filesToFormat = new List<String>();
            ////string filepath = "";
            //var cwd = Directory.GetCurrentDirectory();

            //foreach (var arg in args)
            //{
            //    switch (arg)
            //    {
            //        case "*":
            //            filesToFormat.Concat(Directory.GetFiles(cwd));
            //            break;
            //        default:
            //            if (isSupportedFileType(arg))
            //            {
            //                filesToFormat.Append(Path.GetFullPath(arg));
            //            }
            //            else
            //            {
            //                Console.WriteLine($"can't format {arg}");
            //            }
            //            break;
            //    }
            //}

            //foreach (var file in filesToFormat)
            //{
            //}
        }
    }
}