using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.SceneGraph
{
    public class BroadPhaseWorld
    {
        //Check all children narrow phase worlds for dynamic objects, only ones which have dynamic objects can have things which need simulating
        //Then check if any are marked dirty, which means a body in the world was modified
        //Check if any of the dynamic worlds have any objects which haven't settled down yet
        //If none of this happened, we only need to check worlds with dynamic objects which haven't settled down
        //For a narrow phase world, check all dynamic objects for OBV collisions and then resolve them
        //Call all related events, update all connected objects - update draw position when the associated physics body moves, raise collision events

        public BroadPhaseWorld(int worldSize)
        {

        }
    }
}
