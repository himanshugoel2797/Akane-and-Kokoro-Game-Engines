using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib.Math
{
    public class Vec4 : Obj
    {
        public override object GetDefaultValue()
        {
            return 0;
        }
        #region Operators
        public static Vec4 operator *(KInt a, Vec4 b)
        {
            var k = new Vec4()
            {
                ObjName = "(" + a.ObjName + "*" + b.ObjName + ")"
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

        public static Vec4 operator *(Vec4 a, Vec4 b)
        {
            var k = new Vec4()
            {
                ObjName = "(" + a.ObjName + "*" + b.ObjName + ")"
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

        public static Vec4 operator /(Vec4 a, Vec4 b)
        {
            Vec4 k = new Vec4()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec4),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec4 operator +(Vec4 a, Vec4 b)
        {
            Vec4 k = new Vec4()
            {
                ObjName = "(" + a.ObjName + "+" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec4),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec4 operator -(Vec4 a, Vec4 b)
        {
            Vec4 k = new Vec4()
            {
                ObjName = "(" + a.ObjName + "-" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec4),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }
        #endregion

        #region Converters
        public static implicit operator Vec4(int i)
        {
            return new Vec4()
            {
                ObjName = i.ToString()
            };
        }

        public static implicit operator Vec4(KInt i)
        {
            return new Vec4()
            {
                ObjName = i.ObjName
            };
        }
        #endregion
    }
}
