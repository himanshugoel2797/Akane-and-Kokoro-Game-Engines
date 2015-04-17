using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

using Kokoro.Engine.HighLevel.Cameras;
using Kokoro.Math;
using Kokoro.Engine.Input;

//Conditional compilation used to manage multiplatform support
#if OPENGL
using Kokoro.OpenGL;    //If building for OpenGL
#if PC
using Kokoro.OpenGL.PC;
using System.Windows.Forms;     //If building for OpenGL on PC (Windows, Linux, Mac)
#endif

#elif OPENGL_AZDO
#if PC
using Kokoro.OpenGL.AZDO;
#endif

#endif

namespace Kokoro.Engine
{
    /// <summary>
    /// The Face Culling Modes
    /// </summary>
    public enum CullMode { Off, Back, Front }

    /// <summary>
    /// The Blending Factors
    /// </summary>
    public enum BlendingFactor
    {
        Zero = 0,
        One,
        SrcAlpha,
        OneMinusSrcAlpha,
        DstAlpha,
        OneMinusDstAlpha,
        DstColor,
        OneMinusDstColor,
        SrcAlphaSaturate,
        ConstantColorExt,
        ConstantColor,
        OneMinusConstantColorExt,
        OneMinusConstantColor,
        ConstantAlphaExt,
        ConstantAlpha,
        OneMinusConstantAlpha,
        OneMinusConstantAlphaExt,
        Src1Alpha,
        Src1Color,
        OneMinusSrc1Color,
        OneMinusSrc1Alpha
    }

    /// <summary>
    /// The Blend Function
    /// </summary>
    public struct BlendFunc
    {
        public BlendingFactor Src;
        public BlendingFactor Dst;
    }

    /// <summary>
    /// The GraphicsContext acts as a high level wrapper to the lower level functionality exposed by the platform dependant code
    /// </summary>
    public class GraphicsContext : GraphicsContextLL
    {
        #region State Machine Properties
        /// <summary>
        /// Enable/Disable Wireframe rendering
        /// </summary>
        public bool Wireframe
        {
            get
            {
                return base.GetWireframe();
            }
            set
            {
                base.SetWireframe(value);
            }
        }

        /// <summary>
        /// Enable/Disable writing to the Depth buffer
        /// </summary>
        public bool DepthWrite
        {
            get
            {
                return base.GetDepthWrite();
            }
            set
            {
                base.SetDepthWrite(value);
            }
        }

        /// <summary>
        /// Enable/Disable face culling
        /// </summary>
        public CullMode FaceCulling
        {
            get
            {
                return base.GetCullMode();
            }
            set
            {
                base.SetCullMode(value);
            }
        }

        /// <summary>
        /// Get/Set the Depth Function
        /// </summary>
        public Func<float, float, bool> DepthFunction
        {
            get
            {
                return base.GetDepthFunc();
            }
            set
            {
                base.SetDepthFunc(value);
            }
        }

        /// <summary>
        /// Set the Projection Matrix
        /// </summary>
        public Matrix4 Projection { get; set; }

        /// <summary>
        /// Set the View Matrix
        /// </summary>
        public Matrix4 View
        {
            get
            {
                return Camera.View;
            }
            set
            {
                Camera.View = value;
            }
        }

        /// <summary>
        /// Set the current Camera
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// Get/Set the rendering viewport
        /// </summary>
        public Vector4 Viewport
        {
            get
            {
                return base.GetViewport();
            }
            set
            {
                base.SetViewport(value);
            }
        }

        /// <summary>
        /// Set the Blend function
        /// </summary>
        public BlendFunc Blending
        {
            get
            {
                return GetBlendFunc();
            }
            set
            {
                SetBlendFunc(value);
            }
        }

        /// <summary>
        /// The Far-clipping plane
        /// </summary>
        public float ZFar
        {
            get
            {
                return base.GetZFar();
            }
            set
            {
                base.SetZFar(value);
            }
        }

        /// <summary>
        /// The Near-clipping plane
        /// </summary>
        public float ZNear
        {
            get
            {
                return base.GetZNear();
            }
            set
            {
                base.SetZNear(value);
            }
        }

