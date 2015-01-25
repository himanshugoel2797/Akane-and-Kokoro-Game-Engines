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
        protected GraphicsContextLL(int windowWidth, int windowHeight, bool fullscreen = false)
        {
            Window = new GameWindow(windowWidth, windowHeight);
            Window.RenderFrame += Window_RenderFrame;
            Window.UpdateFrame += Window_UpdateFrame;
            Window.Resize += Window_Resize;
            Window.Closing += (a, b) =>
            {
                Environment.Exit(0);
            };
            //Register the internal keyboard handlers
            Window.Keyboard.KeyDown += Keyboard_KeyDown;
            Window.Keyboard.KeyUp += Keyboard_KeyUp;
            Window.Mouse.ButtonDown += Mouse_ButtonDown;
            Window.Mouse.ButtonUp += Mouse_ButtonUp;

            //Depth Test is always enabled, it's a matter of what the depth function is
            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.Blend);

        }

        void Window_Resize(object sender, EventArgs e)
        {
            //TODO Implement Resize handler
            SetViewport(new Math.Vector4(0, 0, Window.ClientSize.Width, Window.ClientSize.Height));
            InitializeMSAA(0);
        }

        protected Action<double, Engine.GraphicsContext> update;
        void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
#if DEBUG
            Kokoro.Debug.ErrorLogger.AddMessage(0, "End Render Frame", Kokoro.Debug.DebugType.Other, Kokoro.Debug.Severity.Notification);
            Kokoro.Debug.ObjectAllocTracker.MarkGameLoop((long)e.Time, (this as Engine.GraphicsContext));
            Kokoro.Debug.ObjectAllocTracker.PostUPS(Window.UpdateFrequency);
            Kokoro.Debug.ObjectAllocTracker.PostFPS(Window.RenderFrequency);
            Window.Title = "Render : " + ((int)Window.RenderFrequency).ToString() + "  Update : " + ((int)Window.UpdateFrequency).ToString();

            ErrorCode err = GL.GetError();
            if (err != ErrorCode.NoError) Kokoro.Debug.ErrorLogger.AddMessage(0, err.ToString(), Kokoro.Debug.DebugType.Error, Kokoro.Debug.Severity.High);
#endif

            update(e.Time, (this as Engine.GraphicsContext));
        }

        protected Action<double, Engine.GraphicsContext> render;
        void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.ClearColor(0, 0, 1, 0);
            //SetMSAA();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            render(e.Time, (this as Engine.GraphicsContext));
            //BlitMSAA();
            Window.SwapBuffers();
#if DEBUG
            if (curRequestTexture != 0) Debug.DLbmp(curRequestTexture);
            curRequestTexture = 0;
#endif
        }

#if DEBUG
        static int curRequestTexture = 0;
        internal static void RequestTexture(int id)
        {
            curRequestTexture = id;
        }
#endif

        protected void aStart(int fps, int ups)
        {
            Window.Run(ups, fps);
        }

        protected void RegisterUpdateHandler(Action<double, Engine.GraphicsContext> act)
        {
            update += act;
        }

        protected void RegisterRenderHandler(Action<double, Engine.GraphicsContext> act)
        {
            render += act;
        }

        protected void aClear(float r, float g, float b, float a)
        {
            //TODO maybe it'll be faster to just disable depth testing and draw a fsq? This is currently one of the slowest parts of the engine
            GL.ClearColor(r, g, b, a);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        #region Keyboard
        protected string[] aKeys = new string[1];
        protected bool aMouseLeftButtonDown { get; private set; }
        protected bool aMouseRightButtonDown { get; private set; }
        public Math.Vector2 aMousePosition
        {
            get
            {
                return new Math.Vector2(Window.Mouse.X, Window.Mouse.Y);
            }
        }
        protected Math.Vector2 aMouseDelta
        {
            get
            {
                return new Math.Vector2(Window.Mouse.XDelta, Window.Mouse.YDelta);
            }
        }
        List<string> keysDown = new List<string>();
        bool alt, shift, control;
        void Keyboard_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            keysDown.Remove(e.Key.ToString());
            aKeys = keysDown.ToArray();
            alt = e.Alt;
            shift = e.Shift;
            control = e.Control;
        }
        void Mouse_ButtonDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            aMouseLeftButtonDown = (e.Button == OpenTK.Input.MouseButton.Left);
            aMouseRightButtonDown = (e.Button == OpenTK.Input.MouseButton.Right);
        }
        void Mouse_ButtonUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            aMouseLeftButtonDown = !(e.Button == OpenTK.Input.MouseButton.Left);
            aMouseRightButtonDown = !(e.Button == OpenTK.Input.MouseButton.Right);
        }
        void Keyboard_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (!keysDown.Contains(e.Key.ToString())) keysDown.Add(e.Key.ToString());
            aKeys = keysDown.ToArray();
            alt = e.Alt;
            shift = e.Shift;
            control = e.Control;
        }
        #endregion

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
        int msaaTexID, fbufID, msaaLevel;
        protected void InitializeMSAA(int sampleCount)
        {
            msaaTexID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DMultisample, msaaTexID);
            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, sampleCount, PixelInternalFormat.Rgba8, Window.ClientSize.Width, Window.ClientSize.Height, false);

            fbufID = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbufID);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, msaaTexID, 0);
            msaaLevel = sampleCount;
        }

        protected int GetMSAALevel() { return msaaLevel; }

        protected void SetMSAA()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbufID);
            GL.DrawBuffers(1, new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 });
        }

        protected void BlitMSAA()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, fbufID);
            GL.DrawBuffer(DrawBufferMode.Back);
            GL.BlitFramebuffer(0, 0, Window.ClientSize.Width, Window.ClientSize.Height, 0, 0, Window.ClientSize.Width, Window.ClientSize.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
        }

        protected void ResetMSAA()
        {
            if (fbufID != 0) GL.DeleteFramebuffer(fbufID);
            if (msaaTexID != 0) GL.DeleteTexture(msaaTexID);
            fbufID = 0;
            msaaTexID = 0;
        }
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

        #region ZNear and ZFar
        float ZNear, ZFar;
        protected void SetZNear(float val)
        {
            ZNear = val;
            GL.DepthRange(ZNear, ZFar);
        }
        protected float GetZNear() { return ZNear; }

        protected void SetZFar(float val)
        {
            ZFar = val;
            GL.DepthRange(ZNear, ZFar);
        }
        protected float GetZFar() { return ZFar; }
        #endregion

        #region Viewport
        Kokoro.Math.Vector4 Viewport;
        protected void SetViewport(Kokoro.Math.Vector4 viewport)
        {
            Viewport = viewport;
            GL.Viewport((int)viewport.X, (int)viewport.Y, (int)viewport.Z, (int)viewport.W);
        }
        protected Kokoro.Math.Vector4 GetViewport() { return Viewport; }
        #endregion

        #region Blending
        Engine.BlendFunc blFunc;
        protected Engine.BlendFunc GetBlendFunc()
        {
            return blFunc;
        }
        protected void SetBlendFunc(Engine.BlendFunc blend)
        {
            blFunc = blend;
            GL.BlendFunc(EnumConverters.EBlendFuncSRC(blFunc.Src), EnumConverters.EBlendFuncDST(blFunc.Dst));
        }
        #endregion

        #endregion
    }
}
