using Kokoro.Engine;
using Kokoro.Engine.HighLevel.Cameras;
using Kokoro.Engine.Prefabs;
using Kokoro.Engine.SceneGraph;
using Kokoro.Engine.Shaders;
using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    public class PathTracer : IScene
    {
        Model fsq;

        public PathTracer(GraphicsContext context)
        {
            Kokoro.VFS.FSReader.LoadFileSystem("Resources", "Resources");
            context.Camera = new FirstPersonCamera(Vector3.Zero, Vector3.UnitZ);
            context.Projection = Matrix4.CreatePerspectiveFieldOfView(0.78539f, 16f / 9f, context.ZNear, context.ZFar);
            context.DepthFunction = (x, y) => x <= y;
            context.Blending = new BlendFunc()
            {
                Src = BlendingFactor.SrcAlpha,
                Dst = BlendingFactor.OneMinusSrcAlpha
            };

            fsq = new FullScreenQuad();
            fsq.Materials[0].Shader = new ShaderProgram(VertexShader.Load("Resources"), FragmentShader.Load("Resources"));
        }

        public IScene Parent
        {
            get;
            set;
        }

        Stopwatch s = Stopwatch.StartNew();
        Vector4 MPos = Vector4.Zero;
        public void Render(double interval, GraphicsContext context)
        {
            context.Clear(0, 0.5f, 1.0f, 0.0f);

            fsq.Materials[0].Shader["iResolution"] = new Vector3(context.WindowSize.X, context.WindowSize.Y, 0);
            if (Kokoro.Engine.Input.Mouse.ButtonsDown.Left || Kokoro.Engine.Input.Mouse.ButtonsDown.Right)
            {
                fsq.Materials[0].Shader["iMouse"] = new Vector4(Kokoro.Engine.Input.Mouse.MousePos);
            }
            fsq.Materials[0].Shader["iGlobalTime"] = (float)(s.ElapsedMilliseconds / 1000f);
            fsq.Draw(context);

            context.SwapBuffers();
        }

        public void Update(double interval, GraphicsContext context)
        {
            context.Camera.Update(interval, context);
        }


        public void LoadResources(GraphicsContext context)
        {

        }
    }
}
