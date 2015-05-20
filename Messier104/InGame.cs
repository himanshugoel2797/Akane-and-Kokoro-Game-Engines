using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;
using Kokoro.Engine;
using Kokoro.Debug;
using Kokoro.ShaderLib;
using Kokoro.Engine.SceneGraph;
using Kokoro.Engine.Prefabs;
using Kokoro.Engine.Shaders;
using Kokoro.Engine.HighLevel.CharacterControllers;
using Kokoro.Engine.HighLevel.Cameras;
using Kokoro.Engine.HighLevel;
using Kokoro.Engine.HighLevel.Rendering;
using Kokoro.Engine.HighLevel.Rendering.Compositor;
using Kokoro.Engine.Input;
using Messier104.LocalObjects;
using Messier104.HeavenlyObjects;

namespace Messier104
{
    class InGame : IScene
    {
        bool loaded = false;

        FirstPersonCamera FPSCam;
        Star star;
        SpaceRock a, b;

        public InGame(GraphicsContext context)
        {
            context.ResourceManager += LoadResources;
        }

        public IScene Parent
        {
            get;
            set;
        }

        public void Render(double interval, GraphicsContext context)
        {
            context.Clear(0f, 1f, 0f, 1f);
            if (loaded)
            {
                star.Draw(context, interval, FPSCam.Position);
                //a.Render(context, FPSCam.Position);
                //b.Render(context, FPSCam.Position);
            }
            context.SwapBuffers();
        }

        public void Update(double interval, GraphicsContext context)
        {
            //Console.WriteLine(interval);
            if (loaded)
            {
                star.Update(interval / 100);
            }
        }

        public void LoadResources(GraphicsContext context)
        {
            if (FPSCam == null)
            {
                FPSCam = new FirstPersonCamera(Vector3.Zero, Vector3.UnitY);
                context.Camera = FPSCam;
            }

            if (star == null)
            {
                star = new Star(500);
                //star.Update(100);

                a = new SpaceRock(10, 1000, Vector3.Zero, null, null);
                a.ObjectMaterial = new Material()
                {
                    Shader = ColorDefaultShader.Create()
                };
                b = new SpaceRock(10, 1000, new Vector3(1, 1, 1), null, null);
                b.ObjectMaterial = a.ObjectMaterial;
            }

            context.Wireframe = true;
            //context.FaceCulling = CullMode.Back;
            loaded = true;
        }
    }
}
