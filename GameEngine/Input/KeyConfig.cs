using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Input
{
    public class KeyConfig
    {

        #region members
        private Keys positiveAcc;
        private Keys negativeAcc;
        private Keys cameraCyle;
        private Keys leftSteer;
        private Keys rightSteer;
        private Keys resetToStart;
        #endregion
        
        /// <summary>
        /// The key which is pressed to accelerate the player in the forward direction
        /// </summary>
        public Keys PositiveAccelerationKey { get { return positiveAcc; } }

        /// <summary>
        /// The key which is pressed to accelerate the player in the backward direction
        /// </summary>
        public Keys NegativeAccelerationKey { get { return negativeAcc; } }

        /// <summary>
        /// The key which is pressed to cycle through the cameras.
        /// </summary>
        public Keys CycleCameraKey { get { return cameraCyle; } }

        // steering:
        public Keys LeftSteerKey { get { return leftSteer; } }
        public Keys RightSteerKey { get { return rightSteer; } }

        // reset to starting positoin
        public Keys ResetToStartKey { get { return resetToStart; } }

        public KeyConfig(Keys PositiveAcceleration, Keys NegativeAcceleration, Keys LeftSteer, Keys RightSteer, Keys CycleCamera, Keys ResetToStart)
        {
            this.positiveAcc = PositiveAcceleration;
            this.negativeAcc = NegativeAcceleration;
            this.leftSteer = LeftSteer;
            this.rightSteer = RightSteer;
            this.cameraCyle = CycleCamera;
            this.resetToStart = ResetToStart;
        }
    }
}
