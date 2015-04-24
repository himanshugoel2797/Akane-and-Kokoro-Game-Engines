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

namespace Messier104
{
    class InGame : IScene
    {
        bool loaded = false;

        FirstPersonCamera FPSCam;
        SpaceRock spaceRockA;
        Material spaceRockAMat;

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
            context.Clear(0f, 0f, 0f, 1f);
            if (loaded)
            {
                spaceRockAMat.Shader["inColor"] = Vector4.One;
                spaceRockA.ObjectMaterial = spaceRockAMat;
                spaceRockA.Draw(context, FPSCam.Position);
            }
            context.SwapBuffers();
        }

        public void Update(double interval, GraphicsContext context)
        {
            if (loaded)
            {

            }
        }

        public void LoadResources(GraphicsContext context)
        {
            if (FPSCam == null)
            {
                FPSCam = new FirstPersonCamera(Vector3.Zero, Vector3.UnitY);
                context.Camera = FPSCam;
            }

            if (spaceRockA == null)
            {
                spaceRockA = new SpaceRock(100, 1000, Vector3.Zero, null, null);
                spaceRockAMat = new Material()
                {
                    Name = "spaceRockAMat",
                    Shader = ColorDefaultShader.Create()
                };
                spaceRockAMat.Shader["inColor"] = Vector4.One;
                spaceRockA.ObjectMaterial = spaceRockAMat;
            }

            context.Wireframe = true;
            context.FaceCulling = CullMode.Back;
            loaded = true;
        }
    }
}