        /// <summary>
        /// Get/Set the MSAA sample count
        /// </summary>
        public int MSAALevel
        {
            get
            {
                return base.GetMSAALevel();
            }
            set
            {
                base.ResetMSAA();
                base.InitializeMSAA(value);
            }
        }

        /// <summary>
        /// Set the Viewport Window Size
        /// </summary>
        public Vector2 WindowSize
        {
            get
            {
                return GetWinSize();
            }
            set
            {
                SetWinSize(value);
            }
        }
        #endregion

        //TODO fix MSAA
        #region MSAA
        internal void SetMSAABuffer() { base.SetMSAA(); }
        #endregion

        #region Clear
        /// <summary>
        /// Clear the screen
        /// </summary>
        /// <param name="r">The red component from 0 to 1</param>
        /// <param name="g">The green component from 0 to 1</param>
        /// <param name="b">The blue component from 0 to 1</param>
        /// <param name="a">The alpha component from 0 to 1</param>
        public void Clear(float r, float g, float b, float a)
        {
            base.aClear(r, g, b, a);
        }
        /// <summary>
        /// Clear the screen
        /// </summary>
        /// <param name="col">The color to clear the screen with (all 0 to 1)</param>
        public void Clear(Vector4 col) { Clear(col.X, col.Y, col.Z, col.W); }
        #endregion

        #region Game Loop
        /// <summary>
        /// The thread running the Update Loop
        /// </summary>
        public Thread UpdateThread { get; private set; }
        /// <summary>
        /// The thread running the Physics Loop
        /// </summary>
        public Thread PhysicsThread { get; private set; }
        /// <summary>
        /// The thread running the Animation loop
        /// </summary>
        public Thread AnimationThread { get; private set; }
        /// <summary>
        /// The Render thread
        /// </summary>
        public Thread RenderThread { get; private set; }
        /// <summary>
        /// The thread on which the engine resource manager runs
        /// </summary>
        public Thread ResourceManagerThread { get; private set; }     //NOTE: The resource manager also deals with balancing the world octree, as a result it manages the resources in the tree by unloading any objects which are too far away for current use

        /// <summary>
        /// The Update handler
        /// </summary>
        public Action<double, GraphicsContext> Update { get; set; }
        /// <summary>
        /// The Render handler
        /// </summary>
        public Action<double, GraphicsContext> Render { get; set; }
        /// <summary>
        /// The animation handler
        /// </summary>
        public Action<double, GraphicsContext> Animation { get; set; }
        /// <summary>
        /// The physics handler
        /// </summary>
        public Action<double, GraphicsContext> Physics { get; set; }
        /// <summary>
        /// The initialization handler
        /// </summary>
        public Action<GraphicsContext> Initialize { get; set; }
        /// <summary>
        /// The Resource Manager handler - Use for Async resource loading
        /// </summary>
        public Action<GraphicsContext> ResourceManager { get; set; }
        /// <summary>
        /// Start the game loop
        /// </summary>
        public void Start(int tpf, int tpu)
        {
            //Update handler thread
            UpdateThread = new Thread(() =>
            {
                Stopwatch su = Stopwatch.StartNew();
                //TODO: Implement skipping to prevent the spiral of death
                while (true)
                {
                    if (Update != null)
                    {
                        lock (Update)
                        {
                            //Update all input data
                            Keyboard.Update();
                            Mouse.Update();

                            //Call update handler
                            Update((tpu == 0) ? GetNormTicks(su) : tpu, this);
                        }
                    }

                    if (tpu != 0 && tpu > GetNormTicks(su))
                    {
                        try
                        {
                            Thread.Sleep(TimeSpan.FromTicks((long)tpu - (long)GetNormTicks(su)));
                        }
                        catch (Exception) { }
                    }
                    Kokoro.Debug.ObjectAllocTracker.PostUPS(GetNormTicks(su));
                    su.Reset();
                    su.Start();
                }
            });

            //Physics handler thread
            PhysicsThread = new Thread(() =>
            {
                GameLooper(160000, Physics);
            });

            AnimationThread = new Thread(() =>
            {
                GameLooper(80000, Animation);
            });

            RenderThread = new Thread(() =>
            {
                GameLooper(tpf, Render);
            });

            ResourceManagerThread = new Thread(() =>
            {
                GameLooper(160000, (a, b) =>
                {
                    if (ResourceManager != null) ResourceManager(b);
                    ResourceManager = null;
                });
            });
            #region LL executor
            Stopwatch s = new Stopwatch();
            bool tmpCtrl = false;
            ViewportControl.Paint += (a, b) =>
            {
                //TODO setup command buffer system
                if (inited)
                {
                    if (!tmpCtrl)
                    {
                        Initialize(this);
                        //Push the accumulated command buffers for this thread right now
                        Sinus.SinusManager.PushCommandBuffer();
                        tmpCtrl = true;
                    }

                    if (!s.IsRunning) s.Start();
                    Window_RenderFrame(0);
                    lock (Sinus.SinusManager.CommandBuffer)
                    {
                        while (Sinus.SinusManager.CommandBuffer.Count > 0)
                        {
                            Sinus.SinusManager.CommandBuffer.Dequeue()();   //Invoke the next function
                            //TODO Drop everything if the renderer starts falling behind
                        }
                    }
                    Sinus.SinusManager.PushCommandBuffer(); //Push this scene's command buffer
                }

                Kokoro.Debug.ObjectAllocTracker.PostFPS(GetNormTicks(s));
                //ViewportControl.Invalidate();
                s.Reset();
                s.Start();
            };
            #endregion

            var tmp = Initialize;
            Initialize = (GraphicsContext c) =>
            {
                Debug.ErrorLogger.StartLogger(true);
                Debug.ErrorLogger.AddMessage(0, "Engine Started", Debug.DebugType.Marker, Debug.Severity.Notification);

                ZNear = 0.1f;
                ZFar = 1000f;
                DepthWrite = true;
                Viewport = new Vector4(0, 0, WindowSize.X, WindowSize.Y);

            };
            Initialize += tmp;
            Initialize += (GraphicsContext c) =>
            {

                //Spawn threads for each: Update, Physics, Animation, Render
                UpdateThread.Start();
                PhysicsThread.Start();
                AnimationThread.Start();
                RenderThread.Start();
                ResourceManagerThread.Start();
            };
        }

