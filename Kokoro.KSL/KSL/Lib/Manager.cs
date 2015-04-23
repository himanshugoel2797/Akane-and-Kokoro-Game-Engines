using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.ComponentModel;
using Kokoro.KSL.Lib.General;

#if GLSL
using CodeGenerator = Kokoro.KSL.GLSL.GLSLCodeGenerator;
#if PC
using Kokoro.KSL.GLSL.PC;
#endif
#endif

namespace Kokoro.KSL.Lib
{
    /// <summary>
    /// Manages variable creation
    /// </summary>
    public class Manager
    {
        internal static dynamic VarDB;

        //Define shader name metadata and setup a global shader manager class in the engine to handle the representation of all shaders as subroutines
        //We now have dynamic models, we need to setup draw calls for them as well
        //Need to work on more functions for the shading language
        //Implement the path tracer in KSL and attempt to speed it up if possible, use the oppurtunity to study if a realtime path tracing process is feasible

        static bool uberMode = false;
        internal static void UbershaderMode(bool enabled)
        {
            //Ubershader mode is used to make the system ignore some problems like the ShaderStart to allow the continuous chain to be evaluated
            uberMode = enabled;
        }

        internal static string TranslateVarName(string var)
        {
            return CodeGenerator.SubstitutePredefinedVars(var);
        }

        private static void HandlePropertyChanges(
        object sender, PropertyChangedEventArgs e)
        {
            //Handle assignment operations so we can generate equivalent shading language instructions
            if (((IDictionary<string, object>)VarDB)[e.PropertyName] as Obj != null && e.PropertyName != ((Obj)((IDictionary<string, object>)VarDB)[e.PropertyName]).ObjName)
            {
                SyntaxTree.Instructions.Enqueue(new SyntaxTree.Instruction()
                {
                    instructionType = SyntaxTree.InstructionType.Assign,
                    Parameters = new string[] { e.PropertyName, ((Obj)((IDictionary<string, object>)VarDB)[e.PropertyName]).ObjName }
                });

                ((Obj)((IDictionary<string, object>)VarDB)[e.PropertyName]).ObjName = e.PropertyName;
            }
        }

        internal static void Init()
        {
            VarDB = new ExpandoObject();
            SyntaxTree.Variables = new Dictionary<string, SyntaxTree.Variable>();
            SyntaxTree.Variables.Add("VertexPosition", new SyntaxTree.Variable()
            {
                name = "VertexPosition",
                paramType = SyntaxTree.ParameterType.Variable,
                type = typeof(Math.Vec4),
                value = null
            });
            ((IDictionary<string, object>)VarDB).Add("VertexPosition", new Math.Vec4()
            {
                ObjName = "VertexPosition"
            });

            SyntaxTree.Variables.Add("VertexID", new SyntaxTree.Variable()
            {
                name = "VertexID",
                paramType = SyntaxTree.ParameterType.Variable,
                type = typeof(Math.KInt),
                value = null
            });
            ((IDictionary<string, object>)VarDB).Add("VertexID", new Math.KInt()
            {
                ObjName = "VertexID"
            });

            SyntaxTree.Variables.Add("InstanceID", new SyntaxTree.Variable()
            {
                name = "InstanceID",
                paramType = SyntaxTree.ParameterType.Variable,
                type = typeof(Math.KInt),
                value = null
            });
            ((IDictionary<string, object>)VarDB).Add("InstanceID", new Math.KInt()
            {
                ObjName = "InstanceID"
            });

            SyntaxTree.Variables.Add("FragCoord", new SyntaxTree.Variable()
            {
                name = "FragCoord",
                paramType = SyntaxTree.ParameterType.Variable,
                type = typeof(Math.Vec2),
                value = null
            });
            ((IDictionary<string, object>)VarDB).Add("FragCoord", new Math.Vec2()
            {
                ObjName = "FragCoord"
            });


            //Reset the engine state
            ((INotifyPropertyChanged)VarDB).PropertyChanged +=
            new PropertyChangedEventHandler(HandlePropertyChanges);

            SyntaxTree.AssignmentBuffer = new Queue<SyntaxTree.Instruction>();
            SyntaxTree.Instructions = new Queue<SyntaxTree.Instruction>();
            SyntaxTree.Parameters = new Dictionary<string, SyntaxTree.Variable>();
            SyntaxTree.SharedVariables = new Dictionary<string, SyntaxTree.Variable>();

            //PreDefine any variables requested by the host
            foreach (KeyValuePair<string, Obj> t in KSLCompiler.preDefUniforms)
            {
                var tmp = t.Value;
                tmp.ObjName = t.Key;

                SyntaxTree.Parameters.Add(t.Key, new SyntaxTree.Variable()
                {
                    type = tmp.GetType(),
                    value = null,
                    paramType = SyntaxTree.ParameterType.Uniform,
                    name = t.Key
                });

                SyntaxTree.Variables.Add(t.Key, SyntaxTree.Parameters[t.Key]);

                ((IDictionary<string, object>)VarDB).Add(t.Key, tmp);
            }
        }

