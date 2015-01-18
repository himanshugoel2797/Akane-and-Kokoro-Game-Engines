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

        public Func<float, float, bool> DepthFunction
        {
            get
            {
                return base.GetDepthFunc();
            }
            set
            {

            }
        }
        #endregion

        public GraphicsContext(Vector2 WindowSize)
            : base((int)WindowSize.X, (int)WindowSize.Y)
        {

        }




    }
}
