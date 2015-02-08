using Akane.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akane;
using Kokoro.Math;

namespace ImoutoPlatformer
{
    public class GameInitializer
    {
        AkaneManager manager;
        Map map;

        public GameInitializer(){
            manager = new AkaneManager(new Kokoro.Math.Vector2(960, 540));
            manager.Render = Render;
            manager.Update = Update;
            map = new Map("test.tmx", manager);

            manager.Start();
        }

        void Render(double time, AkaneManager manager)
        {
            map.Draw(manager);
        }

        void Update(double time, AkaneManager manager)
        {

        }

    }
}
