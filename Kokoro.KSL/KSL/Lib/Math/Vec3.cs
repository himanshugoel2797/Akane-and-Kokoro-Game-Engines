using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib.Math
{
    public class Vec3 : Obj
    {
        #region Indexer Hack
        public object this[string swizzleMask]
        {
            get
            {
                if (swizzleMask.Length == 3)
                {
                    return new Vec3()
                    {
                        ObjName = this.ObjName + "." + swizzleMask
                    };
                }
                else if (swizzleMask.Length == 2)
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

                return new Vec3()
                {
                    ObjName = this.ObjName
                };
            }
            set
            {
                if (swizzleMask.Length == 3)
                {
                    Manager.Assign<Vec3>(this.ObjName + "." + swizzleMask, value);
                }
                else if (swizzleMask.Length == 2)
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
        public static Vec3 operator *(KInt a, Vec3 b)
        {
            var k = new Vec3()
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

        public static Vec3 operator *(Vec3 a, Vec3 b)
        {
            var k = new Vec3()
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

        public static Vec3 operator /(Vec3 a, Vec3 b)
        {
            Vec3 k = new Vec3()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec3),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec3 operator /(KInt a, Vec3 b)
        {
            Vec3 k = new Vec3()
            {
                ObjName = "(" + a.ObjName + "/" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec3),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec3 operator +(Vec3 a, Vec3 b)
        {
            Vec3 k = new Vec3()
            {
                ObjName = "(" + a.ObjName + "+" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec3),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }

        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            Vec3 k = new Vec3()
            {
                ObjName = "(" + a.ObjName + "-" + b.ObjName + ")"
            };

            SyntaxTree.Variables.Add(k.ObjName, new SyntaxTree.Variable()
            {
                type = typeof(Vec3),
                value = null,
                paramType = SyntaxTree.ParameterType.Variable,
                name = k.ObjName
            });

            return k;
        }
        #endregion

        #region Converters
        public static implicit operator Vec3(int i)
        {
            return new Vec3()
            {
                ObjName = i.ToString()
            };
        }

        public static implicit operator Vec3(KInt i)
        {
            return new Vec3()
            {
                ObjName = i.ObjName
            };
        }
        #endregion

        #region Non-Static Converters
        public void Construct(Vec2 vec, KInt i)
        {
            this["xy"] = vec;
            this["z"] = i;
        }
        #endregion
    }
}
