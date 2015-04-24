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
        internal static Dictionary<string, string> PreDefinedVariablesMap = new Dictionary<string, string>();

        static GLSLCodeGenerator()
        {
            //Map some predefined variables for GLSL
            PreDefinedVariablesMap["VertexPosition"] = "gl_Position";
            PreDefinedVariablesMap["VertexID"] = "gl_VertexID";
            PreDefinedVariablesMap["InstanceID"] = "gl_InstanceID";
            PreDefinedVariablesMap["FragCoord"] = "gl_FragCoord";
        }

        static List<SyntaxTree.Variable> StreamVars = new List<SyntaxTree.Variable>();
        static List<SyntaxTree.Variable> UniformVars = new List<SyntaxTree.Variable>();
        static List<SyntaxTree.Variable> SharedVars = new List<SyntaxTree.Variable>();
        static List<SyntaxTree.Variable> TextureSamplers = new List<SyntaxTree.Variable>();
        internal static string GenerateShader(KSLCompiler.KShaderType ktype)
        {
            StreamVars = new List<SyntaxTree.Variable>();
            UniformVars = new List<SyntaxTree.Variable>();
            SharedVars = new List<SyntaxTree.Variable>();
            TextureSamplers = new List<SyntaxTree.Variable>();

            //Specify version as per Logic.AvailableSM
            switch (KSL.Lib.General.Logic.AvailableSM)
            {

                case ShadingModel.SM4:
                    src = "#version 440 core\n";    //Use attribute to specify version number?
                    break;
                case ShadingModel.SM3:
                    src = "#version 330 core\n";
                    break;
                case ShadingModel.SM2:
                    src = "#version 200 core\n";
                    break;
                case ShadingModel.SM1:
                    src = "";		//TODO look up about whether #version was even available back then
                    break;
            }
            strBuilder = new StringBuilder(src);

            //Get variable declarations from the syntax tree and organize them

            //Insert the ubershader index selection id, to select a shader, perform an in-fragment shader texture fetch
            //We should have multiple switches to control which kind of ubershader is generated, there should also only be one vertex shader



            foreach (KeyValuePair<string, SyntaxTree.Variable> pair in SyntaxTree.Parameters)
            {
                if (pair.Value.paramType == SyntaxTree.ParameterType.StreamIn || pair.Value.paramType == SyntaxTree.ParameterType.StreamOut)
                {
                    StreamVars.Add(pair.Value);
                }
                else if (pair.Value.paramType == SyntaxTree.ParameterType.Uniform)
                {
                    if (pair.Value.type != typeof(Sampler1D) && pair.Value.type != typeof(Sampler2D) && pair.Value.type != typeof(Sampler3D)) UniformVars.Add(pair.Value);
                    else TextureSamplers.Add(pair.Value);
                }
                else if (pair.Value.paramType == SyntaxTree.ParameterType.SharedIn || pair.Value.paramType == SyntaxTree.ParameterType.SharedOut)
                {
                    SharedVars.Add(pair.Value);
                }
            }


            //Generate code for the stream variables
            foreach (SyntaxTree.Variable variable in StreamVars)
            {
                strBuilder.AppendFormat("layout(location = {0}) {1} {2} {3};\n",
                    ((int)variable.value).ToString(),
                    ((variable.paramType == SyntaxTree.ParameterType.StreamOut) ? "out" : "in"),
                    ConvertType(variable.type), variable.name);
            }

            strBuilder.AppendLine();

#if STUPID_CRAP
            if (ktype == KSLCompiler.KShaderType.Vertex)
            {
                //TODO Generate all uniforms as a uniform buffer array for AZDO
                //Generate code for the uniform variables
                strBuilder.Append("layout (std140) uniform Inputs {\n");
                for (int i = 0; i < UniformVars.Count; i++)
                {
                    strBuilder.AppendFormat("{0} {1};\n",
                        ConvertType(UniformVars[i].type),
                        UniformVars[i].name);
                }
                strBuilder.Append("} inputData[16];\n"); //NOTE: 512 is the maximum number of draw calls that can be submitted in one multidraw call
            }
#else
            foreach (SyntaxTree.Variable variable in UniformVars)
            {
                strBuilder.AppendFormat("uniform {0} {1};\n", ConvertType(variable.type), variable.name);
            }
#endif
            strBuilder.AppendLine();

            foreach (SyntaxTree.Variable variable in TextureSamplers)
            {
                strBuilder.AppendFormat("uniform {0} {1};\n", ConvertType(variable.type), variable.name);
            }

            strBuilder.AppendLine();

            //Generate code for the shared variables
            foreach (SyntaxTree.Variable variable in SharedVars)
            {
                strBuilder.AppendFormat("{0} {1} {2} {3};\n",
                    variable.extraInfo.ToLower(),
                    ((variable.paramType == SyntaxTree.ParameterType.SharedOut) ? "out" : "in"),
                    ConvertType(variable.type),
                    variable.name);
            }
            strBuilder.AppendLine();
            strBuilder.Append("void main(){\n");

            //Build the code body using the fundamental operations available to the language
            while (SyntaxTree.Instructions.Count >= 1)
            {
                //Start going through Instruction queue to generate shader body
                SyntaxTree.Instruction instruction = SyntaxTree.Instructions.Dequeue();

                switch (instruction.instructionType)
                {
                    case SyntaxTree.InstructionType.Assign:
                        if (SyntaxTree.Variables.ContainsKey(instruction.Parameters[1]) && SyntaxTree.Variables[instruction.Parameters[0]].type == SyntaxTree.Variables[instruction.Parameters[1]].type)
                        {
                            strBuilder.AppendFormat("{0} = {1};\n", SubstitutePredefinedVars(instruction.Parameters[0]), SubstitutePredefinedVars(instruction.Parameters[1]));
                        }
                        else if (SyntaxTree.Variables.ContainsKey(instruction.Parameters[1]) && SyntaxTree.Variables[instruction.Parameters[0]].type != SyntaxTree.Variables[instruction.Parameters[1]].type)
                        {
                            strBuilder.AppendFormat("{0} = {1}({2});\n", SubstitutePredefinedVars(instruction.Parameters[0]), ConvertType(SyntaxTree.Variables[instruction.Parameters[0]].type), SubstitutePredefinedVars(instruction.Parameters[1]));
                        }
                        else
                        {
                            strBuilder.AppendFormat("{0} = {1};\n", SubstitutePredefinedVars(instruction.Parameters[0]), SubstitutePredefinedVars(instruction.Parameters[1]));
                        }
                        break;

                    case SyntaxTree.InstructionType.Create:
                        SyntaxTree.Variable VAR = SyntaxTree.Variables[instruction.Parameters[0]];
                        int aLen = 0;
                        if (IsArrayType(VAR.value, VAR.type, out aLen)) strBuilder.AppendFormat("{0} {1}[{2}];\n", ConvertType(VAR.type), VAR.name, aLen);
                        else strBuilder.AppendFormat("{0} {1};\n", ConvertType(VAR.type), VAR.name);
                        break;

                    case SyntaxTree.InstructionType.If:
                        strBuilder.Append("if " + instruction.Parameters[0] + " {\n");
                        break;
                    case SyntaxTree.InstructionType.EndIf:
                        strBuilder.Append("\n}\n");
                        break;
                    case SyntaxTree.InstructionType.Else:
                        strBuilder.Append("else { \n");
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

        internal static string TranslateCondition(Obj l, Comparisons c, Obj r)
        {
            string toRet = "((" + SubstitutePredefinedVars(l.ObjName) + ")";
            switch (c)
            {
                case Comparisons.And:
                    toRet += " && ";
                    break;
                case Comparisons.Or:
                    toRet += " || ";
                    break;
                case Comparisons.Equal:
                    toRet += " == ";
                    break;
                case Comparisons.GreaterThan:
                    toRet += " > ";
                    break;
                case Comparisons.LessThan:
                    toRet += " < ";
                    break;
                case Comparisons.GreaterThanEqual:
                    toRet += " >= ";
                    break;
                case Comparisons.LessThanEqual:
                    toRet += " <= ";
                    break;
                case Comparisons.Xor:
                    toRet += " ^^ ";
                    break;
            }
            toRet += "(" + SubstitutePredefinedVars(r.ObjName) + "))";
            return toRet;
        }

        //Translate KSL function calls to GLSL equivalents
        internal static string TranslateSDKFunctionCalls(SyntaxTree.FunctionCalls function, params string[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = Manager.TranslateVarName(parameters[i]);
            }
            string str = "";

            switch (function)
            {
                case SyntaxTree.FunctionCalls.Tex3D:
                    str = "texture(" + parameters[0] + "," + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Tex2D:
                    str = "texture(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Tex1D:
                    str = "texture(" + parameters[0] + ", " + parameters[1] + ")";
                    break;

                case SyntaxTree.FunctionCalls.Cross2D:
                    str = "cross(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Cross3D:
                    str = "cross(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Cross4D:
                    str = "cross(" + parameters[0] + ", " + parameters[1] + ")";
                    break;

                case SyntaxTree.FunctionCalls.Normalize2D:
                    str = "normalize(" + parameters[0] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Normalize3D:
                    str = "normalize(" + parameters[0] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Normalize4D:
                    str = "normalize(" + parameters[0] + ")";
                    break;

                case SyntaxTree.FunctionCalls.Mod:
                    str = "mod(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Clamp:
                    str = "clamp(" + parameters[0] + ", " + parameters[1] + ", " + parameters[2] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Dot:
                    str = "dot(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Sin:
                    str = "sin(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Cos:
                    str = "cos(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Tan:
                    str = "tan(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.ASin:
                    str = "asin(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.ACos:
                    str = "acos(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.ATan:
                    str = "atan(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
                case SyntaxTree.FunctionCalls.Fract:
                    str = "fract(" + parameters[0] + ", " + parameters[1] + ")";
                    break;
            }

            return str;
        }

        //Substitute reserved predefined variables with the GLSL specifc name
        internal static string SubstitutePredefinedVars(string varName)
        {
#if STUPID_CRAP
            var tmp = SyntaxTree.Parameters.Values.ToArray();
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i].paramType == SyntaxTree.ParameterType.Uniform && tmp[i].type != typeof(Sampler1D) && tmp[i].type != typeof(Sampler2D) && tmp[i].type != typeof(Sampler3D) && tmp[i].name == varName) varName = "inputData[DrawID]." + varName;
            }
#endif

            foreach (KeyValuePair<string, string> substitutions in PreDefinedVariablesMap)
            {
                varName = varName.Replace(substitutions.Key, substitutions.Value);
            }

            return varName;
        }

        internal static bool IsArrayType(object a, Type t, out int length)
        {
            length = 0;
            bool isArray = false;

            if (t == typeof(KArray<KFloat>))
            {
                isArray = true;
                length = ((KArray<KFloat>)a).Length;
            }
            else if (t == typeof(KArray<Vec2>))
            {
                isArray = true;
                length = ((KArray<Vec2>)a).Length;
            }
            else if (t == typeof(KArray<Vec3>))
            {
                isArray = true;
                length = ((KArray<Vec3>)a).Length;
            }
            else if (t == typeof(KArray<Vec4>))
            {
                isArray = true;
                length = ((KArray<Vec4>)a).Length;
            }
            else if (t == typeof(KArray<Mat4>))
            {
                isArray = true;
                length = ((KArray<Mat4>)a).Length;
            }
            else if (t == typeof(KArray<Mat3>))
            {
                isArray = true;
                length = ((KArray<Mat3>)a).Length;
            }
            else if (t == typeof(KArray<Mat2>))
            {
                isArray = true;
                length = ((KArray<Mat2>)a).Length;
            }
            else if (t == typeof(KArray<Sampler1D>))
            {
                isArray = true;
                length = ((KArray<Sampler1D>)a).Length;
            }
            else if (t == typeof(KArray<Sampler2D>))
            {
                isArray = true;
                length = ((KArray<Sampler2D>)a).Length;
            }
            else if (t == typeof(KArray<Sampler3D>))
            {
                isArray = true;
                length = ((KArray<Sampler3D>)a).Length;
            }

            return isArray;
        }

        //Convert the C# Type object to its equivalent GLSL string
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
            else if (t == typeof(Mat4)) tStr = "mat4";
            else if (t == typeof(Mat3)) tStr = "mat3";
            else if (t == typeof(Mat2)) tStr = "mat2";
            else if (t == typeof(Sampler1D)) tStr = "sampler1D";
            else if (t == typeof(Sampler2D)) tStr = "sampler2D";
            else if (t == typeof(Sampler3D)) tStr = "sampler3D";
            else if (t == typeof(KArray<KFloat>)) tStr = "float";
            else if (t == typeof(KArray<KInt>)) tStr = "int";
            else if (t == typeof(KArray<Vec2>)) tStr = "vec2";
            else if (t == typeof(KArray<Vec3>)) tStr = "vec3";
            else if (t == typeof(KArray<Vec4>)) tStr = "vec4";
            else if (t == typeof(KArray<Mat4>)) tStr = "mat4";
            else if (t == typeof(KArray<Mat3>)) tStr = "mat3";
            else if (t == typeof(KArray<Mat2>)) tStr = "mat2";
            else if (t == typeof(KArray<Sampler1D>)) tStr = "sampler1D";
            else if (t == typeof(KArray<Sampler2D>)) tStr = "sampler2D";
            else if (t == typeof(KArray<Sampler3D>)) tStr = "sampler3D";

            return tStr;
        }

        //Generate a type declaration
        internal static string TypeDeclaration(Type t, object val)
        {
            string tmp = ConvertType(t) + "(" + val.ToString() + ")";
            currentDeclaration = tmp;
            return tmp;
        }

        //Generate a return statement
        internal static void Return()
        {
            src += "return " + currentDeclaration;
        }
    }
}
