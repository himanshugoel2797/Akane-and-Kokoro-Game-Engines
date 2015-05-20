using Kokoro.Engine;
using Kokoro.Math;
using Messier104.LocalObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messier104.HeavenlyObjects
{
    class Star : SpaceRock
    {
        public SolarSystem Children;
        Random rng;

        public Star(int seed)
        {
            rng = new Random(seed);
            Children = new SolarSystem(rng.Next());
        }

        public void Draw(GraphicsContext context, double interval, Vector3 pos)
        {
            Children.Draw(context, interval, pos);
        }

        public void Update(double interval)
        {
            Children.Update(interval);
        }
    }
}
