using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kokoro.Engine
{
    /// <summary>
    /// Describes how often the object will be updated
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Never
        /// </summary>
        Static,
        /// <summary>
        /// Every Frame
        /// </summary>
        Dynamic
    }

    /// <summary>
    /// Describes all possible objects
    /// </summary>
    public enum ObjectType
    {
        /// <summary>
        /// A Particle System
        /// </summary>
        ParticleSystem,
        /// <summary>
        /// A non-renderable Physics object
        /// </summary>
        PhysicsObject,
        /// <summary>
        /// A renderable mesh, optionally with collision data
        /// </summary>
        Mesh,
        /// <summary>
        /// A light source
        /// </summary>
        LightSource,
        /// <summary>
        /// A sound source
        /// </summary>
        SoundSource
    }

    /// <summary>
    /// Stores information describing an object in the engine world
    /// </summary>
    public struct WorldItem
    {
        /// <summary>
        /// The Path to the associated file
        /// </summary>
        public string FilePath;
        /// <summary>
        /// Specifies whether the object data is updated often (Dynamic) or doesn't change (Static)
        /// </summary>
        public UpdateMode UpdateMode;
        /// <summary>
        /// Describes the object type
        /// </summary>
        public ObjectType ObjType;
        /// <summary>
        /// Contains the path to an associated script for the engine to load if necessary
        /// </summary>
        public string ScriptPath;

        /// <summary>
        /// Contains the object's relative position
        /// </summary>
        public Vector3 Position;
    }

    /// <summary>
    /// Defines the necessary information for a world cell
    /// </summary>
    public struct WorldCell
    {
        /// <summary>
        /// Describes the objects in this world section (indexes into World.AllItems)
        /// </summary>
        public int[] Items;

        /// <summary>
        /// Describes the Top texture for the grid cubemap
        /// </summary>
        public Texture Top;
        /// <summary>
        /// Describes the Bottom texture for the grid cubemap
        /// </summary>
        public Texture Bottom;
        /// <summary>
        /// Describes the Left textrue for the grid cubemap
        /// </summary>
        public Texture Left;
        /// <summary>
        /// Describess the Right texture for the grid cubemap
        /// </summary>
        public Texture Right;
        /// <summary>
        /// Describes the Front texture for the grid cubemap
        /// </summary>
        public Texture Front;
        /// <summary>
        /// Describes the Back texture for the grid cubemap
        /// </summary>
        public Texture Back;

        /// <summary>
        /// Describes the Preconvolved low resolution cubemap used for lighting
        /// </summary>
        public Texture ConvolvedCubeMap;

        /// <summary>
        /// Describes the Depth cubemap of the scene, for fast ray traced effects
        /// </summary>
        public Texture Depth;
    }

    public class World
    {
        /// <summary>
        /// A collection of all the items in the world
        /// </summary>
        public WorldItem[] AllItems;

        /// <summary>
        /// An octree of all the world cells
        /// </summary>
        public Octree<WorldCell> Cells;

        /// <summary>
        /// The Bounding Box for the entire world
        /// </summary>
        public Model.BoundingBox Bounds;

        /// <summary>
        /// Load a world from file
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <returns></returns>
        public static World Load(string path)
        {
            World world = new World();
            world.Cells = new Octree<WorldCell>();

            List<WorldItem> WorldItems = new List<WorldItem>();
            List<string[]> CubeMaps = new List<string[]>();

            Octree<WorldCell> CurrentCell = world.Cells;

            #region XML Parser
            //Parse the document and load all data
            using (XmlReader doc = XmlReader.Create(path))
            {
                while (doc.Read())
                {
                    if (doc.IsStartElement())
                    {
                        switch (doc.Name)
                        {
                            case "WorldItem":
                                WorldItems.Add(new WorldItem()
                                {
                                    FilePath = doc["FilePath"],
                                    ObjType = (ObjectType)Enum.Parse(typeof(ObjectType), doc["ObjectType"]),
                                    ScriptPath = doc["ScriptPath"],
                                    UpdateMode = (UpdateMode)Enum.Parse(typeof(UpdateMode), doc["UpdateMode"]),
                                    Position = new Vector3(doc["Position"])
                                });
                                break;
                            case "CubeMap":
                                CubeMaps.Add(new string[] { 
                                        doc["Top"],
                                        doc["Bottom"],
                                        doc["Left"],
                                        doc["Right"],
                                        doc["Front"],
                                        doc["Back"],
                                        doc["Convolved"],
                                        doc["Depth"]
                                    });
                                break;
                            case "WorldOctree":
                                world.Bounds = new Model.BoundingBox()
                                {
                                    Max = new Vector3(doc["Max"]),
                                    Min = new Vector3(doc["Min"])
                                };
                                break;
                            case "Child":
                                //Move down the node heirarchy
                                switch (doc["Name"])
                                {
                                    case "BottomForwardLeft":
                                        CurrentCell.BottomForwardLeft = new Octree<WorldCell>();
                                        CurrentCell.BottomForwardLeft.Parent = CurrentCell;
                                        CurrentCell = CurrentCell.BottomForwardLeft;
                                        break;
                                    case "BottomForwardRight":
                                        CurrentCell.BottomForwardRight = new Octree<WorldCell>();
                                        CurrentCell.BottomForwardRight.Parent = CurrentCell;
                                        CurrentCell = CurrentCell.BottomForwardRight;
                                        break;
                                    case "BottomBackLeft":
                                        CurrentCell.BottomBackLeft = new Octree<WorldCell>();
                                        CurrentCell.BottomBackLeft.Parent = CurrentCell;
                                        CurrentCell = CurrentCell.BottomBackLeft;
                                        break;
                                    case "BottomBackRight":
                                        CurrentCell.BottomBackRight = new Octree<WorldCell>();
                                        CurrentCell.BottomBackRight.Parent = CurrentCell;
                                        CurrentCell = CurrentCell.BottomBackRight;
                                        break;

                                    case "TopForwardLeft":
                                        CurrentCell.TopForwardLeft = new Octree<WorldCell>();
                                        CurrentCell.TopForwardLeft.Parent = CurrentCell;
                                        CurrentCell = CurrentCell.TopForwardLeft;
                                        break;
                                    case "TopForwardRight":
                                        CurrentCell.TopForwardRight = new Octree<WorldCell>();
                                        CurrentCell.TopForwardRight.Parent = CurrentCell;
                                        CurrentCell = CurrentCell.TopForwardRight;
                                        break;
                                    case "TopBackLeft":
                                        CurrentCell.TopBackLeft = new Octree<WorldCell>();
                                        CurrentCell.TopBackLeft.Parent = CurrentCell;
                                        CurrentCell = CurrentCell.TopBackLeft;
                                        break;
                                    case "TopBackRight":
                                        CurrentCell.TopBackRight = new Octree<WorldCell>();
                                        CurrentCell.TopBackRight.Parent = CurrentCell;
                                        CurrentCell = CurrentCell.TopBackRight;
                                        break;
                                }

                                string[] currentItems = doc["Items"].Split(',');
                                int cubeMapIndex = int.Parse(doc["CubeMapIndex"]);

                                CurrentCell.Information.Items = Array.ConvertAll(currentItems, s => int.Parse(s));
                                CurrentCell.Information.Top = new Texture(CubeMaps[cubeMapIndex][0], true);
                                CurrentCell.Information.Bottom = new Texture(CubeMaps[cubeMapIndex][1], true);
                                CurrentCell.Information.Left = new Texture(CubeMaps[cubeMapIndex][2], true);
                                CurrentCell.Information.Right = new Texture(CubeMaps[cubeMapIndex][3], true);
                                CurrentCell.Information.Front = new Texture(CubeMaps[cubeMapIndex][4], true);
                                CurrentCell.Information.Back = new Texture(CubeMaps[cubeMapIndex][5], true);
                                CurrentCell.Information.ConvolvedCubeMap = new Texture(CubeMaps[cubeMapIndex][6], true);
                                CurrentCell.Information.Depth = new Texture(CubeMaps[cubeMapIndex][7], true);

                                break;
                        }
                    }
                    else
                    {
                        //Move up the node heirarchy
                        if (doc.Name == "Child")
                        {
                            if (CurrentCell.Parent != null) CurrentCell = CurrentCell.Parent;
                        }
                    }



                }
            }
            #endregion
            //Set all the world items
            world.AllItems = WorldItems.ToArray();

            return world;
        }

        private World()
        {

        }

    }
}
