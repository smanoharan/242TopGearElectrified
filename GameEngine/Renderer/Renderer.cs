using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Renderer
{
    public abstract class IRenderer
    {
        public abstract void LoadModels();


        /// <summary>
        /// Apply any custom effects (such as specular lighting, custom texturing etc) to the 3D model
        ///     before rendering.
        /// </summary>
        /// <param name="effect">The effect which is used to draw the model</param>
        public abstract void ApplyCustomEffects(BasicEffect effect);


        /// <summary>
        /// This method draws a model mesh, with the given worldMatrix as viewed by the camera.
        /// </summary>
        /// <param name="mesh">The mesh to draw</param>
        /// <param name="worldMatrix">The matrix holding the position, rotation and scale of the mesh</param>
        /// <param name="camera">The camera which represents the user's current view</param>
        public void DrawModelMesh(ModelMesh mesh, Matrix worldMatrix, bool applyCustomEffects)
        {

            Cameras.Camera currentCamera = DisplayController.Display.CurrentView.Camera;
            // setup effect parameters for each effect
            foreach (BasicEffect effect in mesh.Effects)
            {
                // use default settings for now.
                effect.EnableDefaultLighting();
                effect.PreferPerPixelLighting = true;

                // the view, projection and world matrices must be setup for each effect, each frame. 

                effect.View = currentCamera.ViewMatrix;
                effect.Projection = currentCamera.ProjectionMatrix;
                effect.World = worldMatrix;

                // apply custom effects (if any):
                if (applyCustomEffects) this.ApplyCustomEffects(effect);

                // propagate changes. Any changes to parameters are not applied until CommitChanges() is called.
                effect.CommitChanges();
            }

            // actually draw the model, now that all effects have been setup:
            mesh.Draw();
        }
    }
}
