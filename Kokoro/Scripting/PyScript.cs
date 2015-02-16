using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.IO;

namespace Kokoro.Scripting
{
    public class Script
    {

        public dynamic Execute;

        public dynamic GetVariable(string name)
        {
            return ((ScriptScope)Execute).GetVariable(name);
        }

        public void SetVariable(string name, object obj)
        {
            ((ScriptScope)Execute).SetVariable(name, obj);
        }

        public static Script Load(string filename, params System.Reflection.Assembly[] assembliesToImport)
        {
            ScriptRuntime runtime = Python.CreateRuntime();
            ScriptEngine engine = runtime.GetEngine("py");

            runtime.LoadAssembly(typeof(Script).Assembly);
            for (int a = 0; a < assembliesToImport.Length; a++) { runtime.LoadAssembly(assembliesToImport[a]); }

            ScriptScope scope = engine.CreateScope();
            engine.ExecuteFile(filename, scope);

            Script s = new Script();
            s.Execute = scope;

            return s;
        }
    }
}
