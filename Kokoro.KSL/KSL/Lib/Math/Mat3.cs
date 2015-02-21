using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib.Math
{
    public class Mat3 : Obj
    {
        #region Indexer Hack
        public object this[int indiceA]
        {
            get
            {
                if (indiceA < 3)
                {
                    return new Vec3()
                    {
                        ObjName = this.ObjName + "[" + indiceA + "]"
                    };
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                if (indiceA < 3)
                {
                    Manager.Assign<Vec3>(this.ObjName + "[" + indiceA + "]", value);
                }
            }
        }
        #endregion

        public override object GetDefaultValue()
        {
            return 0;
        }

        #region Operators
        public static Mat3 operator *(Vec3 a, Mat3 b)
        {
            var k = new Mat3()
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

        public static Mat3 operator *(Mat3 a, Mat3 b)
        {
            var k = new Mat3()
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

        public static Mat3 operator /(Mat3 a, Mat3 b)
        {
            Mat3 k = new Mat3()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat3),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Mat3 operator /(Vec3 a, Mat3 b)
        {
            Mat3 k = new Mat3()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat3),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Mat3 operator +(Mat3 a, Mat3 b)
        {
            Mat3 k = new Mat3()
            {
                ObjName = "(" + a.ObjName + "+" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat3),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Mat3 operator -(Mat3 a, Mat3 b)
        {
            Mat3 k = new Mat3()
            {
                ObjName = "(" + a.ObjName + "-" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Mat3),
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
        public void Construct(Vec3 row0, Vec3 row1, Vec3 row2)
        {
            this[0] = row0;
            this[1] = row1;
            this[2] = row2;
        }
        #endregion
    }
}
