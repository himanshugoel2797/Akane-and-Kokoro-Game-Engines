using Kokoro.Engine;
using Kokoro.Engine.Prefabs;
using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace Akane.World
{
    public class Map
    {
        private TmxMap map;
        private Dictionary<string, Vector3> TileSets;   //X,Y = Offset in generated texture atlas, Z = texture atlas number
        private List<Texture> TextureAtlases;           //The Texture Atlases representing the textures on this map
        private List<Texture> TileLayers;               //The Tile Layers from bottom to top, each pixel on a texture stores information on the tile it represents
        private List<int> MaxGID_onLayer;
        private List<Vector3> TransparentColors;
        private Vector2 MapSize;
        private Vector2 TileSize;
        private Vector2 TilesCount;
        private Vector2 LocalPos;

        internal bool dirty = true;

        /*
         * Algorithm Description:
         * 
         *  Load tile information into TileLayers (using GPU to write to textures using multiple render targets?)
         *  Generate texture atlases for the ids and store the calculated offsets in the TileSets Dictionary
         *  Finally, render a high res quad which represents the number of tiles which should be shown on screen, pass the Tile Layers information to it along with the texture atlases, pick and draw the proper tile in the shader
         *  Store height data in TileLayers to be able to perform advanced lighting algorithms like ray tracing GI, render the information to an offscreen texture? second draw call shouldn't be very expensive either
         */

        public Map(string path, AkaneManager akane)
        {
            map = new TmxMap(path);

            TileSets = new Dictionary<string, Vector3>();
            TextureAtlases = new List<Texture>();
            TileLayers = new List<Texture>();
            MaxGID_onLayer = new List<int>();

            TileSize = new Vector2(map.TileWidth, map.TileHeight);
            TilesCount = new Vector2(map.Width, map.Height);
            MapSize = new Vector2(TileSize.X * TilesCount.X, TileSize.Y * TilesCount.Y);

            #region TileLayers
            //Backup the projection matrix
            Matrix4 projBackup = akane.context.Projection;

            //Each pixel is one tile
            FrameBuffer renderTarget = new FrameBuffer(map.Width, map.Height, PixelComponentType.RGBA16f, akane.context);
            Model tileRenderer = new FullScreenQuad();
            tileRenderer.Materials[0].Shader = new Kokoro.Engine.Shaders.ShaderProgram("Shaders/TileLayer");
            
            Matrix4 TileLayersOrthoMatrix = Matrix4.CreateOrthographicOffCenter(0, map.Width, -map.Height, 0, -1.0f, 1.0f);
            akane.context.Projection = TileLayersOrthoMatrix;

            renderTarget.Bind(akane.context);
            for(int i = 0; i < map.Layers.Count; i++)
            {
                int maxGid = 0;
                Vector2[,] tileInfo = GenerateTileIDArray(i, out maxGid);

                akane.context.Clear(0, 0, 0, 0);

                //Generate a tile layer texture for this layer
                for(int x = 0; x < map.Width; x++)
                {
                    for(int y = 0; y < map.Height; y++)
                    {
                        //Need a shader to generate this
                        tileRenderer.Materials[0].Shader["Height"] = (float)(i)/(float)(map.Layers.Count);
                        tileRenderer.Materials[0].Shader["tileID"] = tileInfo[x, y].X;
                        tileRenderer.Materials[0].Shader["maxGID"] = (float)maxGid;
                        tileRenderer.World = Matrix4.CreateTranslation(x, -y, 0.0f);
                        tileRenderer.Draw(akane.context);
                    }
                }

                //Store the maxGid so we can reconstruct information from the tile later
                MaxGID_onLayer.Add(maxGid);
                TileLayers.Add(renderTarget["Color"]);
                if(i < map.Layers.Count - 1)renderTarget.Add("Color", new FrameBufferTexture(map.Width, map.Height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment0, akane.context);    //Add a new texture to the framebuffer
            }

            //Restore the backed up matrix
            //akane.context.Projection = projBackup;
            #endregion

            quad = new FullScreenQuad();
            quad.Materials[0].Shader = new Kokoro.Engine.Shaders.ShaderProgram("Shaders/LayerDrawer");

            LoadResources(akane);
        }

        private Vector2[,] GenerateTileIDArray(int layer, out int maxGid)
        {
            Vector2[,] ids = new Vector2[map.Width, map.Height];
            maxGid = 0;

            for(int index = 0; index < map.Layers[layer].Tiles.Count; index++)
            {
                TmxLayerTile t = map.Layers[layer].Tiles[index];
                ids[t.X, t.Y] = new Vector2(t.Gid, map.Tilesets.IndexOf(GetTileSetForTile(t.Gid)));
                if (t.Gid > maxGid) maxGid = t.Gid;
            }

            return ids;
        }

        private TmxTileset GetTileSetForTile(int tileIndex)
        {
            var tiledTileset = map.Tilesets.Where(t => t.FirstGid <= tileIndex).OrderByDescending(t => t.FirstGid).FirstOrDefault();
            if (tiledTileset == null)
            {
                // Try to search the other way around. This should solve it...
                tiledTileset = map.Tilesets.Where(t => t.FirstGid >= tileIndex).OrderByDescending(t => t.FirstGid).FirstOrDefault();
                if (tiledTileset == null)
                {
                    // Still couldn't find the tileset.
                    string message = string.Format("Could not find tileset for tile #{0}.", tileIndex);
                    throw new InvalidOperationException(message);
                }
            }
            return tiledTileset;
        }

        //Load resources and apply their transparencies
        private void LoadResources(AkaneManager manager)
        {
            FrameBuffer tileSetTmpBuffer = new FrameBuffer((int)map.Tilesets[0].Image.Width, (int)map.Tilesets[0].Image.Height, PixelComponentType.RGBA8, manager.context);
            FullScreenQuad quad = new FullScreenQuad();
            quad.Materials[0].Shader = new Kokoro.Engine.Shaders.ShaderProgram("Shaders/TransparentColor");

            tileSetTmpBuffer.Bind(manager.context);

            for (int i = 0; i < map.Tilesets.Count; i++)
            {
                var t = map.Tilesets[i];

                if ( (t.Properties.ContainsKey("Visible") && t.Properties["Visible"] != "false") || !t.Properties.ContainsKey("Visible"))
                {
                    manager.context.Clear(0, 0, 0, 0);
                    quad.Materials[0].ColorMap = new Texture(t.Image.Source);
                    quad.Materials[0].Shader["TransparentColor"] = new Vector3(t.Image.Trans.R/255f, t.Image.Trans.G/255f, t.Image.Trans.B/255f);

                    quad.Draw(manager.context);

                    TextureAtlases.Add(tileSetTmpBuffer["Color"]);
                    if (i < map.Tilesets.Count - 1) tileSetTmpBuffer.Add("Color", 
                        new FrameBufferTexture((int)map.Tilesets[i + 1].Image.Width, (int)map.Tilesets[i + 1].Image.Height, PixelFormat.BGRA, PixelComponentType.RGBA8, PixelType.Float)
                        , FrameBufferAttachments.ColorAttachment0, manager.context);

                    //quad.Materials[0].ColorMap.Dispose();
                }
            }
        }

        FullScreenQuad quad;
        float y = 0, x = 0;
        public void Draw(AkaneManager manager)
        {
            for (int layer = 0; layer < TileLayers.Count; layer++)
            {
                quad.Materials[0].ColorMap = TextureAtlases[0];
                quad.Materials[0].Shader["layerNum"] = (float)layer;
                quad.Materials[0].Shader["TileIDs"] = TileLayers[layer];
                quad.Materials[0].Shader["maxGid"] = (float)MaxGID_onLayer[layer];
                quad.Materials[0].Shader["textureSize"] = TextureAtlases[0].Size;
                quad.Materials[0].Shader["mapSize"] = TilesCount;
                quad.Materials[0].Shader["tileSize"] = new Vector2(TileSize.X, TileSize.Y);
                quad.Materials[0].Shader["viewportTileRes"] = new Vector2(16, 9);
                quad.Materials[0].Shader["viewportOffset"] = new Vector2(x, TilesCount.Y * TileSize.Y - y);

                //x += 500;
                //x = x % (TilesCount.X * TileSize.X);
                y = 380;
                y = y % (TilesCount.Y * TileSize.Y);
                //y = TilesCount.Y * TileSize.Y;
                quad.Draw(manager.context);
            }
        }

        /*
         * TODO: Implement properites to control the various parameters, figure out a way to handle multiple textures, render map to texture at a fixed resolution before
         * rendering to screen so that we can get filtering without the artifacts, also need to add filtering option to Texture class
         * 
         * FIXME: Something's wrong with the X bounding, figure it out and fix it
         * 
         * Render one or more tiles extra, also set it to render out a heightmap of the data for lighting purposes
         * Next implement lighting system, study feasibility of 2D GI, bring over spritesheet class from older engine
         */

    }
}