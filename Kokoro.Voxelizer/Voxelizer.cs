using Kokoro.Engine;
using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Voxelizer
{
    public class Voxelizer
    {
        GraphicsContext context;
        VoxelUI form;

        public Voxelizer()
        {

            form = new VoxelUI();
            context = new GraphicsContext(new Vector2(10, 10));
            context.ViewportControl.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(context.ViewportControl);
            form.ClientSize = new System.Drawing.Size(960, 540);
        }

        public void Voxelize()
        {
            VoxelRenderer renderer = new VoxelRenderer();
            context.Initialize += renderer.Initialize;
            context.Render += renderer.Render;
            context.Update += renderer.Update;
            context.Start(160000, 160000);
            form.ShowDialog();
        }

    }
}
