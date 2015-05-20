using Kokoro.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Math;
using Kokoro.ShaderLib;

namespace Messier104.LocalObjects
{
    class SolarSystem
    {
        public Planet[] Children;
        public Vector3 Center;

        Random rng;

        static int times = 0;

        public SolarSystem(int seed)
        {
            rng = new Random(seed);

            Children = new Planet[rng.Next(0, 20)];

            Material m = new Material()
            {
                Name = "spaceRockAMat",
                Shader = ColorDefaultShader.Create()
            };

            m.Shader.PreApply += (A, b) =>
            {
                times++;
            };
            for (int i = 0; i < Children.Length; i++)
            {
                Children[i] = new Planet(rng.Next(), m);
                Children[i].Parent = this;
            }
        }

        public void Draw(GraphicsContext context, double interval, Vector3 pos)
        {
            for (int i = 0; i < Children.Length; i++)
            {
                Children[i].Draw(context, interval, pos);
            }

            Console.WriteLine("times : " + times);
            times = 0;
        }

        public void Update(double interval)
        {
            for (int i = 0; i < Children.Length; i++)
            {
                Children[i].Update(interval);
            }
        }
    }
}
