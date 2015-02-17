using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib.Math
{
    public class KInt : INum
    {
        private int value;


        public static implicit operator int(KInt i)
        {
            return i.value;
        }

        public static implicit operator KInt(int i)
        {
            KInt k = new KInt();
            k.value = i;
            return k;
        }

    }
}
