using Akane.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akane;
using Kokoro.Math;
using Akane.Graphics;

namespace ImoutoPlatformer
{
    public class GameInitializer
    {
        AkaneManager manager;
        Map map;
        Sprite fumiko;
        CharacterController controller;

        public GameInitializer(){
            manager = new AkaneManager(new Kokoro.Math.Vector2(960,540));
            manager.Render = Render;
            manager.Update = Update;
            manager.Initialize = Initialize;
            manager.Start();
        }

        void Initialize(AkaneManager manager)
        {
            map = new Map("test.tmx", manager);

            fumiko = new Sprite("fumikoAnim.xml");
            fumiko.SetAnimation("standing", 1, 0);
            controller = new CharacterController(fumiko, map);
            controller.Speed = 1;
            controller.AnimationInterval = 10;

            manager.Projection = map.ProjectionMatrix;
            manager.AspectRatio = map.AspectRatio;

        }

        void Render(double time, AkaneManager manager)
        {
            map.Draw(manager);
            controller.Update(manager);
            //fumiko.Draw(manager);
        }

        void Update(double time, AkaneManager manager)
        {

        }

    }
}
