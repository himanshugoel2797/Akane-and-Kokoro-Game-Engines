using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace Kokoro.ContentPipeline
{
    class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            try
            {
#endif
                if (args.Length != 3 || args[0] == "-h" || args[0] == "--h")
                {
                    Console.WriteLine("Kokoro Content Processor\nUsage:\n\tKokoro.ContentPipeline [COMMAND] [ARCHIVE] [FILE/DIRECTORY PATH]\nCommands:\n  -c\tCreate Archive from Directory\n  -a\tAppend file to archive\n  -3dm\tConvert file to engine format and add to archive\n  -b\tBatch mode - Read commands from supplied XML file");
                }
                else
                {
                    ProcessFS2(args);
                }
#if !DEBUG
            }
            catch (Exception) { }   //Ignore all errors if release build
#endif
        }

        static void ProcessFS2(string[] args)
        {
            if (args[0] != "-b")
            {
                ProcessFS(args[0], args[1], args[2]);
            }
            else
            {
                string[] commands = File.ReadAllLines(args[1]);

                for (int a = 0; a < commands.Length; a++)
                {
                    string cmd = commands[a].Split(' ')[0];
                    ProcessFS2(new string[] { cmd, commands[a].Replace(cmd, "").Split(',')[0], commands[a].Replace(cmd, "").Split(',')[1] });       //Allow recursive commands in a command list
                }
            }
        }

        static void ProcessFS(string cmd, string archiveName, string option)
        {
            if (cmd == "-c")
            {
                ZipFile.CreateFromDirectory(option, archiveName, CompressionLevel.Fastest, false);
            }
            else if (cmd == "-a")
            {
                ZipArchive archive = new ZipArchive(new System.IO.FileStream(archiveName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite));
                archive.CreateEntryFromFile(option, option);
            }
            else if (cmd == "-3dm")
            {
                ZipArchive archive = new ZipArchive(new System.IO.FileStream(archiveName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite));
                if (new string[] { ".obj", ".3ds", ".dae" }.Contains(Path.GetExtension(option))) File.WriteAllBytes(Path.GetFileNameWithoutExtension(option) + ".ko", ModelConvert.Process(option));
                archive.CreateEntryFromFile(Path.GetFileNameWithoutExtension(option) + ".ko", option);
            }
        }

    }
}
