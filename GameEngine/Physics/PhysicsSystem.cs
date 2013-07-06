using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics;

namespace GameEngine.Physics
{
    /// <summary>
    /// Handle all interaction with the underlying physics Engine
    /// </summary>
    public interface PhysicsSystem
    {
        Space Space { get; }

        void Update(GameTime gametime);


    }


}
