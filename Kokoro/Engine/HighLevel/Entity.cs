using Kokoro.Math;
using Kokoro.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.HighLevel
{
    /// <summary>
    /// Represents an entity to the engine
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// The position of the body
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return State.Position;
            }
            set
            {
                State.Position = value;
            }
        }
        
        /// <summary>
        /// The renderable representation of the entity
        /// </summary>
        public Model Renderable;

        /// <summary>
        /// Enable/Disable rendering of the entity
        /// </summary>
        public bool Visible;

        /// <summary>
        /// The PhysicsState of the body
        /// </summary>
        public PhysicsState State;
    }
}
