using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameEngine.Physics;

namespace GameEngine.Cameras
{
    public class CarCamera : BaseCamera
    {
        float lerpFactor_;
        float offset_;

        public CarCamera(float offset, float lerpFactor, float FOV, float AspectRatio, float NearPlane, float FarPlane,
            Vector3 Position, Vector3 Target, Vector3 CameraUp)
            : base(FOV, AspectRatio,
                NearPlane, FarPlane, Position, Target, CameraUp)
        {
            lerpFactor_ = lerpFactor;
            offset_ = offset;
        }

        public virtual void Update(CarActor carActor)
        {
            // This method can be over-ridden in other classes, i.e. to keep an offset from the target for a chase camera.
            // This simple camera is fixed in place (but turns to track the target).
            // Perhaps this method should be made abstract?

            Vector3 carPos = carActor.WorldTransform.Translation;
            

            this.target_ = carPos;
            //this.target_ = Vector3.Lerp(target_, carPos, lerpFactor_);
            viewNeedsUpdate_ = true; // lazy eval >> don't actually update the view matrix until needed.

            //get behind the target:
            Vector3 rightOri = carActor.WorldTransform.Forward; // TODO: POSSIBLE ERROR SOURCE
            Vector3 vel = carActor.LinearVelocity + Vector3.UnitY * 3f;

                currentFov_ = Lerp(currentFov_, fov_ + (vel.Length() / (CarActor.ForwardSpeed * 2f)), 0.1f);

            switch (cameraState_)
            {
                case CameraState.Chase:
                    position_ = Vector3.Lerp(position_, carPos - (rightOri * offset_ * vel.Length() / 14) - (rightOri * offset_ * 3) + (Vector3.UnitY * offset_), lerpFactor_);
                    //this.currentFov_ = Math.Min(car.Body.Velocity.LengthSquared() / 1000 + this.fov_, MathHelper.Pi - 0.1f); 
                    //this.projectionNeedsUpdate_ = true;
                    break;
                case CameraState.Reverse:
                    position_ = Vector3.Lerp(position_, carPos + (rightOri * offset_ * vel.Length() / 7) + (rightOri * offset_ * 7) + (Vector3.UnitY * offset_), lerpFactor_);
                    break;
                case CameraState.Dynamic:
                    Vector3 velocity = vel;
                    velocity.Normalize();
                    velocity.Y = 0;
                    position_ = Vector3.Lerp(position_, carPos - (velocity * offset_ * vel.Length() / 16) - (velocity * offset_ * 4) + (Vector3.UnitY * offset_), lerpFactor_);
                    break;
                case CameraState.TopDown:
                    position_ = Vector3.Lerp(position_, new Vector3(0f, 200f, 30f), lerpFactor_);
                    break;
                case CameraState.Count:
                    throw new Exception("Logic error; Camera state set to count");
            }

            this.viewNeedsUpdate_ = true;
            this.projectionNeedsUpdate_ = true;
        }

        private float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
