using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine.Cameras
{
    public interface Camera
    {
        /// <summary>
        /// The projection represents the volume visible to the camera
        /// and takes into account the field of view, near & far planes and aspect ratio
        /// </summary>
        Matrix ProjectionMatrix { get; } // note: no mutator (the set method) required.



        /// <summary>
        /// The view represents the part of the world visible to the camera
        /// and takes into account the position, target and the Up vector.
        /// </summary>
        Matrix ViewMatrix { get; } // note: no mutator required.


        /// <summary>
        /// Update the camera, with the target parameters. This method is called by the camera controller, usually once per draw/update.
        /// </summary>
        /// <param name="targetPosition">The current position of the target of this camera</param>
        void Update();


        /// <summary>
        /// Allow cameras to have their fovs and aspect ratios changed externally.
        /// </summary>
        /// <param name="FOV"></param>
        /// <param name="AspectRatio"></param>
        void SetViewParameters(float FOV, float AspectRatio);
    }

    public enum CameraState { Chase, Reverse, Dynamic, TopDown, Count }
}
