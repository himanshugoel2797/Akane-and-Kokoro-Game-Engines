#if RELEASE || (DEBUG && CHECK_FILESYSTEMS)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.VFS
{
    public class FSReader
    {
        /*
        A loosely designed file system, header is the file tree. Individual files can be RSA-encrypted with game specific keys, write mode is only available in 'EDITOR/DEBUG' build mode
        supports header RSA-CMAC hashing
    */
    }
}
#endif