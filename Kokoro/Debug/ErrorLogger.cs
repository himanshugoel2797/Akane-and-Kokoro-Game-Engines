using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if OPENGL
#if PC
using deb = Kokoro.OpenGL.PC.Debug;
#endif
#endif

namespace Kokoro.Debug
{
    /// <summary>
    /// Message Type
    /// </summary>
    public enum DebugType
    {
        Error, Performance, Marker, Other, Compatibility
    }

    /// <summary>
    /// Message Severity
    /// </summary>
    public enum Severity
    {
        High, Medium, Low, Notification
    }

    /// <summary>
    /// Logs all messages
    /// </summary>
    public class ErrorLogger
    {
        /// <summary>
        /// Start logging
        /// </summary>
        /// <param name="showLoggerWindow">Show Logger Window?</param>
        public static void StartLogger(bool showLoggerWindow)
        {
            deb.EnableDebug();
            deb.RegisterCallback(Callback);
        }

        static string log = "";
        private static void Callback(string message)
        {
            log += message;
            log += "\n";
        }

        /// <summary>
        /// Add a message to the log
        /// </summary>
        /// <param name="id">The object ID</param>
        /// <param name="message">Any related message</param>
        /// <param name="type">The message type</param>
        /// <param name="severity">The message severity</param>
        public static void AddMessage(int id, string message, DebugType type, Severity severity)
        {
            deb.InsertDebugMessage(id, message, type, severity);
        }

    }
}
