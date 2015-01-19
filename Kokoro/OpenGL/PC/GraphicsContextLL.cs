using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace Kokoro.OpenGL.PC
{
    /// <summary>
    /// This class exposes low level OpenGL statemachine functions to the engine
    /// </summary>
    public class GraphicsContextLL
    {
        protected GameWindow Window;

        private GraphicsContextLL() { }     //Block this class from normal construction
        protected GraphicsContextLL(int windowWidth, int windowHeight)
        {
            Window = new GameWindow(windowWidth, windowHeight);

            //Depth Test is always enabled, it's a matter of what the depth function is

        }

        #region State Machine

        #region Depth Write
        bool depthWriteEnabled;
        protected void SetDepthWrite(bool enabled)
        {
            GL.DepthMask(enabled);
            depthWriteEnabled = enabled;
        }
        protected bool GetDepthWrite() { return depthWriteEnabled; }
        #endregion

        #region FillMode
        PolygonMode polyMode;
        protected void SetWireframe(bool mode)
        {
            if (mode)
            {
                polyMode = PolygonMode.Line;
            }
            else
            {
                polyMode = PolygonMode.Fill;
            }

            GL.PolygonMode(MaterialFace.FrontAndBack, polyMode);
        }
        protected bool GetWireframe() { return polyMode == PolygonMode.Line; }
        #endregion

        #region Multisampling

        #endregion

        #region Cull Face
        Engine.CullMode cullMode = Engine.CullMode.Off;
        protected void SetCullMode(Engine.CullMode cullMode)
        {
            if (cullMode != Engine.CullMode.Off) GL.CullFace(EnumConverters.ECullMode(cullMode));
            this.cullMode = cullMode;
        }
        protected Engine.CullMode GetCullMode() { return cullMode; }
        #endregion

        #region Depth Test
        Func<float, float, bool> depthFunc = (x, y) => true;
        protected void SetDepthFunc(Func<float, float, bool> func)
        {
            //x ? y
            //Test the delegate with inputs of 0 and 1 to determine the depth function specified
            bool resultA = func(0, 1);  //True = Less  False = Greater
            bool resultB = func(1, 1);  //True = Equal False = Not Equal
            bool resultC = func(1, 0);  //True = Greater False = Less

            //All True = Always
            //All False = Never
            DepthFunction dFunction = DepthFunction.Lequal;

            if (resultA && resultB && resultC) dFunction = DepthFunction.Always;
            else if (!resultA && !resultB && !resultC) dFunction = DepthFunction.Never;
            else if (resultA && !resultB && !resultC) dFunction = DepthFunction.Less;
            else if (!resultA && !resultB && resultC) dFunction = DepthFunction.Greater;
            else if (resultA && resultB && !resultC) dFunction = DepthFunction.Lequal;
            else if (!resultA && resultB && resultC) dFunction = DepthFunction.Gequal;
            else if (resultB) dFunction = DepthFunction.Equal;
            else if (!resultB) dFunction = DepthFunction.Notequal;

            GL.DepthFunc(dFunction);
            GL.Enable(EnableCap.DepthTest);
            depthFunc = func;
        }
        protected Func<float, float, bool> GetDepthFunc()
        {
            return depthFunc;
        }
        #endregion

        #region Transparency
        
        #endregion

        #endregion

    }
}
