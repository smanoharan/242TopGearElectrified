using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics;
using Microsoft.Xna.Framework;
using GameEngine.Renderer;
using BEPUphysics.Entities;
using BEPUphysics.Vehicle;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace GameEngine.Physics.BepuPhysicsAdapter
{
    public class BepuCarActor : CarActor
    {
        public static float forwardSpeed = 40f;  

        protected AudioEmitter emitter_;
        public AudioEmitter Emitter { get { return emitter_; } }

        // car characteristics:
        //private float backwardSpeed = 13f;
        //private float forwardSpeed = 40f;
        Vehicle car;

        /// <summary>
        /// Create a car Actor which can interface with the BEPU physics engine
        /// </summary>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        public BepuCarActor(Game game, CarModelRenderer renderer, Matrix worldMatrix)
             : base(game, renderer)
        {
           
            emitter_ = new AudioEmitter();
            CarSoundEmitters.AudioEmitters.Add(emitter_);
           

            // create a pin (for keeping the car stuck to the track)
            Capsule pin = new Capsule(new Vector3(0, -0.7f, -2), 0.6f, 0.12f, 1);
            pin.Bounciness = 0f;
            pin.DynamicFriction = 0f;
            pin.StaticFriction = 0f;

            // create a body:
            CompoundBody body = new CompoundBody();
            body.AddBody(new Box(new Vector3(0, 0, 0), 2.5f, .75f, 4.5f, 60));
            body.AddBody(new Box(new Vector3(0, .75f / 2 + .3f / 2, .5f), 2.5f, .3f, 2f, 1));            
            body.AddBody(pin);
            body.CenterOfMassOffset = new Vector3(0, -.5f, 0);

            body.WorldTransform = worldMatrix;
            body.LinearVelocity = Vector3.Zero;
            body.AngularVelocity = Vector3.Zero;
            // construct the car:
            car = new Vehicle(body);

            // attach wheels:
            Matrix wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2);
            for (int i = 0; i < 4; i++)
            {
                Wheel wheel = new Wheel(
                                 new RaycastWheelShape(.375f, wheelGraphicRotation),
                                 new WheelSuspension(2000, 100f, Vector3.Down, .8f, 
                                     new Vector3(
                                         ( i <= 1 ) ? -1.1f : 1.1f, 
                                         0, 
                                         ( (i & 1) == 0 ) ? 1.8f : - 1.8f)),
                                 new WheelDrivingMotor(2.5f, 30000, 10000),
                                 new WheelBrake(1.5f, 2, .02f),
                                 new WheelSlidingFriction(4, 5));

                car.AddWheel(wheel);

                // adjust additional values
                wheel.Shape.FreezeWheelsWhileBraking = true;
                wheel.Suspension.SolverSettings.MaximumIterations = 2;
                wheel.Brake.SolverSettings.MaximumIterations = 2;
                wheel.SlidingFriction.SolverSettings.MaximumIterations = 2;
                wheel.DrivingMotor.SolverSettings.MaximumIterations = 2;

                
            }

            // add it to physics
            Engine.currentWorld.Space.Add(car);
        }

        public override float Acceleration
        {
            get
            {
                // get current target speed:
                float targetSpeed = this.car.Wheels[1].DrivingMotor.TargetSpeed;

                // divide by the max speed to get the acceleration:
                return (targetSpeed <= 0 ? 0 : targetSpeed / forwardSpeed);
            }
            set
            {
                float targetSpeed = value * (value <= 0 ? 0 : forwardSpeed);
                this.car.Wheels[1].DrivingMotor.TargetSpeed = targetSpeed;
                this.car.Wheels[3].DrivingMotor.TargetSpeed = targetSpeed;
            }
        }

        public override Matrix WorldTransform
        {
            get
            {
                return this.car.Body.WorldTransform;
            }
            set
            {
                this.car.Body.WorldTransform = value;
            }
        }

        public override Vector3 LinearVelocity
        {
            get
            {
                return this.car.Body.LinearVelocity;
            }
            set
            {
                this.car.Body.LinearVelocity = value;
            }
        }

        public override Vector3 AngularVelocity
        {
            get
            {
                return this.car.Body.AngularVelocity;
            }
            set
            {
                this.car.Body.AngularVelocity = value;
            }
        }

        public override List<Wheel> Wheels
        {
            get
            {
                return this.car.Wheels;
            }
        }

        private Matrix startPos;
        public override Matrix startingPosition 
        { 
            get { return startPos;  }
            set { startPos = value; }
        }

        public override void TeleportToStartingPosition()
        {
            this.WorldTransform = startPos;
            this.car.Body.LinearVelocity = Vector3.Zero;
            this.car.Body.AngularVelocity = Vector3.Zero;
            this.car.Body.AngularMomentum = Vector3.Zero;
            this.car.Body.LinearMomentum = Vector3.Zero;
            this.car.Body.TeleportTo(startPos.Translation);            
        } 
    }
}
