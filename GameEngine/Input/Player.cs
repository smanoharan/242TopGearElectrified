using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using GameEngine.Physics;
using Microsoft.Xna.Framework;
using GameEngine.Cameras;

namespace GameEngine.Input
{
    public class Player : GameComponent
    {
        private CarActor carActor;
        private KeyConfig keyConfig;
        private KeyboardState currentKbs;
        private KeyboardState lastKbs;
        private CameraController cameraController;

        


        public Player(Game game, CarActor carActor, KeyConfig keyboardConfiguration, CameraController cameraController)
            : base(game)
        {
            this.carActor = carActor;
            this.keyConfig = keyboardConfiguration;
            this.cameraController = cameraController;
        }

        public override void Update(GameTime gameTime)
        {
            // get current keyState:
            currentKbs = Keyboard.GetState();

            // process input:
            ProcessInput();

            // update last keyState:
            lastKbs = currentKbs;

            base.Update(gameTime);
        }

        /// <summary>
        /// Handle input, assuming the keyboard states have been set correctly.
        /// This method can be overridden by subclasses which, for example, can provide a more smooth acceleration.
        /// </summary>
        protected virtual void ProcessInput()
        {
            // acceleration:
            float acc = 0;
            if (currentKbs.IsKeyDown(keyConfig.PositiveAccelerationKey)) acc = 1f;
            else if (currentKbs.IsKeyDown(keyConfig.NegativeAccelerationKey)) acc = -1f;

            this.carActor.Acceleration = acc;

            // steering:
            float steer = 0;
            if (currentKbs.IsKeyDown(keyConfig.LeftSteerKey)) steer = -MathHelper.PiOver4 / 2;
            else if (currentKbs.IsKeyDown(keyConfig.RightSteerKey)) steer = MathHelper.PiOver4 / 2;

            this.carActor.Wheels[1].Shape.SteeringAngle = steer;
            this.carActor.Wheels[3].Shape.SteeringAngle = steer;

            //if (isKeyPressed(Keys.Left)) carActor.WorldTransform *= Matrix.CreateTranslation(0.1f, 0f, 0f);

            
            // cycle cameras 
            if (isKeyPressed(keyConfig.CycleCameraKey)) this.cameraController.CycleNextCam();

            // reset to start position
            if (isKeyPressed(keyConfig.ResetToStartKey)) this.carActor.TeleportToStartingPosition();
        }

        /// <summary>
        /// Check if a key has <b>just</b> been pressed.
        ///     i.e. The key was not pressed last frame, but it is now pressed
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>True if key was pressed now, but not last frame, false if otherwise.</returns>
        public bool isKeyPressed(Keys key)
        {
            return (currentKbs.IsKeyDown(key) && lastKbs.IsKeyUp(key));
        }



    }
}
