using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Cameras;

namespace GameEngine.Util
{
    /// <summary>
    /// Runtime class for loading and rendering a textured skydome
    /// that was created during the build process by the SkyProcessor.
    /// </summary>
    public class SkyRenderer
    {
        #region Fields

        public Model Model;
        public Texture Texture;

        #endregion


        /// <summary>
        /// Helper for drawing the skydome mesh.
        /// </summary>
        public void Draw(Camera currentCamera)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["View"].SetValue(currentCamera.ViewMatrix);
                    effect.Parameters["Projection"].SetValue(currentCamera.ProjectionMatrix);
                    effect.Parameters["Texture"].SetValue(Texture);
                }
                mesh.Draw(SaveStateMode.SaveState);
            }
        }
    }

    public class SkyBoxActor : DrawableGameComponent
    {
        SkyRenderer renderer;

        public SkyBoxActor(Game game) : base(game) { }

        protected override void LoadContent()
        {
            renderer = Game.Content.Load<SkyRenderer>("skydome5"); // 3 & 5 
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            renderer.Draw(DisplayController.Display.CurrentView.Camera);
            base.Draw(gameTime);
        }
    }
}
