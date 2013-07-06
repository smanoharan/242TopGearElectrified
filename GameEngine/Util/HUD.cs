using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Physics;


namespace GameEngine.Util
{
    public class HUD : Microsoft.Xna.Framework.DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        Texture2D hud;
        Texture2D needle;
        CarActor carActor;
        private SpriteFont spriteFont;
        float rotation;
        int xEdge;
        int yEdge;
        private int lapPosX;
        private int lapPosZ;
        private int oldLapPosX;
        private int oldLapPosZ;
        private int lapCount;
        private int counter;

        public HUD(Game game, CarActor car, int xEdge, int yEdge)
            : base(game)
        {
            spriteFont = Game.Content.Load<SpriteFont>("GameplayFont");
            counter = 0;
            lapCount = 0;
            lapPosX = 0;
            lapPosZ = 0;
            oldLapPosX = 0;
            oldLapPosZ = 0;
            content = game.Content;
            this.xEdge = xEdge;
            this.yEdge = yEdge;
            carActor = car;
        }

        public void Load() { this.LoadContent(); }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            hud = content.Load<Texture2D>("HUD/SpeedoMeter");
            needle = content.Load<Texture2D>("HUD/Needle");
        }

        public override void Update(GameTime gameTime)
        {
            counter++;
            if (counter > 100)
            {
                oldLapPosX = lapPosX;
                oldLapPosZ = lapPosZ;
                lapPosX = Math.Sign((carActor.startingPosition.Translation - carActor.WorldTransform.Translation).X);
                lapPosZ = Math.Sign((carActor.startingPosition.Translation - carActor.WorldTransform.Translation).Z);
                if (lapPosX == (oldLapPosX * -1) && lapPosZ == (oldLapPosZ * -1) && lapPosX == 1 && lapPosZ == 1)
                    lapCount++;
                counter = 0;
            }

            Vector3 velocity = carActor.LinearVelocity;
            velocity.Y = 0;
            float speed = velocity.Length() / CarActor.ForwardSpeed;
            rotation = MathHelper.Lerp(rotation, MathHelper.ToRadians(250f) * speed, 0.2f);
        }

        public override void Draw(GameTime gameTime)
        {
            float scale = 0.8f;    //times larger than image size
            Vector2 origin = new Vector2(199, 197);

            
            spriteBatch.Begin();

            Vector2 hudLocation = new Vector2( xEdge - (hud.Width * scale), yEdge - (hud.Height * scale));
            Vector2 needleLocation = new Vector2(xEdge - ((hud.Width - 200) * scale), yEdge - ((hud.Height - 200) * scale));
            Vector2 lapCountLocation = new Vector2(xEdge - ((hud.Width - 150) * scale), yEdge - ((hud.Height - 300) * scale));
           // Vector2 cameraState = new Vector2(xEdge - ((hud.Width - 150) * scale), yEdge - ((hud.Height - 340) * scale));
            spriteBatch.Draw(hud, hudLocation, null, Color.White, 0, new Vector2(), scale ,SpriteEffects.None, 0);
            spriteBatch.Draw(needle, needleLocation, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(spriteFont, "Lap " + lapCount.ToString(), lapCountLocation, Color.DarkBlue);

            spriteBatch.End();
            //Game.GraphicsDevice.RenderState.DepthBufferEnable = true;
        }
    }
}
