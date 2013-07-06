using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine.Cameras
{
    public class BaseCamera : Camera
    {
        // camera properties:

        // FOR PROJECTION
        protected float fov_;
        protected float currentFov_;
        protected float aspectRatio_;
        protected float nearPlane_;
        protected float farPlane_;

        // FOR VIEW
        protected Vector3 position_;
        protected Vector3 target_;
        protected Vector3 cameraUp_;

        public Vector3 Position { get { return position_; } }

        // cached results. May need to be updated:
        protected bool projectionNeedsUpdate_;
        protected bool viewNeedsUpdate_;
        protected Matrix projMat_;
        protected Matrix viewMat_;

        //protected bool switching_;
        protected CameraState cameraState_;

        public BaseCamera(float FOV, float AspectRatio, float NearPlane, float FarPlane,
            Vector3 Position, Vector3 Target, Vector3 CameraUp)
        {
            fov_ = FOV;
            currentFov_ = fov_;
            aspectRatio_ = AspectRatio;
            nearPlane_ = NearPlane;
            farPlane_ = FarPlane;
            position_ = Position;
            target_ = Target;
            cameraUp_ = CameraUp;
            //switching_ = false;
            cameraState_ = CameraState.Chase;

            // calculate Projection and view Matricies.
            updateProjectionMatrix();
            updateViewMatrix();
        }

        // read only.
        public Matrix ProjectionMatrix
        {
            get
            {
                if (projectionNeedsUpdate_) updateProjectionMatrix();
                return projMat_;
            }
        }

        // read only.
        public Matrix ViewMatrix
        {
            get
            {
                if (viewNeedsUpdate_) updateViewMatrix();
                return viewMat_;
            }
        }

        public CameraState State
        {
            get { return cameraState_; }
            set { cameraState_ = value; }
        }

        private void updateProjectionMatrix()
        {
            projMat_ = Matrix.CreatePerspectiveFieldOfView((currentFov_ >= MathHelper.Pi ? MathHelper.Pi - 0.001f : currentFov_), aspectRatio_, nearPlane_, farPlane_);
            projectionNeedsUpdate_ = false;
        }

        private void updateViewMatrix()
        {
            viewMat_ = Matrix.CreateLookAt(position_, target_, cameraUp_);
            viewNeedsUpdate_ = false;
        }

        /// <summary>
        /// Update the camera to reflect the new target position.
        /// </summary>
        /// <param name="targetPosition">The position of the new target</param>
        public virtual void Update()
        {
            // This method can be over-riden in other classes, i.e. to keep an offset from the target for a chase camera.
            // This simple camera is fixed in place (but turns to track the target).
            // Perhaps this method should be made abstract?

            //this.target_ = car.Body.Position;
            viewNeedsUpdate_ = true; // lazy eval >> don't actually update the view matrix until needed.
        }

        public void SetViewParameters(float FOV, float AspectRatio)
        {
            this.fov_ = FOV;
            this.currentFov_ = this.fov_;
            this.aspectRatio_ = AspectRatio;
            this.viewNeedsUpdate_ = true;
            this.projectionNeedsUpdate_ = true;
            updateProjectionMatrix();
        }
    }
}
