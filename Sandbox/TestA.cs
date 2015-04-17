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
using Kokoro.Engine.HighLevel.Cameras;
using Kokoro.Engine.HighLevel.Rendering;
using Kokoro.Scripting;
using System.Diagnostics;

namespace Kokoro.Game
{
    class TestA : IScene
    {
        Model room, t2, fsq, fsq2;
        Texture tmp, wait;
        FrameBuffer buffer;
        object lockObject = new object();
        bool resourcesLoaded = false;

        public TestA(GraphicsContext context)
        {
            VFS.FSReader.LoadFileSystem("Resources", "Resources");
            //context.Wireframe = true;
            context.Camera = new FirstPersonCamera(Vector3.Zero, Vector3.UnitZ);
            context.Projection = Matrix4.CreatePerspectiveFieldOfView(0.78539f, 16f / 9f, context.ZNear, context.ZFar);
            context.DepthFunction = (x, y) => x <= y;
            context.Blending = new BlendFunc()
            {
                Src = BlendingFactor.SrcAlpha,
                Dst = BlendingFactor.OneMinusSrcAlpha
            };

            fsq = new Sphere(3);
            fsq2 = new FullScreenQuad();
            fsq2.Materials[0].Shader = ShaderLib.FrameBufferShader.Create();
            fsq.Materials[0].Shader = ShaderLib.DefaultShader.Create();
            wait = new Texture("Resources/Inori.jpg");
            buffer = new FrameBuffer((int)context.WindowSize.X, (int)context.WindowSize.Y, PixelComponentType.RGBA16f, context);
            fsq.Materials[0].ColorMap = wait;
            fsq2.Materials[0].ColorMap = buffer["Color"];

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
            if (resourcesLoaded)
            {
                lock (lockObject)
                {
                    if (t2 != null) t2.Draw(context);
                    if (room != null) room.Draw(context);
                }
            }
            else
            {
                buffer.Bind(context);
                context.Clear(0, 1.0f, 1.0f, 1);
                fsq.Draw(context);
                context.ForceDraw();
                buffer.UnBind(context);
                fsq2.Draw(context);
            }
            /*
             * 
             * The shader architecture works by defining a class which will be responsible for compiling all KSL shaders into one ubershader and assigning IDs to them so their params
             * can be accessed in the shader while hiding all of the complexity and management issues of dealing with uber shaders, this will however introduce a performance concern
             * if the shaders get too large.
             * 
             * Also, if moving physics to the GPU using the VoxSphere method, collisions might still need to be done on the CPU? Also how to deal with rotating objects without
             * seriously impacting performance? We could solve rotational dynamics on GPU as well but not sure about the performance, maybe sticking with CPU physics is the better idea?
             * 
             * But GPU physics and animations would free up the CPU for better AI.
             * 
             * Dealing with keeping some spheres fixed relative to others will be tricky as well, we do not want to evaluate all spheres as independent objects otherwise everything will just fall apart
             * one way would be to assign an ID to each set of spheres which represent an object, and those spheres will always keep the same distance and direction from the nearest sphere
             * This will still be tricky for animated meshes where the locations of the spheres will also change relative to each other, but at the same time they must not react to physics independently
             * 
             * Another issue is that low resolution sphere simulation will result in bumps and 'holes' in meshes, which will be a problem. The only workaround appears to be to do 
             * mesh collision detection and then to use the spheres to do other updates (like deforming). 
             * Another possibility would be to have multiple levels of detail of the spheres (like a spherical octree). The problem with this will be keeping the octree balanced, unless
             * a clever system to determine the offset quickly can be devised. If this can be achieved, the solution would be good for both physics and animations.
             * 
             * The final solution might be to use the compute shader for the simpler situations where the meshes aren't animated but need to be deformable, or for simulations of systems
             * like explosions of objects, basically hardware accelerated destructive physics. Making these interact with CPU based non-destructive physics however...
             * 
             * For now, implement Texture Arrays and Shader subroutines along with modifications to the KSL compiler to produce one indexed ubershader instead of multiple separate shaders 
             */

            context.SwapBuffers();
        }

        public void Update(double interval, GraphicsContext context)
        {
            context.Camera.Update(interval, context);
        }


        public void LoadResources(GraphicsContext context)
        {
            lock (lockObject)
            {
                tmp = new Texture("Resources/asuna.png");

                room = new Sphere(5, 50);//Model.Load("room.obj");
                room.Materials[0].Shader = ShaderLib.DefaultShader.Create();
                room.Materials[0].ColorMap = tmp;

                //t2 = new VertexMesh("Resources/room.obj", false);
                //for (int a = 0; a < t2.Materials.Length; a++)
                //{
                 //   t2.Materials[a].Shader = ShaderLib.DefaultShader.Create();
                 //   t2.Materials[a].ColorMap = tmp;
                //}

                //resourcesLoaded = true;
            }

        }
    }
}
