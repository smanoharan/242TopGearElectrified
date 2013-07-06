using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Net;
using GameEngine.Physics;

namespace GameEngine.Network
{
    /// <summary>
    /// 'Extend' the CarActor class to support sending the crucial information across the network.
    ///  Allows methods for sending current input, current positional state and both.
    ///  
    /// In all cases: <b>Order is Important</b>. The reading and writing order should be the same in corresponding methods.
    /// 
    /// This class has been fully unit tested.
    /// </summary>
    static class CarActorExtension
    {
        #region Dealing with the state
        public static void sendStateToNetwork(this CarActor carActor, PacketWriter writer)
        {
            // read values from carActor and send over network:
            // position, orientation and velocity
            writer.Write(carActor.WorldTransform);
            writer.Write(carActor.LinearVelocity);
            writer.Write(carActor.AngularVelocity);
            
        }
        public static void updateStateFromNetwork(this CarActor carActor, PacketReader reader)
        {
            // read values and apply to carActor:
            // position and orientation and velocity 
            carActor.WorldTransform = reader.ReadMatrix();
            carActor.LinearVelocity = reader.ReadVector3();
            carActor.AngularVelocity = reader.ReadVector3();
        }
        #endregion

        #region Dealing with the input
        public static void sendInputToNetwork(this CarActor carActor, PacketWriter writer)
        {
            // read values from carActor and send over network:
            // accleration
            writer.Write(carActor.Acceleration);
        }
        public static void updateInputFromNetwork(this CarActor carActor, PacketReader reader)
        {
            // read values and apply to carActor:
            // accleration
            carActor.Acceleration = reader.ReadSingle();
        }
        #endregion

        #region Dealing with the entire carActor
        public static void sendToNetwork(this CarActor carActor, PacketWriter writer)
        {
            // send input first, then state. ORDER is important!
            carActor.sendInputToNetwork(writer);
            carActor.sendStateToNetwork(writer);
        }
        public static void updateFromNetwork(this CarActor carActor, PacketReader reader)
        {
            // update input first, then state. ORDER is important!
            carActor.updateInputFromNetwork(reader);
            carActor.updateStateFromNetwork(reader);
        }
        #endregion

    }
}
