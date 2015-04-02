using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib
{
    public class KArray<T> : Obj where T : Obj, new()
    {
        internal Type type;
        internal int Length;

        public KArray(int length)
        {
            type = typeof(T);
            Length = length;
        }

        public T this[int index]
        {
            get
            {
                if (index < Length && index >= 0)
                {
                    return new T()
                    {
                        ObjName = this.ObjName + "[" + index + "]"
                    };
                }
                throw new IndexOutOfRangeException("'index' was out of range");
            }
            set
            {
                if (index < Length && index >= 0)
                {
                    Manager.Assign<T>(this.ObjName + "[" + index + "]", value);
                }
            }
        }
    }
}
