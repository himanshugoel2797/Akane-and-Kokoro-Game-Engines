using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Math;

namespace Kokoro.Physics
{
    public class VPoint
    {
        Vector3 pos;
        public Vector3 Position
        {
            get
            {
                return pos;
            }
            set
            {
                OldPosition = pos;  //The current position is now old
                pos = value;        //Update the current position
            }
        }
        public Vector3 OldPosition { get; private set; }
        public Vector3 Acceleration { get; set; }
    }
}
