using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib.Math
{
    public class Mat4 : Obj
    {
        #region Indexer Hack
        public object this[int indiceA]
        {
            get
            {
                if (indiceA < 4)
                {
                    return new Vec4()
                    {
                        ObjName = this.ObjName + "[" + indiceA + "]"
                    };
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                if (indiceA < 4)
                {
                    Manager.Assign<Vec4>(this.ObjName + "[" + indiceA + "]", value);
                }
            }
        }
        #endregion

        public override object GetDefaultValue()
        {
            return 0;
        }

        #region Operators
        public static Mat4 operator *(Mat4 a, Mat4 b)
        {
            var k = new Mat4()
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

        public static Mat4 operator /(Mat4 a, Mat4 b)
        {
            Mat4 k = new Mat4()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat4),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Mat4 operator +(Mat4 a, Mat4 b)
        {
            Mat4 k = new Mat4()
            {
                ObjName = "(" + a.ObjName + "+" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat4),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Mat4 operator -(Mat4 a, Mat4 b)
        {
            Mat4 k = new Mat4()
            {
                ObjName = "(" + a.ObjName + "-" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat4),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }
        #endregion

        #region Converters
        //TODO Implement converters to Matrix3?
        #endregion

        #region Non-Static Converters
        public void Construct(Vec4 row0, Vec4 row1, Vec4 row2, Vec4 row3)
        {
            this[0] = row0;
            this[1] = row1;
            this[2] = row2;
            this[3] = row3;
        }
        #endregion
    }
}
