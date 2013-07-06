using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine.Network
{
    /// <summary>
    /// Network agent: Responsible for communicating with other network agents participating in the same game.
    /// </summary>
    public abstract class NetworkAgent : GameComponent
    {
        // assumption: The game world is linked in through the constructor, thus they do not need to passed in at update time.
        public NetworkAgent(Game game) : base(game) { }

        /// <summary>
        /// Send the current data out to the network. Whether this is broadcast to everyone or just sent to certain players 
        /// is decided by the implementation.
        /// </summary>
        protected abstract void SendToNetwork(GameTime gameTime);

        /// <summary>
        /// Read any available data from the network, and update world state.
        /// </summary>
        protected abstract void UpdateFromNetwork(GameTime gameTime);

        /// <summary>
        /// Do all update actions. Read first, then write
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            UpdateFromNetwork(gameTime);
            SendToNetwork(gameTime);

        }

    }
}
