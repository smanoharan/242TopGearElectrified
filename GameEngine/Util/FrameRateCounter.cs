using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Util
{
    /// <summary>
    /// SOURCE: Shawn Hargreaves - http://blogs.msdn.com/b/shawnhar/archive/2007/06/08/displaying-the-framerate.aspx
    /// Modified for XNA 3.1 & Splitscreen counting.
    /// </summary>
    public class FrameRateCounter : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public FrameRateCounter(Game game)
            : base(game)
        {
            content = game.Content;
        }

        public void Load() { this.LoadContent(); }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteFont = content.Load<SpriteFont>("fpsCounterFont");

        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }


        public override void Draw(GameTime gameTime)
        {


            // only draw (and count fps) in the first frame (ignore all other frames):
            if (DisplayController.Display.CurrentView.ViewPort.X != 0) return;


            frameCounter++;
            string fps = string.Format("fps: {0}", frameRate);

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, fps, new Vector2(33, 33), Color.Black);
            spriteBatch.DrawString(spriteFont, fps, new Vector2(32, 32), Color.White);

            spriteBatch.End();

        }
    }
}
