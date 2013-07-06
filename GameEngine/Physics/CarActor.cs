using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using GameEngine.Renderer;
using BEPUphysics.Vehicle;

namespace GameEngine.Physics
{
    /// <summary>
    /// Interface between a car Actor and the Physics Engine
    /// </summary>
    public abstract class CarActor : DrawableGameComponent
    {
        protected CarModelRenderer renderer;
        public abstract float Acceleration { get; set; }
        public abstract Matrix WorldTransform { get; set; }
        public abstract Vector3 LinearVelocity { get; set; }
        public abstract Vector3 AngularVelocity { get; set; }
        public abstract List<Wheel> Wheels { get; }
        public abstract Matrix startingPosition { get; set; }
        public static float ForwardSpeed = BepuPhysicsAdapter.BepuCarActor.forwardSpeed;

        public abstract void TeleportToStartingPosition();

        public CarActor(Game game, CarModelRenderer renderer) : base(game) 
        {
            this.renderer = renderer;
        }         

        public override void Draw(GameTime gameTime)
        {
            renderer.DrawCar(this);
        }

        protected override void LoadContent()
        {
            renderer.LoadModels();
        }

    }

    
}
