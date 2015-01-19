using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;

using Kokoro.Engine;

namespace Kokoro.Scripting
{
    public class Script
    {
        MemoryStream outputStream, pdbStream;
        Assembly assembly;
        
        private Script(string file)
        {
            string script = File.ReadAllText(file);

            var syntaxTree = CSharpSyntaxTree.ParseText(script);

            var engineDll = Path.GetFullPath(typeof(Script).Assembly.Location);
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var defaultReferences = new[] { "mscorlib.dll", "System.dll", "System.Core.dll" };

            // Compile the SyntaxTree to a CSharpCompilation
            var compilation = CSharpCompilation.Create(file,
                new[] { syntaxTree },
                defaultReferences.Select(x => MetadataReference.CreateFromFile(Path.Combine(assemblyPath, x))),
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            outputStream = new MemoryStream();
            pdbStream = new MemoryStream();
            // Emit assembly to streams.
            var result = compilation.Emit(outputStream, pdbStream: pdbStream);
            if (!result.Success)
            {
                return;
            }

            // Load the emitted assembly.
            assembly = Assembly.Load(outputStream.ToArray(), pdbStream.ToArray());
        }

        public object InvokeStaticMethod(string typeName, string methodName, params object[] parameters)
        {
            return assembly.GetType(typeName).GetMethod(methodName).Invoke(null, parameters);
        }


        public static Script Load(string filename)
        {
            return new Script(filename);
        }

    }
}
