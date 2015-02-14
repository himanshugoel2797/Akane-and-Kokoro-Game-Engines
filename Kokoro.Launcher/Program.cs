using Kokoro.Engine;
using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Launcher
{
    class Program
    {
        static GraphicsContext context;
        static void Main(string[] args)
        {
            Form1 form = new Form1();
            context = new GraphicsContext(new Vector2(960, 540));
            context.ViewportControl.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(context.ViewportControl);
            form.ClientSize = new System.Drawing.Size((int)context.ViewportControl.Width, (int)context.ViewportControl.Height);

            Kokoro.Game.Game game = new Game.Game(context);
            form.ShowDialog();
            return;
        }
    }
}
