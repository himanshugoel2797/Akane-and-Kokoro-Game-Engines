using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if GLSL
using CodeGenerator = Kokoro.KSL.GLSL.GLSLCodeGenerator;
#if PC
using Kokoro.KSL.GLSL.PC;
#endif
#endif

namespace Kokoro.KSL.Lib.Math
{
    public class KMath
    {
        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            var k = new Vec3()
            {
                ObjName = CodeGenerator.TranslateSDKFunctionCalls(SyntaxTree.FunctionCalls.Cross3D, a.ObjName, b.ObjName)
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = k.GetType(),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec2 Cross(Vec2 a, Vec2 b)
        {
            var k = new Vec2()
            {
                ObjName = CodeGenerator.TranslateSDKFunctionCalls(SyntaxTree.FunctionCalls.Cross2D, a.ObjName, b.ObjName)
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = k.GetType(),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec4 Cross(Vec4 a, Vec4 b)
        {
            var k = new Vec4()
            {
                ObjName = CodeGenerator.TranslateSDKFunctionCalls(SyntaxTree.FunctionCalls.Cross4D, a.ObjName, b.ObjName)
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = k.GetType(),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec2 Normalize(Vec2 a)
        {
            var k = new Vec2()
            {
                ObjName = CodeGenerator.TranslateSDKFunctionCalls(SyntaxTree.FunctionCalls.Normalize2D, a.ObjName)
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = k.GetType(),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec3 Normalize(Vec3 a)
        {
            var k = new Vec3()
            {
                ObjName = CodeGenerator.TranslateSDKFunctionCalls(SyntaxTree.FunctionCalls.Normalize3D, a.ObjName)
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = k.GetType(),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec4 Normalize(Vec4 a)
        {
            var k = new Vec4()
            {
                ObjName = CodeGenerator.TranslateSDKFunctionCalls(SyntaxTree.FunctionCalls.Normalize4D, a.ObjName)
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = k.GetType(),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

    }
}