        public void ForceDraw()
        {
            SubmitDraw();
            Draw();

            //TODO should we force the Viewport to start processing commands?
        }

        /// <summary>
        /// Swap the backbuffer and frontbuffer
        /// </summary>
        public void SwapBuffers()
        {
            ForceDraw();

            Sinus.SinusManager.QueueCommand(swap);

            //Push all data, this should 
            //Sinus.SinusManager.PushCommandBuffer();

            //Invalidate the winform paint and trigger the draw thread
            if (ViewportControl.IsHandleCreated)
            {
                ViewportControl.BeginInvoke(new MethodInvoker(() =>
                    {
                        ViewportControl.Invalidate();
                    }));
            }
        }

        private void GameLooper(double timestep, Action<double, GraphicsContext> handler)
        {
            Stopwatch s = Stopwatch.StartNew();
            //TODO: Implement skipping to avoid the spiral of death
            while (true)
            {
                if (handler != null)
                {
                    lock (handler)
                    {
                        handler((timestep == 0) ? GetNormTicks(s) : timestep, this);
                    }
                    //Each thread is required to push its commandbuffer when done
                    Sinus.SinusManager.PushCommandBuffer();
                }

                if (timestep != 0 && timestep > GetNormTicks(s))
                {
                    try
                    {
                        Thread.Sleep(TimeSpan.FromTicks((long)timestep - (long)GetNormTicks(s)));
                    }
                    catch (InvalidOperationException) { }
                    catch (ArgumentOutOfRangeException) { }
                }
                s.Reset();
                s.Start();
            }
        }

        private double GetNormTicks(Stopwatch s)
        {
            return (double)(s.ElapsedTicks * 1000 * 10000) / (Stopwatch.Frequency);
        }

        #endregion

        /// <summary>
        /// Create a new GraphicsContext
        /// </summary>
        /// <param name="WindowSize">The size of the Window</param>
        public GraphicsContext(Vector2 WindowSize)
            : base((int)WindowSize.X, (int)WindowSize.Y)
        {
            Debug.DebuggerManager.ShowDebugger();
            Debug.ObjectAllocTracker.NewCreated(this, 0, "GraphicsContext Created");
        }


    }
}
