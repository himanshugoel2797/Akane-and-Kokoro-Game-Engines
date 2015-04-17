using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Math;

namespace Kokoro.Physics
{
    /// <summary>
    /// Stores the PhysicsState of a body
    /// </summary>
    public struct PhysicsState
    {
        /// <summary>
        /// The collision body against which intersection testing is performed
        /// </summary>
        public ICollisionBody CollisionBody;
        /// <summary>
        /// The Mass (in kg) of the body
        /// </summary>
        public float Mass;
        /// <summary>
        /// Enable/Disable physics on this body
        /// </summary>
        public bool Enabled;
        /// <summary>
        /// The acceleration due to gravity this body experiences
        /// </summary>
        public float Gravity;

        /// <summary>
        /// The Position of this object
        /// </summary>
        public Vector3 Position;
        
    }
}
