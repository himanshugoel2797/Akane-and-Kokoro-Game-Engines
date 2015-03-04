//#if (DEBUG && !CHECK_FILESYSTEMS) || RELEASE
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.VFS
{
    public static class FSReader
    {
        //A version of the same file system API only this just wraps to normal FS calls for debugging
        static Dictionary<string, string> Archives;
        static List<string> AllFiles;

        static FSReader()
        {
            Archives = new Dictionary<string, string>();
            AllFiles = new List<string>();

            LoadFileSystem(Environment.CurrentDirectory);
        }

        public static void LoadFileSystem(string archive)
        {
            Archives.Add(archive, archive);
            AllFiles.AddRange(Directory.EnumerateFileSystemEntries(archive));
        }

        public static void UnloadFileSystem(string archive)
        {
            if (Archives.ContainsKey(archive))
            {
                string[] tmp = Directory.EnumerateFileSystemEntries(archive, "*", SearchOption.AllDirectories).ToArray();
                AllFiles.RemoveRange(AllFiles.IndexOf(tmp[0]), tmp.Length);
                Archives.Remove(archive);
            }
        }

        /// <summary>
        /// Check if an item (file or Directory) exists
        /// </summary>
        /// <param name="file">The path to check</param>
        /// <returns>True if the item exists</returns>
        public static bool ItemExists(string file)
        {
            return AllFiles.Contains(file);
        }

        /// <summary>
        /// Open a file
        /// </summary>
        /// <param name="file">The path to the file to open</param>
        /// <returns>The File Stream</returns>
        public static Stream OpenFile(string file, bool decompress = false)
        {
            //TODO: temporary hack for annoying emulated filesystem bug
            return new FileStream(file, FileMode.Open);

            //Check if the file exists
            if (AllFiles.Contains(file))
            {
                //Search all loaded filesystems for file
                foreach (string reader in Archives.Values)
                {
                    if (File.Exists(Path.Combine(reader, file)))
                    {
                        if (!decompress)
                        {
                            //Return the file stream
                            return File.OpenRead(Path.Combine(reader, file));
                        }
                        else
                        {
                            //Decompress the file if specified
                            return new GZipStream(File.OpenRead(Path.Combine(reader, file)), CompressionMode.Decompress);
                        }
                    }
                }

                //This will never be reached
                return null;
            }
            else throw new FileNotFoundException("File Not Found", file);
        }

    }
}
//#endif