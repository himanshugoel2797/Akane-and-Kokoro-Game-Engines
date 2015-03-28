#if OPENGL && PC

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK;
using OpenTK.Input;
using System.Windows.Forms;
using Kokoro.Sinus;

namespace Kokoro.OpenGL.PC
{
    struct MDIEntry
    {
        public uint count;
        public uint instanceCount;
        public uint first;
        public uint baseVertex;
        public uint baseInstance;
    }

    /// <summary>
    /// This class exposes low level OpenGL statemachine functions to the engine
    /// </summary>
    public class GraphicsContextLL
    {
        protected bool inited, tmpCtrl;
        private GLControl Window;
        public Control ViewportControl
        {
            get
            {
                return Window;
            }
        }

        private GraphicsContextLL() { }     //Block this class from normal construction
        protected GraphicsContextLL(int windowWidth, int windowHeight)
        {
            Window = new GLControl();
            Window.Size = new System.Drawing.Size(windowWidth, windowHeight);

            Window.Resize += Window_Resize;
            Window.Load += Window_Load;
        }

        void Window_Load(object sender, EventArgs e)
        {
            Window.GotFocus += ParentForm_GotFocus;
            Window.LostFocus += ParentForm_LostFocus;
            Window.ParentForm.Move += Window_Move;
            Window.ParentForm.ResizeBegin += ParentForm_ResizeBegin;
            Window.ParentForm.ResizeEnd += ParentForm_ResizeEnd;

            inited = true;
            SinusManager.QueueCommand(() =>
            {
                //Depth Test is always enabled, it's a matter of what the depth function is
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.LineSmooth);
                GL.LineWidth(4);
                MDIBuffer.Bind();   //This is the only place the draw indirect can be used so we only bind this once, it will never be replaced unless someone does something really wrong
            });

        }

        void Window_Resize(object sender, EventArgs e)
        {
            //TODO Implement Resize handler
            SetViewport(new Math.Vector4(0, 0, Window.ClientSize.Width, Window.ClientSize.Height));
            InitializeMSAA(0);
        }

        protected void Window_RenderFrame(double interval)
        {
            ErrorCode err = GL.GetError();
            if (err != ErrorCode.NoError) Kokoro.Debug.ErrorLogger.AddMessage(0, err.ToString(), Kokoro.Debug.DebugType.Error, Kokoro.Debug.Severity.High);

            SinusManager.QueueCommand(() => GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0));
#if DEBUG
            if (curRequestTexture != 0) Debug.DLbmp(curRequestTexture);
            curRequestTexture = 0;
            Kokoro.Debug.ErrorLogger.AddMessage(0, "End Render Frame", Kokoro.Debug.DebugType.Other, Kokoro.Debug.Severity.Notification);
            Kokoro.Debug.ObjectAllocTracker.MarkGameLoop(interval, (this as Engine.GraphicsContext));
#endif
        }
        protected void swap()
        {
            Window.SwapBuffers();
        }

#if DEBUG
        static int curRequestTexture = 0;
        internal static void RequestTexture(int id)
        {
            curRequestTexture = id;
        }
