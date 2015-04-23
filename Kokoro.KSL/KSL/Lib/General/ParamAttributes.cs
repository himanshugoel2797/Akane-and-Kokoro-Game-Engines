using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL.Lib.General
{
    public enum Interpolators { Smooth = 0, Flat }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class SharedAttribute : Attribute
    {
        public Interpolators Interpolator { get; set; }
        public SharedAttribute(Interpolators interpolator)
        {
            Interpolator = interpolator;
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class UniformAttribute : Attribute
    {
        public UniformAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class StreamAttribute : Attribute
    {
        public int Location;
        public StreamAttribute(int location)
        {
            Location = location;
        }
    }
}
