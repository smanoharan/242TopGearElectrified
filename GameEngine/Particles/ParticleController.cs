using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameEngine.Physics;
using GameEngine.Cameras;

namespace GameEngine.Particles
{
    public class ParticleController : DrawableGameComponent
    {
        List<ParticleSystem> particleSystems;
        ParticleEmitter fireEmitter;
        ParticleEmitter smokeEmitter;
        CarActor carActor;

        public ParticleController(Game game, CarActor carActor) : base(game)
        {
            this.carActor = carActor;

            particleSystems = new List<ParticleSystem>();
            particleSystems.Add(new ParticleSystem(game, game.Content, "Particles/FireSettings"));
            particleSystems.Add(new ParticleSystem(game, game.Content, "Particles/ProjectileTrailSettings"));

            fireEmitter = new ParticleEmitter(particleSystems[0], 10f, Vector3.Zero);
            smokeEmitter = new ParticleEmitter(particleSystems[1], 50f, Vector3.Zero);
        }

        public override void Initialize()
        {
            foreach (ParticleSystem p in particleSystems) p.Initialize();
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            Camera camera = DisplayController.Display.CurrentView.Camera;
            foreach (ParticleSystem p in particleSystems)
            {
                p.SetCamera(camera.ViewMatrix, camera.ProjectionMatrix);
                p.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            float particlesPerSecond = carActor.LinearVelocity.LengthSquared();
            Vector3 pos = carActor.WorldTransform.Translation;

            smokeEmitter.Update(gameTime, pos, particlesPerSecond / 2f);
            //fireEmitter.Update(gameTime, pos, particlesPerSecond / 5f);
            foreach (ParticleSystem p in particleSystems) p.Update(gameTime);
            base.Update(gameTime);
        }


        
    }
}
