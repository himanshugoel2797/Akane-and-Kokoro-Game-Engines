using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.KSL.Lib.Math;
using Kokoro.KSL.Lib.Texture;
using Kokoro.KSL.Lib.General;
using Kokoro.KSL.Lib;

#if PC
using CodeGenLL = Kokoro.KSL.GLSL.PC.PCCodeGenerator;
#endif

namespace Kokoro.KSL.GLSL
{
    partial class GLSLCodeGenerator
    {
        static string src;
        static StringBuilder strBuilder;

        internal static string CompileFromSyntaxTree(KSL.KSLCompiler.KShaderType shaderType)
        {
            src = "#version 440 core\n";
            strBuilder = new StringBuilder(src);

            List<SyntaxTree.Variable> StreamVars = new List<SyntaxTree.Variable>();
            List<SyntaxTree.Variable> UniformVars = new List<SyntaxTree.Variable>();
            List<SyntaxTree.Variable> SharedVars = new List<SyntaxTree.Variable>();

            foreach (KeyValuePair<string, SyntaxTree.Variable> pair in SyntaxTree.Parameters)
            {
                if (pair.Value.paramType == SyntaxTree.ParameterType.StreamIn || pair.Value.paramType == SyntaxTree.ParameterType.StreamOut)
                {
                    StreamVars.Add(pair.Value);
                }
                else if (pair.Value.paramType == SyntaxTree.ParameterType.Uniform)
                {
                    UniformVars.Add(pair.Value);
                }
                else if (pair.Value.paramType == SyntaxTree.ParameterType.SharedIn || pair.Value.paramType == SyntaxTree.ParameterType.SharedOut)
                {
                    SharedVars.Add(pair.Value);
                }
            }

            foreach (SyntaxTree.Variable variable in StreamVars)
            {
                strBuilder.AppendFormat("layout(location = {0}) {1} {2} {3};\n",
                    ((int)variable.value).ToString(),
                    ((variable.paramType == SyntaxTree.ParameterType.StreamOut) ? "out" : "in"),
                    ConvertType(variable.type), variable.name);
            }

            strBuilder.AppendLine();

            foreach (SyntaxTree.Variable variable in UniformVars)
            {
                strBuilder.AppendFormat("uniform {0} {1};\n",
                    ConvertType(variable.type),
                    variable.name);
            }

            strBuilder.AppendLine();

            foreach (SyntaxTree.Variable variable in SharedVars)
            {
                strBuilder.AppendFormat("{0} {1} {2};\n",
                    ((variable.paramType == SyntaxTree.ParameterType.SharedOut) ? "out" : "in"),
                    ConvertType(variable.type),
                    variable.name);
            }

            strBuilder.AppendLine();
            strBuilder.AppendLine("void main(){");

            while (SyntaxTree.Instructions.Count >= 1)
            {
                //Start going through Instruction queue to generate shader body
                SyntaxTree.Instruction instruction = SyntaxTree.Instructions.Dequeue();

                switch (instruction.instructionType)
                {
                    case SyntaxTree.InstructionType.Assign:
                        if (SyntaxTree.Variables.ContainsKey(instruction.Parameters[1]) && SyntaxTree.Variables[instruction.Parameters[0]].type == SyntaxTree.Variables[instruction.Parameters[1]].type)
                        {
                            strBuilder.AppendFormat("{0} = {1};\n", instruction.Parameters[0], instruction.Parameters[1]);
                        }else if(SyntaxTree.Variables.ContainsKey(instruction.Parameters[1]) && SyntaxTree.Variables[instruction.Parameters[0]].type != SyntaxTree.Variables[instruction.Parameters[1]].type)
                        {
                            strBuilder.AppendFormat("{0} = {1}({2});\n", instruction.Parameters[0], ConvertType(SyntaxTree.Variables[instruction.Parameters[0]].type),  instruction.Parameters[1]);
                        }
                        else
                        {
                            strBuilder.AppendFormat("{0} = {1};\n", instruction.Parameters[0], instruction.Parameters[1]);
                        }
                        break;

                    case SyntaxTree.InstructionType.Create:
                        SyntaxTree.Variable VAR = SyntaxTree.Variables[instruction.Parameters[0]];
                        strBuilder.AppendFormat("{0} {1};\n", ConvertType(VAR.type), VAR.name);
                        break;
                }
            }

            strBuilder.AppendLine("}");

            return strBuilder.ToString();
        }

        internal static string GetShader()
        {
            return src;
        }


        static string currentDeclaration = "";

        internal static string GenerateOperation(SyntaxTree.Instruction inst)
        {

            return "";
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
        internal static string TypeDeclaration(Type t, object val)
        {
            string tmp = ConvertType(t) + "(" + val.ToString() + ")";
            currentDeclaration = tmp;
            return tmp;
        }

        internal static void Return()
        {
            src += "return " + currentDeclaration;
        }
    }
}
