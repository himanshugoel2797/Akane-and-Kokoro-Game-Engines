using Kokoro.KSL.Lib.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.OpenGL.PC.GLSL
{
    public class CodeGenerator
    {
        static string src;

        internal static void Init()
        {
            src = "";
        }

        internal static string GetShader()
        {
            return src;
        }

        internal static string ConvertType(Type t)
        {
            string tStr = "";

            if (t == typeof(KFloat))
            {
                tStr = "float";
            }
            else if (t == typeof(KInt)) tStr = "int";
            else if (t == typeof(Vec2)) tStr = "vec2";
            else if (t == typeof(Vec3)) tStr = "vec3";
            else if (t == typeof(Vec4)) tStr = "vec4";

            return tStr;
        }

        internal static void TypeDeclaration(Type t, object val)
        {
            src += ConvertType(t) + "(" + val.ToString() + ")";
        }

    }
}