        /// <summary>
        /// Marks the start of a new shader
        /// </summary>
        /// <returns>Provides a dynamic object which will contain all the shader's variables</returns>
        public static dynamic ShaderStart(string name)
        {
            //Define predefined variables beforehand

            SyntaxTree.ShaderName = name;
            return VarDB;
        }

        public static void ShaderEnd() { }

        /// <summary>
        /// Marks the end of a shader object
        /// </summary>
        public static void ShaderEnd<A, B>(A input, B output)
            where A : struct
            where B : struct
        {
            var inputFields = input.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var outputFields = output.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            //TODO Streams might be bugged, test later

            #region Input Fields
            for (int i = 0; i < inputFields.Length; i++)
            {
                var iFieldAttributes = inputFields[i].CustomAttributes.ToArray();
                for (int i1 = 0; i1 < iFieldAttributes.Length; i1++)
                {
                    if (iFieldAttributes[i1].AttributeType == typeof(SharedAttribute))
                    {
                        #region Shared In
                        SyntaxTree.Parameters[inputFields[i].Name] = new SyntaxTree.Variable()
                        {
                            type = inputFields[i].FieldType,
                            value = inputFields[i].GetValue(input),
                            paramType = SyntaxTree.ParameterType.SharedIn,
                            name = inputFields[i].Name,
                            extraInfo = ((Interpolators)iFieldAttributes[i1].ConstructorArguments[0].Value).ToString()
                        };

                        SyntaxTree.Variables[inputFields[i].Name] = SyntaxTree.Parameters[inputFields[i].Name];
                        #endregion
                    }
                    else if (iFieldAttributes[i1].AttributeType == typeof(UniformAttribute))
                    {
                        #region Uniform
                        SyntaxTree.Parameters[inputFields[i].Name] = new SyntaxTree.Variable()
                        {
                            type = inputFields[i].FieldType,
                            value = inputFields[i].GetValue(input),
                            paramType = SyntaxTree.ParameterType.Uniform,
                            name = inputFields[i].Name
                        };

                        SyntaxTree.Variables[inputFields[i].Name] = SyntaxTree.Parameters[inputFields[i].Name];
                        #endregion
                    }
                    else if (iFieldAttributes[i1].AttributeType == typeof(StreamAttribute))
                    {
                        #region Stream In
                        SyntaxTree.Parameters[inputFields[i].Name] = new SyntaxTree.Variable()
                        {
                            type = inputFields[i].FieldType,
                            value = inputFields[i].GetValue(input),
                            paramType = SyntaxTree.ParameterType.StreamIn,
                            name = inputFields[i].Name,
                            extraInfo = ((int)iFieldAttributes[i1].ConstructorArguments[0].Value).ToString()
                        };

                        SyntaxTree.Variables[inputFields[i].Name] = SyntaxTree.Parameters[inputFields[i].Name];
                        #endregion
                    }
                }
            }
            #endregion

            #region Output Fields
            for (int i = 0; i < outputFields.Length; i++)
            {
                var iFieldAttributes = outputFields[i].CustomAttributes.ToArray();
                for (int i1 = 0; i1 < iFieldAttributes.Length; i1++)
                {
                    if (iFieldAttributes[i1].AttributeType == typeof(SharedAttribute))
                    {
                        #region Shared Out
                        SyntaxTree.Parameters[outputFields[i].Name] = new SyntaxTree.Variable()
                        {
                            type = outputFields[i].FieldType,
                            value = outputFields[i].GetValue(output),
                            paramType = SyntaxTree.ParameterType.SharedOut,
                            name = outputFields[i].Name,
                            extraInfo = ((Interpolators)iFieldAttributes[i1].ConstructorArguments[0].Value).ToString()
                        };

                        SyntaxTree.Variables[outputFields[i].Name] = SyntaxTree.Parameters[outputFields[i].Name];
                        #endregion
                    }
                    else if (iFieldAttributes[i1].AttributeType == typeof(StreamAttribute))
                    {
                        #region Stream Out
                        SyntaxTree.Parameters[outputFields[i].Name] = new SyntaxTree.Variable()
                        {
                            type = outputFields[i].FieldType,
                            value = outputFields[i].GetValue(output),
                            paramType = SyntaxTree.ParameterType.StreamOut,
                            name = outputFields[i].Name,
                            extraInfo = ((int)iFieldAttributes[i1].ConstructorArguments[0].Value).ToString()
                        };

                        SyntaxTree.Variables[outputFields[i].Name] = SyntaxTree.Parameters[outputFields[i].Name];
                        #endregion
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Define a new shader shared input value
        /// </summary>
        /// <typeparam name="T">The type of the input</typeparam>
        /// <param name="name">The name of the variable</param>
        public static void SharedIn<T>(string name, Interpolators interpolator) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            SyntaxTree.Parameters[name] = new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = null,
                paramType = SyntaxTree.ParameterType.SharedIn,
                name = name,
                extraInfo = interpolator.ToString()
            };

            SyntaxTree.Variables[name] = SyntaxTree.Parameters[name];

            ((IDictionary<string, object>)VarDB)[name] = tmp;
        }

        /// <summary>
        /// Define a new shader shared output value
        /// </summary>
        /// <typeparam name="T">The type of the output</typeparam>
        /// <param name="name">The name of the variable</param>
        public static void SharedOut<T>(string name, Interpolators interpolator) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            SyntaxTree.Parameters[name] = new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = null,
                paramType = SyntaxTree.ParameterType.SharedOut,
                name = name,
                extraInfo = interpolator.ToString()
            };

            SyntaxTree.Variables[name] = SyntaxTree.Parameters[name];

            ((IDictionary<string, object>)VarDB)[name] = null;
        }

