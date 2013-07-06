using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics;
using Microsoft.Xna.Framework;

namespace GameEngine.Physics.BepuPhysicsAdapter
{
    public class BepuPhysicsSystem : PhysicsSystem
    {
        private Space space;
        public Space Space { get { return space; } }

        public BepuPhysicsSystem()
        {
            // create some empty space
            space = new Space();
            space.SimulationSettings.MotionUpdate.Gravity = new Vector3(0, -9.81f, 0);
        }

        public void Update(GameTime gameTime)
        {
            // update the physics
            space.Update(gameTime);
        }

    }
}
