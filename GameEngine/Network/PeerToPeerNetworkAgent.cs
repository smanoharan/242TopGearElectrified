using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using GameEngine.Physics;

namespace GameEngine.Network
{
    public class PeerToPeerNetworkAgent : NetworkAgent
    {
        protected Dictionary<byte, LinkedList<CarActor>> idTocarActorMap_;
        protected LocalNetworkGamer localGamer_;

        /// <summary>
        /// Init the Peer to peer Network agent. Note the restrictions on idToCarActorMap.
        /// </summary>
        /// <param name="idTocarActorMap">
        ///     This is a map with a Gamer's id as key, and an ordered linked list of the cars controlled by the gamer as the value.
        ///     The order must be the same on all gamers playing this game.
        /// </param>
        public PeerToPeerNetworkAgent(Game game, Dictionary<byte, LinkedList<CarActor>> idTocarActorMap, LocalNetworkGamer localGamer)
            : base(game)
        {
            this.idTocarActorMap_ = idTocarActorMap;
            this.localGamer_ = localGamer;
        }

        protected override void SendToNetwork(GameTime gameTime)
        {
            // write input to all players : 
            // Packets are sent inorder to make sure the world state never goes backwards in time
            // Packets can still get lost, however.
            PacketWriter writer = new PacketWriter();
            writer.Write(PacketHeader.INGAME_DATA);
            foreach (CarActor carActor in idTocarActorMap_[localGamer_.Id]) carActor.sendToNetwork(writer);
            localGamer_.SendData(writer, SendDataOptions.InOrder);
        }

        protected override void UpdateFromNetwork(GameTime gameTime)
        {
            // read any available data and process it:
            while (localGamer_.IsDataAvailable)
            {
                PacketReader reader = new PacketReader();
                NetworkGamer sender;
                localGamer_.ReceiveData(reader, out sender);

                // only process remote data (ignore data sent from the local player):
                if (localGamer_.Id == sender.Id) continue;

                // also ignore data from wrong packet headers:
                if (reader.ReadByte() != PacketHeader.INGAME_DATA) continue;

                foreach (CarActor carActor in idTocarActorMap_[sender.Id]) carActor.updateFromNetwork(reader);
            }
        }
    }
}