        /// <summary>
        /// Define a new Stream In location variable
        /// </summary>
        /// <typeparam name="T">The Type of the variable</typeparam>
        /// <param name="name">The name of the variable</param>
        /// <param name="location">The location of the variable</param>
        public static void StreamIn<T>(string name, int location) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            SyntaxTree.Parameters[name] = new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = location,
                paramType = SyntaxTree.ParameterType.StreamIn,
                name = name
            };

            SyntaxTree.Variables[name] = SyntaxTree.Parameters[name];

            ((IDictionary<string, object>)VarDB)[name] = tmp;
        }

        /// <summary>
        /// Define a new Stream Out location variable
        /// </summary>
        /// <typeparam name="T">The type of the variable</typeparam>
        /// <param name="name">The name of the variable</param>
        /// <param name="location">The location of the variable</param>
        public static void StreamOut<T>(string name, int location) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            SyntaxTree.Parameters[name] = new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = location,
                paramType = SyntaxTree.ParameterType.StreamOut,
                name = name
            };

            SyntaxTree.Variables[name] = SyntaxTree.Parameters[name];

            ((IDictionary<string, object>)VarDB)[name] = tmp;
        }

        /// <summary>
        /// Define a new uniform variable
        /// </summary>
        /// <typeparam name="T">The type of the variable</typeparam>
        /// <param name="name">The name of the variable</param>
        internal static void RequestUniform<T>(string name) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            SyntaxTree.Parameters[name] = new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = null,
                paramType = SyntaxTree.ParameterType.Uniform,
                name = name
            };

            SyntaxTree.Variables[name] = SyntaxTree.Parameters[name];

            ((IDictionary<string, object>)VarDB)[name] = tmp;
        }

        internal static void DeclareUniformFromType(string name, Type t)
        {
            if (uberMode)   //This is a hacky setup so only allow it in UberMode
            {
                object tmp = Activator.CreateInstance(t);
                (tmp as Obj).ObjName = name;

                SyntaxTree.Parameters[name] = new SyntaxTree.Variable()
                {
                    type = t,
                    value = null,
                    paramType = SyntaxTree.ParameterType.Uniform,
                    name = name
                };

                SyntaxTree.Variables[name] = SyntaxTree.Parameters[name];

                ((IDictionary<string, object>)VarDB)[name] = tmp;
            }
        }

        /// <summary>
        /// Define a new variable
        /// </summary>
        /// <typeparam name="T">The type of the variable</typeparam>
        /// <param name="name">The name of the variable</param>
        public static void Create<T>(string name) where T : Obj, new()
        {
            T tmp = new T();
            tmp.ObjName = name;

            //TODO Implement type specific object value assigners
            SyntaxTree.Variables[name] = new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = name
            };

            SyntaxTree.Instructions.Enqueue(new SyntaxTree.Instruction()
            {
                instructionType = SyntaxTree.InstructionType.Create,
                Parameters = new string[] { name }
            });

            ((IDictionary<string, object>)VarDB)[name] = tmp;
        }

        static int tmpVarIDs = 0;
        internal static string TemporaryVariable<T>() where T : Obj, new()
        {
            Create<T>("tmp" + tmpVarIDs++);
            return "tmp" + (tmpVarIDs - 1);
        }

        internal static void Assign<T>(string name, object val)
        {
            if (!SyntaxTree.Variables.ContainsKey(name))
            {
                SyntaxTree.Variables[name] = new SyntaxTree.Variable();
            }

            SyntaxTree.Variables[name] = new SyntaxTree.Variable()
            {
                type = typeof(T),
                value = val,
                paramType = SyntaxTree.ParameterType.Variable,
                name = name
            };

            ((IDictionary<string, object>)VarDB)[name] = val;
        }

    }
}
