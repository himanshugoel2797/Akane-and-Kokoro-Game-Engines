using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;

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

        public Matrix4 Projection { get; set; }
        public Matrix4 View { get; set; }

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
        /// Render Event
        /// </summary>
        public Action<long, GraphicsContext> Render
        {
            get
            {
                return base.render;
            }
            set
            {
                base.render = value;
            }
        }

        /// <summary>
        /// Update Event
        /// </summary>
        public Action<long, GraphicsContext> Update
        {
            get
            {
                return base.update;
            }
            set
            {
                base.update = value;
            }
        }
        #endregion

        internal void SetMSAABuffer() { base.SetMSAA(); }

        #region Clear
        public void Clear(float r, float g, float b, float a)
        {
            base.aClear(r, g, b, a);
        }
        public void Clear(Vector4 col) { Clear(col.X, col.Y, col.Z, col.W); }
        #endregion

        public GraphicsContext(Vector2 WindowSize)
            : base((int)WindowSize.X, (int)WindowSize.Y)
        {
            Debug.ErrorLogger.StartLogger(true);
            Debug.ObjectAllocTracker.NewCreated(this, -1, "GraphicsContext Created");
        }

        /// <summary>
        /// Start the game loop
        /// </summary>
        /// <param name="fps">Render calls per second</param>
        /// <param name="ups">Updates per second</param>
        public void Start(int fps, int ups)
        {
            Debug.ErrorLogger.AddMessage(0, "Engine Started", Debug.DebugType.Marker, Debug.Severity.Notification);
            base.aStart(fps, ups);
        }


    }
}
