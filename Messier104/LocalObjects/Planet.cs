using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Math;
using Messier104.HeavenlyObjects;
using Kokoro.Engine;

namespace Messier104.LocalObjects
{
    class Planet : SpaceRock
    {
        public SolarSystem Parent;
        public Moon[] Children;
        public float OrbitDistance;
        public float X, Y;
        Random rng;

        public Planet(int seed, Material objMaterial)
            : base()
        {
            rng = new Random(seed);

            this.Radius = rng.Next(500, 10000);
            this.Mass = rng.Next(10000, 500000);
            this.OrbitDistance = (float)rng.Next(1000, 50000) / 10000f;
            this.ObjectMaterial = objMaterial;

            Children = new Moon[rng.Next(0, 10)];
            for (int i = 0; i < Children.Length; i++)
            {
                Children[i] = new Moon(rng.Next(50, (int)(this.Radius / 2)), rng.Next(1000, (int)(Mass / 2)));
                Children[i].Parent = this;
                Children[i].ObjectMaterial = this.ObjectMaterial;
                Children[i].OrbitDistance = rng.Next((int)(Radius + 100), (int)(Radius + 1000)) / 1000f;
            }
        }

        public void Draw(GraphicsContext context, double interval, Vector3 pos)
        {
            this.ObjectMaterial.Shader["inColor"] = new Vector4(1, 0, 0, 1);
            Render(context, pos);
            for (int i = 0; i < Children.Length; i++)
            {
                Children[i].ObjectMaterial.Shader["inColor"] = new Vector4(0, 1, 0, 1);
                Children[i].Render(context, pos);
            }
        }

        public void Update(double interval)
        {
            this.Center = Parent.Center + Vector3.FromSpherical(new Vector3(OrbitDistance, X, Y));
            //X += MathHelper.DegreesToRadians((float)interval / 100000f);

            for (int i = 0; i < Children.Length; i++)
            {
                Children[i].Update(interval);
            }
        }
    }
}
