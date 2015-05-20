using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;
using Kokoro.Engine;
using Kokoro.Debug;
using Kokoro.Engine.SceneGraph;
using Kokoro.Engine.Prefabs;
using Kokoro.Engine.Shaders;
using Kokoro.Engine.HighLevel.CharacterControllers;
using Kokoro.Engine.HighLevel.Cameras;
using Kokoro.Engine.HighLevel;
using Kokoro.Engine.HighLevel.Rendering;
using Kokoro.Engine.HighLevel.Rendering.Compositor;
using Kokoro.Engine.Input;

namespace Messier104.LocalObjects
{
    public class SpaceRock
    {
        static Sphere[] LODs;   //Represents 20 different levels of detail for a space rock

        protected float Mass;
        protected Texture terrainHeightMap, terrainColorMap;
        protected float Radius;
        public Vector3 Center;

        public Material ObjectMaterial;

        public SpaceRock()
        {

            if (LODs == null)
            {
                LODs = new Sphere[20];

                LODs[0] = new Sphere(1, 3);
                LODs[1] = new Sphere(1, 3);
                LODs[2] = new Sphere(1, 3);
                LODs[3] = new Sphere(1, 5);
                LODs[4] = new Sphere(1, 10);
                LODs[5] = new Sphere(1, 15);
                LODs[6] = new Sphere(1, 20);
                LODs[7] = new Sphere(1, 20);
                LODs[8] = new Sphere(1, 30);
                LODs[9] = new Sphere(1, 53);
                LODs[10] = new Sphere(1, 56);
                LODs[11] = new Sphere(1, 58);
                LODs[12] = new Sphere(1, 70);
                LODs[13] = new Sphere(1, 84);
                LODs[14] = new Sphere(1, 88);
                LODs[15] = new Sphere(1, 100);
                LODs[16] = new Sphere(1, 130);
                LODs[17] = new Sphere(1, 135);
                LODs[18] = new Sphere(1, 150);
                LODs[19] = new Sphere(1, 160);

            }
        }

        public SpaceRock(float Mass, float Radius, Vector3 center, Texture heightMap, Texture colorMap) : this()
        {

            this.Mass = Mass;
            this.Radius = Radius;
            this.terrainColorMap = colorMap;
            this.terrainHeightMap = heightMap;
            this.Center = center;
        }

        public void Render(GraphicsContext context, Vector3 playerPos)
        {
            float distSq = (playerPos - Center).LengthSquared;

            if (distSq < 10)
            {
                int index = 19 - (int)(distSq / 2);
                LODs[index].Materials[0] = ObjectMaterial;
                LODs[index].World = Matrix4.Scale(Radius / 1000) * Matrix4.CreateTranslation(Center);
                LODs[index].Draw(context);
            }
            else if (distSq < 50)
            {
                int index = 15 - (int)(distSq / 5);
                LODs[index].Materials[0] = ObjectMaterial;
                LODs[index].World = Matrix4.Scale(Radius / 1000) * Matrix4.CreateTranslation(Center);
                LODs[index].Draw(context);
            }
            else if (distSq < 1000 * 1000)
            {
                //start checking logarithmic distance from this point
                int index = 6 - (int)System.Math.Log(distSq, 20);
                LODs[index].Materials[0] = ObjectMaterial;
                LODs[index].World = Matrix4.Scale(Radius / 1000) * Matrix4.CreateTranslation(Center);
                LODs[index].Draw(context);
            }
            //context.ForceDraw();
        }
    }
}