#endif

        #region Multidraw
        static object locker = new object();
        static GPUBufferLL MDIBuffer = new GPUBufferLL(Engine.UpdateMode.Dynamic, Engine.BufferUse.Indirect, 1024); //Allocate 1kb to the indirect buffer
        static List<MDIEntry> MDIEntries = new List<MDIEntry>();
        static int EntryCount = 0;
        static int EntryOffset = 0;

        internal static void Draw()
        {
            Engine.Model.staticBuffer.Bind();   //Eventually this should be replaced by one call for static buffers and one for dynamic buffers

            SinusManager.QueueCommand(() =>
            {
                GL.MultiDrawElementsIndirect(All.Triangles, All.UnsignedInt, IntPtr.Zero, EntryCount, 0);
                MDIBuffer.PostFence();      //The MDIBuffer can not be modified until this is done
                Kokoro.Engine.Model.staticBuffer.PostFence();   //The draw buffers may not be modified until they have been drawn
                Kokoro.Engine.Model.dynamicBuffer.PostFence();  
            });
        }

        //Submit all the current draw calls and clear the draw list
        internal static void SubmitDraw()
        {
            lock (locker)
            {
                //Build the array to submit to the GPU
                uint[] buf = new uint[MDIEntries.Count * 5];
                for (int i = 0; i < MDIEntries.Count; i++)
                {
                    buf[i * 5] = MDIEntries[i].count;
                    buf[i * 5 + 1] = MDIEntries[i].instanceCount;
                    buf[i * 5 + 2] = MDIEntries[i].first;
                    buf[i * 5 + 3] = MDIEntries[i].baseVertex;
                    buf[i * 5 + 4] = MDIEntries[i].baseInstance;
                }
                EntryCount = MDIEntries.Count;
                MDIEntries.Clear();

                if (buf.Length > 0)
                {
                    MDIBuffer.BufferData(buf, 0, buf.Length);
                }
            }
        }

        internal static void AddDrawCall(uint first, uint count, uint baseVertex)
        {
            lock (locker)
            {
                //Append the draw call data to the MDIBuffer
                MDIEntries.Add(new MDIEntry()
                {
                    baseInstance = 0,
                    baseVertex = baseVertex,
                    count = count,
                    first = first,
                    instanceCount = 1
                });
            }
        }
        #endregion

        #region Input Focus Handlers
        void ParentForm_ResizeEnd(object sender, EventArgs e)
        {
            InputLL.IsFocused(Window.Focused);
        }

        void ParentForm_ResizeBegin(object sender, EventArgs e)
        {
            InputLL.IsFocused(Window.Focused);
        }

        void ParentForm_LostFocus(object sender, EventArgs e)
        {
            InputLL.IsFocused(false);
        }

        void ParentForm_GotFocus(object sender, EventArgs e)
        {
            InputLL.IsFocused(true);
        }

        void Window_Move(object sender, EventArgs e)
        {
            InputLL.SetWinXY(Window.ParentForm.DesktopLocation.X, Window.ParentForm.DesktopLocation.Y, Window.ParentForm.ClientSize.Width, Window.ParentForm.ClientSize.Height);
        }

        #endregion

        #region State Machine
        protected void aClear(float r, float g, float b, float a)
        {
            SinusManager.QueueCommand(() =>
            {
                //TODO maybe it'll be faster to just disable depth testing and draw a fsq? This is currently one of the slowest parts of the engine
                GL.ClearColor(r, g, b, a);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            });
        }

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

            SinusManager.QueueCommand(() => GL.PolygonMode(MaterialFace.FrontAndBack, polyMode));
        }
        protected bool GetWireframe() { return polyMode == PolygonMode.Line; }
        #endregion

        #region Multisampling
        int msaaTexID, fbufID, msaaLevel;
        protected void InitializeMSAA(int sampleCount)
        {
            return; //TODO Fixme
            SinusManager.QueueCommand(() =>
            {
                msaaTexID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2DMultisample, msaaTexID);
                GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, sampleCount, PixelInternalFormat.Rgba8, Window.ClientSize.Width, Window.ClientSize.Height, false);

                fbufID = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbufID);
                GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, msaaTexID, 0);
                msaaLevel = sampleCount;
            });
        }

        protected int GetMSAALevel() { return msaaLevel; }

        protected void SetMSAA()
        {
            SinusManager.QueueCommand(() =>
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbufID);
                GL.DrawBuffers(1, new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 });
            });
        }

        protected void BlitMSAA()
        {
            SinusManager.QueueCommand(() =>
            {
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, fbufID);
                GL.DrawBuffer(DrawBufferMode.Back);
                GL.BlitFramebuffer(0, 0, Window.ClientSize.Width, Window.ClientSize.Height, 0, 0, Window.ClientSize.Width, Window.ClientSize.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            });
        }

        protected void ResetMSAA()
        {
            SinusManager.QueueCommand(() =>
            {
                if (fbufID != 0) GL.DeleteFramebuffer(fbufID);
                if (msaaTexID != 0) GL.DeleteTexture(msaaTexID);
            });
            fbufID = 0;
            msaaTexID = 0;
        }
        #endregion

        #region Cull Face
        Engine.CullMode cullMode = Engine.CullMode.Off;
        protected void SetCullMode(Engine.CullMode cullMode)
        {
            if (cullMode != Engine.CullMode.Off)
            {
                SinusManager.QueueCommand(() =>
                {
                    GL.Enable(EnableCap.CullFace);
                    GL.CullFace(EnumConverters.ECullMode(cullMode));
                });
            }
            else
            {
                SinusManager.QueueCommand(() => GL.Disable(EnableCap.CullFace));
            }
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

            SinusManager.QueueCommand(() =>
            {
                GL.DepthFunc(dFunction);
                GL.Enable(EnableCap.DepthTest);
            });
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
            //GL.DepthRange(ZNear, ZFar);
        }
        protected float GetZNear() { return ZNear; }

        protected void SetZFar(float val)
        {
            ZFar = val;
            //GL.DepthRange(ZNear, ZFar);
        }
        protected float GetZFar() { return ZFar; }
        #endregion

        #region Viewport
        Kokoro.Math.Vector4 Viewport;
        protected void SetViewport(Kokoro.Math.Vector4 viewport)
        {
            Viewport = viewport;
            SinusManager.QueueCommand(() => GL.Viewport((int)viewport.X, (int)viewport.Y, (int)viewport.Z, (int)viewport.W));
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
            SinusManager.QueueCommand(() => GL.BlendFunc(EnumConverters.EBlendFuncSRC(blFunc.Src), EnumConverters.EBlendFuncDST(blFunc.Dst)));
        }
        #endregion

        #region Window Size
        protected Kokoro.Math.Vector2 GetWinSize()
        {
            return new Kokoro.Math.Vector2(Window.ClientSize.Width, Window.ClientSize.Height);
        }
        protected void SetWinSize(Kokoro.Math.Vector2 vec)
        {
            Window.ClientSize = new System.Drawing.Size((int)vec.X, (int)vec.Y);
        }
        #endregion

        #endregion
    }
}

#endif