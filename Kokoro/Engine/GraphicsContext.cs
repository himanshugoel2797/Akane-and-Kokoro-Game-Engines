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
using Kokoro.OpenGL.PC;     //If building for OpenGL on PC (Windows, Linux, Mac)
#endif
#endif

namespace Kokoro.Engine
{
    public enum CullMode { Off, Back, Front }

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
        public void Clear(float r, float g, float b, float a)
        {
            base.aClear(r, g, b, a);
        }
        public void Clear(Vector4 col) { Clear(col.X, col.Y, col.Z, col.W); }
        #endregion

        #region Game Loop

        public Thread UpdateThread { get; private set; }
        public Thread PhysicsThread { get; private set; }
        public Thread AnimationThread { get; private set; }

        public Action<double, GraphicsContext> Update { get; set; }
        public Action<double, GraphicsContext> Render { get; set; }
        public Action<double, GraphicsContext> Animation { get; set; }
        public Action<double, GraphicsContext> Physics { get; set; }
        public Action<GraphicsContext> Initialize { get; set; }

        /// <summary>
        /// Start the game loop
        /// </summary>
        public void Start(int tpf, int tpu)
        {
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

                            Keyboard.Update();
                            Mouse.Update();
                            Update(GetNormTicks(su), this);
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

            PhysicsThread = new Thread(() =>
            {
                GameLooper(160000, Physics);
            });

            AnimationThread = new Thread(() =>
            {
                GameLooper(80000, Animation);
            });

            Stopwatch s = new Stopwatch();
            ViewportControl.Paint += (a, b) =>
            {
                if (inited)
                {
                    if (!s.IsRunning) s.Start();
                    Clear(1, 05f, 0, 0);
                    Window_RenderFrame(GetNormTicks(s));
                    lock (Render)
                    {
                        Render(GetNormTicks(s), this);
                    }
                    SwapBuffers();
                }

                if (tpf != 0 && tpf > GetNormTicks(s))
                {
                    Thread.Sleep(TimeSpan.FromTicks((long)tpf - (long)GetNormTicks(s)));
                    Debug.ErrorLogger.AddMessage(0, (GetNormTicks(s)).ToString(), Debug.DebugType.Performance, Debug.Severity.Notification);
                }
                Kokoro.Debug.ObjectAllocTracker.PostFPS(GetNormTicks(s)); //TODO setup some sort of smoothing, this gives us mostly crap data
                s.Reset();
                s.Start();
                ViewportControl.Invalidate();
            };

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

                //Spawn threads for each: Update, Physics, Animation; Render is on the current thread (control.paint)
                UpdateThread.Start();
                PhysicsThread.Start();
                AnimationThread.Start();
            };
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
                }

                if (timestep != 0 && timestep > GetNormTicks(s))
                {
                    try
                    {
                        Thread.Sleep(TimeSpan.FromTicks((long)timestep - (long)GetNormTicks(s)));
                    }
                    catch (InvalidOperationException) { }
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

        public GraphicsContext(Vector2 WindowSize)
            : base((int)WindowSize.X, (int)WindowSize.Y)
        {
            Debug.DebuggerManager.ShowDebugger();
            Debug.ObjectAllocTracker.NewCreated(this, 0, "GraphicsContext Created");
        }


    }
}
