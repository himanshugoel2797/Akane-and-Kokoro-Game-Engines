using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine
{
    /// <summary>
    /// Describes how often the object will be updated
    /// </summary>
    public enum DrawMode
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
        public DrawMode UpdateMode;
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
        public Texture PreConvolvedCubeMap;
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

        public static World Load(string path)
        {
            World world = new World();


            return world;
        }

        private World()
        {

        }

    }
}
