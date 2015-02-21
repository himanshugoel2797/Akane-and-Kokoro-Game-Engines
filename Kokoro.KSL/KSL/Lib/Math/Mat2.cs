using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib.Math
{
    public class Mat2 : Obj
    {
        #region Indexer Hack
        public object this[int indiceA]
        {
            get
            {
                if (indiceA < 2)
                {
                    return new Vec2()
                    {
                        ObjName = this.ObjName + "[" + indiceA + "]"
                    };
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                if (indiceA < 2)
                {
                    Manager.Assign<Vec2>(this.ObjName + "[" + indiceA + "]", value);
                }
            }
        }
        #endregion

        public override object GetDefaultValue()
        {
            return 0;
        }

        #region Operators
        public static Mat2 operator *(Vec2 a, Mat2 b)
        {
            var k = new Mat2()
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

        public static Mat2 operator *(Mat2 a, Mat2 b)
        {
            var k = new Mat2()
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

        public static Mat2 operator /(Mat2 a, Mat2 b)
        {
            Mat2 k = new Mat2()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat2),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Mat2 operator /(Vec2 a, Mat2 b)
        {
            Mat2 k = new Mat2()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat2),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Mat2 operator +(Mat2 a, Mat2 b)
        {
            Mat2 k = new Mat2()
            {
                ObjName = "(" + a.ObjName + "+" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat2),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Mat2 operator -(Mat2 a, Mat2 b)
        {
            Mat2 k = new Mat2()
            {
                ObjName = "(" + a.ObjName + "-" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat2),
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
        public void Construct(Vec2 row0, Vec2 row1)
        {
            this[0] = row0;
            this[1] = row1;
        }
        #endregion
    }
}
