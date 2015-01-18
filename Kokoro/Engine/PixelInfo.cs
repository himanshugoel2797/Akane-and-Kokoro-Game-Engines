using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine
{
    public enum PixelFormat
    {
        RGBA, BGRA, Depth
    }

    public enum PixelComponentType
    {
        RGBA8, RGBA16f, RGBA32f, D24S8, D32, D32S8, SRGBA8
    }

    public enum PixelType
    {
        UByte332, UInt1010102, UInt248, UInt8888, UShort4444, UShort5551, Float, HalfFloat
    }
}
