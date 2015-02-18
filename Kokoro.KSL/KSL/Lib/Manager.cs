using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.ComponentModel;

namespace Kokoro.KSL.Lib
{
    public class Manager
    {
        static dynamic VarDB;

        private static void HandlePropertyChanges(
        object sender, PropertyChangedEventArgs e)
        {
            if (((IDictionary<string, object>)VarDB)[e.PropertyName] as Obj != null)
            {
                SyntaxTree.Instructions.Enqueue(new SyntaxTree.Instruction()
                {
                    instructionType = SyntaxTree.InstructionType.Assign,
                    Parameters = new string[] { e.PropertyName, ((Obj)((IDictionary<string, object>)VarDB)[e.PropertyName]).ObjName }
                });
            }
        }

        public static dynamic ShaderStart()
        {
            VarDB = new ExpandoObject();
            ((INotifyPropertyChanged)VarDB).PropertyChanged +=
            new PropertyChangedEventHandler(HandlePropertyChanges);

            return VarDB;
        }

        public static void ShaderEnd()
        {

        }

        public static void SharedIn<T>(string name) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            SyntaxTree.Parameters.Add(name, new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = null,
                paramType = SyntaxTree.ParameterType.SharedIn,
                name = name
            });

            SyntaxTree.Variables.Add(name, SyntaxTree.Parameters[name]);

            ((IDictionary<string, object>)VarDB).Add(name, null);
        }

        public static void SharedOut<T>(string name) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            SyntaxTree.Parameters.Add(name, new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = null,
                paramType = SyntaxTree.ParameterType.SharedOut,
                name = name
            });

            SyntaxTree.Variables.Add(name, SyntaxTree.Parameters[name]);

            ((IDictionary<string, object>)VarDB).Add(name, null);
        }

        public static void StreamIn<T>(string name, int location) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            SyntaxTree.Parameters.Add(name, new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = location,
                paramType = SyntaxTree.ParameterType.StreamIn,
                name = name
            });

            SyntaxTree.Variables.Add(name, SyntaxTree.Parameters[name]);

            ((IDictionary<string, object>)VarDB).Add(name, location);
        }

        public static void StreamOut<T>(string name, int location) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            SyntaxTree.Parameters.Add(name, new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = location,
                paramType = SyntaxTree.ParameterType.StreamOut,
                name = name
            });

            SyntaxTree.Variables.Add(name, SyntaxTree.Parameters[name]);

            ((IDictionary<string, object>)VarDB).Add(name, location);
        }


        public static void Uniform<T>(string name) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            SyntaxTree.Parameters.Add(name, new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = null,
                paramType = SyntaxTree.ParameterType.Uniform,
                name = name
            });

            SyntaxTree.Variables.Add(name, SyntaxTree.Parameters[name]);

            ((IDictionary<string, object>)VarDB).Add(name, tmp);
        }

        public static void Create<T>(string name, T val) where T : Obj, new()
        {
            //TODO Implement type specific object value assigners

            SyntaxTree.Variables.Add(name, new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = val,
                paramType = SyntaxTree.ParameterType.Variable,
                name = name
            });

            SyntaxTree.Instructions.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Create,
                Parameters = new string[] { name }
            });

            SyntaxTree.Instructions.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Assign,
                Parameters = new string[] { name, (val == null) ? "" : ((val.ToString() == "") ? val.ObjName : val.ToString()) }
            });

            ((IDictionary<string, object>)VarDB).Add(name, val);
        }

        static int tmpVarIDs = 0;
        internal static string TemporaryVariable<T>(T val = null) where T : Obj, new()
        {
            Create<T>("tmp" + tmpVarIDs++, val);
            return "tmp" + (tmpVarIDs - 1);
        }

        internal static void Assign(string name, object val)
        {
            ((IDictionary<string, object>)VarDB).Add(name, val);
        }

    }
}
