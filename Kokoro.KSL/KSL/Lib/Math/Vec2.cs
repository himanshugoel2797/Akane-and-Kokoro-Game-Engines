using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib.Math
{
    public class Vec2 : Obj
    {
        #region Indexer Hack
        public object this[string swizzleMask]
        {
            get
            {
                if (swizzleMask.Length == 2)
                {
                    return new Vec2()
                    {
                        ObjName = this.ObjName + "." + swizzleMask
                    };
                }
                else if (swizzleMask.Length == 1)
                {
                    return new KFloat()
                    {
                        ObjName = this.ObjName + "." + swizzleMask
                    };
                }

                return new Vec2()
                {
                    ObjName = this.ObjName
                };
            }
            set
            {
                if (swizzleMask.Length == 2)
                {
                    Manager.Assign<Vec2>(this.ObjName + "." + swizzleMask, value);
                }
                else if (swizzleMask.Length == 1)
                {
                    Manager.Assign<KFloat>(this.ObjName + "." + swizzleMask, value);
                }
            }
        }
        #endregion

        public override object GetDefaultValue()
        {
            return 0;
        }

        #region Operators
        public static Vec2 operator *(KInt a, Vec2 b)
        {
            var k = new Vec2()
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

        public static Vec2 operator *(Vec2 a, Vec2 b)
        {
            var k = new Vec2()
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

        public static Vec2 operator /(Vec2 a, Vec2 b)
        {
            Vec2 k = new Vec2()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec2),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec2 operator /(KInt a, Vec2 b)
        {
            Vec2 k = new Vec2()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec2),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            Vec2 k = new Vec2()
            {
                ObjName = "(" + a.ObjName + "+" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec2),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            Vec2 k = new Vec2()
            {
                ObjName = "(" + a.ObjName + "-" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec2),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }
        #endregion

        #region Converters
        public static implicit operator Vec2(int i)
        {
            return new Vec2()
            {
                ObjName = i.ToString()
            };
        }

        public static implicit operator Vec2(KInt i)
        {
            return new Vec2()
            {
                ObjName = i.ObjName
            };
        }
        #endregion

        #region Non-Static Converters
        public void Construct(KInt a, KInt b)
        {
            this["x"] = a;
            this["y"] = b;
        }
        #endregion
    }
}
