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
        ScriptEngine engine;

        private Script(string filename)
        {
            string file = File.ReadAllText(filename);
            engine = Python.CreateEngine();
            var scope = engine.CreateScope();
            var scriptSrc = engine.CreateScriptSourceFromString(file);
        }

        public static Script Load(string filename) { return new Script(filename); }
    }
}
