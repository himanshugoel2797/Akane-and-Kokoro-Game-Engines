using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.OpenGL.PC
{
    /// <summary>
    /// Exposes Debugging related functions to the engine runtime
    /// </summary>
    public class Debug
    {
        public static void EnableDebug()
        {
            GL.Enable(EnableCap.DebugOutput);
        }

        public static void InsertDebugMessage(int id, string message, Kokoro.Debug.DebugType dt, Kokoro.Debug.Severity severity)
        {
            //DebugType converter
            DebugType type = DebugType.DebugTypeOther;
            if (dt == Kokoro.Debug.DebugType.Compatibility) type = DebugType.DebugTypePortability;
            else if (dt == Kokoro.Debug.DebugType.Error) type = DebugType.DebugTypeError;
            else if (dt == Kokoro.Debug.DebugType.Marker) type = DebugType.DebugTypeMarker;
            else if (dt == Kokoro.Debug.DebugType.Other) type = DebugType.DebugTypeOther;
            else if (dt == Kokoro.Debug.DebugType.Performance) type = DebugType.DebugTypePerformance;

            //DebugSeverity converter
            DebugSeverity severeness = DebugSeverity.DebugSeverityNotification;
            if (severity == Kokoro.Debug.Severity.High) severeness = DebugSeverity.DebugSeverityHigh;
            else if (severity == Kokoro.Debug.Severity.Low) severeness = DebugSeverity.DebugSeverityLow;
            else if (severity == Kokoro.Debug.Severity.Medium) severeness = DebugSeverity.DebugSeverityMedium;
            else if (severity == Kokoro.Debug.Severity.Notification) severeness = DebugSeverity.DebugSeverityNotification;

            GL.DebugMessageInsert(DebugSourceExternal.DebugSourceApplication, type, id, severeness, message.Length, message);
        }

        static Action<string> actionCallback;
        private static void DebugCallback(DebugSource src, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr usrData)
        {
            string msg = "";
            if (src == DebugSource.DebugSourceApi) msg += "[API]";
            else if (src == DebugSource.DebugSourceApplication) msg += "[Application]";
            else if (src == DebugSource.DebugSourceOther) msg += "[Other]";
            else if (src == DebugSource.DebugSourceShaderCompiler) msg += "[Shader Compiler]";
            else if (src == DebugSource.DebugSourceThirdParty) msg += "[Third Party]";
            else if (src == DebugSource.DebugSourceWindowSystem) msg += "[Window System]";

            if (type == DebugType.DebugTypeDeprecatedBehavior) msg += "[Deprecated]";
            else if (type == DebugType.DebugTypeError) msg += "[Error]";
            else if (type == DebugType.DebugTypeMarker) msg += "[Marker]";
            else if (type == DebugType.DebugTypeOther) msg += "[Other]";
            else if (type == DebugType.DebugTypePerformance) msg += "[Performance]";
            else if (type == DebugType.DebugTypePortability) msg += "[Portability]";
            else if (type == DebugType.DebugTypeUndefinedBehavior) msg += "[Undefined]";

            if (severity == DebugSeverity.DebugSeverityHigh) msg += "[High]";
            else if (severity == DebugSeverity.DebugSeverityLow) msg += "[Low]";
            else if (severity == DebugSeverity.DebugSeverityMedium) msg += "[Medium]";
            else if (severity == DebugSeverity.DebugSeverityNotification) msg += "[Notification]";

            msg += "[ID = " + id + "]";

            if(message != IntPtr.Zero)msg += System.Runtime.InteropServices.Marshal.PtrToStringAuto(message, length);

            actionCallback(msg);
        }
        public static void RegisterCallback(Action<string> callback)
        {
            actionCallback = callback;
            GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);
        }



    }
}
