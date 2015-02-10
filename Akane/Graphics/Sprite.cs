using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Akane.Graphics
{
    public struct FrameData
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    public class Sprite
    {
        public bool Visible { get; set; }
        public Vector2 Position { get; set; }
        string spriteImg;
        public Dictionary<string, FrameData> Frames { get; set; }
        public Dictionary<string, string[]> Animations { get; set; }
        public Vector2 Size;
        string curAnimation;
        int interval;
        int curFrame, curInterval;

        public Sprite(string animations)
        {
            Visible = true;
            LoadAnimations(animations);
        }

        public void SetAnimation(string name, int interval, int defaultFrame)
        {
            curAnimation = name;
            curFrame = defaultFrame;
            curInterval = 0;
            this.interval = interval;
        }

        public void Draw(AkaneManager manager)
        {
            if (curInterval == interval)
            {
                curInterval = 0;
                curFrame++;
            }
            curFrame %= Animations[curAnimation].Length;
            curInterval++;

            FrameData img = Frames[Animations[curAnimation][curFrame]];
            Size.X = img.Width;
            Size.Y = img.Height;

            //EngineCore.SetTexture(Textures.GetTexture(spriteImg), 0);
            //EngineCore.DrawRect(Position.X, Position.Y, img.X, img.Y, img.Width, img.Height);
        }

        private void LoadAnimations(string animationPath)
        {
            string curName = "";
            List<string> tmpList = new List<string>();
            Animations = new Dictionary<string, string[]>();
            using (XmlReader doc = XmlReader.Create(animationPath))
                while (doc.Read())
                    if (doc.IsStartElement())
                    {
                        switch (doc.Name)
                        {
                            case "Animations":
                                LoadSpriteSheet(Path.Combine(Path.GetDirectoryName(animationPath), doc["src"]));
                                break;
                            case "Animation":
                                Animations[doc["Name"]] = null;
                                if (curName != "") Animations[curName] = tmpList.ToArray();
                                curName = doc["Name"];
                                tmpList.Clear();
                                break;
                            case "Frame":
                                tmpList.Add(doc["Id"]);
                                break;
                        }
                    }

            Animations[curName] = tmpList.ToArray();
        }

        private void LoadSpriteSheet(string spritePath)
        {
            Frames = new Dictionary<string, FrameData>();
            using (XmlReader doc = XmlReader.Create(spritePath))
                while (doc.Read())
                    if (doc.IsStartElement())
                    {
                        switch (doc.Name)
                        {
                            case "TextureAtlas":
                                spriteImg = doc["imagePath"];
                                if (!Path.IsPathRooted(spriteImg)) spriteImg = Path.Combine(Path.GetDirectoryName(spritePath), spriteImg);
                                break;
                            case "sprite":
                                Frames[doc["n"]] = new FrameData()
                                {
                                    X = Convert.ToInt32(doc["x"]),
                                    Y = Convert.ToInt32(doc["y"]),
                                    Width = Convert.ToInt32(doc["w"]),
                                    Height = Convert.ToInt32(doc["h"])
                                };
                                break;
                        }
                    }



        }

        public Sprite()
        {
            Visible = false;
        }

    }
}