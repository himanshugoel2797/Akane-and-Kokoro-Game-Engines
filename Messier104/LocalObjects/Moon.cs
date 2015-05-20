using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messier104.LocalObjects
{
    class Moon : SpaceRock
    {
        public Planet Parent;
        public float OrbitDistance;
        public float X, Y;

        public Moon(float radius, float mass)
        {
            this.Radius = radius;
            this.Mass = mass;

        }

        public void Update(double interval)
        {
            this.Center = Parent.Center + Vector3.FromSpherical(new Vector3(OrbitDistance, X, Y));
            //Y += MathHelper.DegreesToRadians((float)interval / 100000);
        }
    }
}
