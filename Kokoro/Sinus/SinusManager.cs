using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Sinus
{
    public class SinusManager
    {
        internal static Queue<Action> CommandBuffer = new Queue<Action>();

        [ThreadStatic]
        private static Queue<Action> LocalCommands = new Queue<Action>();

        static SinusManager()
        {
            LocalCommands = new Queue<Action>();
        }

        /// <summary>
        /// Queue a command for execution on the main thread
        /// </summary>
        /// <param name="command">The command to queue</param>
        /// <remarks>This should only be called from the Render thread, there may be undefined behavior if called from other threads</remarks>
        public static void QueueCommand(params Action[] command)
        {
            if (LocalCommands == null) LocalCommands = new Queue<Action>();

            for (int i = 0; i < command.Length; i++)
            {
                LocalCommands.Enqueue(command[i]);
            }

            //If the local command collection has gotten very large, push the commands to the main buffer, while logging a performance warning
            if (LocalCommands.Count >= 200) // NOTE : This constant can be changed as the minimum requirements increase
            {
                PushCommandBuffer();
                Kokoro.Debug.ErrorLogger.AddMessage(-1, "Too many local commands being issued without pushing to main buffer, pushing now", Debug.DebugType.Performance, Debug.Severity.High);
            }
        }

        internal static void PushCommandBuffer()
        {
            if (LocalCommands != null)  //Make sure LocalCommands has been initialized
            {
                if (LocalCommands.Count > 0)    //Only spend time on a lock if absolutely necessary
                {
                    lock (CommandBuffer)
                    {
                        //Enqueue all local buffer commands
                        LocalCommands.ToList().ForEach(i => CommandBuffer.Enqueue(i));
                    }
                    //Clear the local command buffer
                    LocalCommands.Clear();
                }
            }
            else
            {
                LocalCommands = new Queue<Action>();
            }
        }
    }
}
