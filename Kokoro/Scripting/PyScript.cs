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

        /// <summary>
        /// Get the value of a variable
        /// </summary>
        /// <param name="name">The variable name</param>
        /// <returns>The variable's value</returns>
        public dynamic GetVariable(string name)
        {
            return ((ScriptScope)Execute).GetVariable(name);
        }

        /// <summary>
        /// Set the value of a variable
        /// </summary>
        /// <param name="name">The variable name</param>
        /// <param name="obj">the variable value</param>
        public void SetVariable(string name, object obj)
        {
            ((ScriptScope)Execute).SetVariable(name, obj);
        }

        /// <summary>
        /// Load a Python script from file
        /// </summary>
        /// <param name="filename">The path to the file to load</param>
        /// <param name="assembliesToImport">Array of external assemblies to make available to the script</param>
        /// <returns></returns>
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
