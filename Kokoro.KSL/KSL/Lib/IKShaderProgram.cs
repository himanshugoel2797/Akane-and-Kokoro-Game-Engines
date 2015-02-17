using Kokoro.KSL.Lib.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib
{
    public interface IKShaderProgram
    {
        Vec4 Vertex();
        Vec4 Fragment(); 
    }
}
