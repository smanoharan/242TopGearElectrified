using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GameEngine.Physics;
using GameEngine.Util;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Cameras
{
    public class CameraController : GameComponent
    {
        protected LinkedList<CameraState> cameraState_;

        protected AudioListener listener_;
        public AudioListener Listener { get { return listener_; } }


        protected CarActor carActor_;
        public CarActor owner { get { return carActor_; } }

        protected CarCamera camera_;

        public CameraController(Game game, CarActor carActor)
            : base(game)
        {
            this.carActor_ = carActor;
            cameraState_ = new LinkedList<CameraState>();
            listener_ = new AudioListener();
            SetupCamera();

            
        }


        public void SetupCamera()
        {
            camera_ = new CarCamera(20f, 0.02f, MathHelper.PiOver4 / 2, 1.33f, 0.1f, 1000f, new Vector3(0f, 10f, 10f), Vector3.Zero, Vector3.Up);
            cameraState_.AddLast(CameraState.TopDown);
            cameraState_.AddLast(CameraState.Dynamic);
            cameraState_.AddLast(CameraState.Reverse);
            cameraState_.AddLast(CameraState.Chase);
        }

        public CarCamera CurrentCamera { get { return camera_; } }

        public void CycleNextCam()
        {

            int current = (int)this.camera_.State;
            current++;
            if (current >= (int)CameraState.Count) current = 0;
            this.camera_.State = (CameraState)current;

        }

        public override void Update(GameTime gameTime)
        {
            listener_.Position = CurrentCamera.Position;
            // update the current camera to look at the car:           
            this.camera_.Update(this.carActor_);

            base.Update(gameTime);
        }

    }
}
