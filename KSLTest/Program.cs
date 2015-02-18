using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSLTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Kokoro.KSL.KSLCompiler.Compile(new TestShader(), Kokoro.KSL.KSLCompiler.KShaderType.Vertex));

            Console.ReadLine();
        }
    }
}
