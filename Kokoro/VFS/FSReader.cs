using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscUtils.SquashFs;
using System.IO;
using System.IO.Compression;

namespace Kokoro.VFS
{
    public static class FSReader
    {
        /*
        A loosely designed file system, header is the file tree. Individual files can be RSA-encrypted with game specific keys, write mode is only available in 'EDITOR/DEBUG' build mode
        supports header RSA-CMAC hashing
    */

        static Dictionary<string, SquashFileSystemReader> Archives;
        static List<string> AllFiles;

        static FSReader()
        {
            Archives = new Dictionary<string, SquashFileSystemReader>();
            AllFiles = new List<string>();
        }

        public static void LoadFileSystem(string archive)
        {
            FileStream arch = File.OpenRead(archive);
            SquashFileSystemReader r = new SquashFileSystemReader(arch);

            AllFiles.AddRange(r.GetFileSystemEntries(r.Root.FullName));
            Archives.Add(archive, r);
        }

        public static void UnloadFileSystem(string archive)
        {
            if (Archives.ContainsKey(archive))
            {
                string[] tmp = Archives[archive].GetFileSystemEntries(Archives[archive].Root.FullName);
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
            //Check if the file exists
            if (AllFiles.Contains(file))
            {
                //Search all loaded filesystems for file
                foreach(SquashFileSystemReader reader in Archives.Values)
                {
                    if (reader.FileExists(file))
                    {
                        if (!decompress)
                        {
                            //Return the file stream
                            return reader.OpenFile(file, FileMode.Open, FileAccess.Read);
                        }else
                        {
                            //Decompress the file if specified
                            return new GZipStream(reader.OpenFile(file, FileMode.Open, FileAccess.Read), CompressionMode.Decompress);
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