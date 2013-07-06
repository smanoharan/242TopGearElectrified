using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Physics;
using BEPUphysics.Vehicle;

namespace GameEngine.Renderer
{
    /// <summary>
    /// The renderer for the car model which came packaged with the JigLibX library.
    /// This renderer supports multiple textures for the car.
    ///     5 colour schemes are provided : white (default), blue, green, red and yellow
    /// </summary>
    class DefaultCarModelRenderer : CarModelRenderer
    {
        // texture colour schemes
        public enum TextureColourScheme { White, Blue, Green, Red, Yellow, Count }
        TextureColourScheme colour_;
        Texture2D texture_;

        Game game;

        // members for drawing:
        //protected Vector3 scale_; // scale defaulted 1
        protected Model wheelModelLeft;
        protected Model wheelModelRight;
        protected Matrix[] wheelModelTransformsLeft;
        protected Matrix[] wheelModelTransformsRight;
        protected Model carModel_;
        protected Matrix[] carModelTransforms_;
        


        public DefaultCarModelRenderer(Game game, TextureColourScheme colour)
        {
            this.game = game;
            this.colour_ = colour;
            //this.scale_ = scale
        }

        public override void DrawCar(CarActor owner)
        {
            
            // draw the wheels
            for (int i = 0; i < 2; i++) DrawWheel(owner.Wheels[i], wheelModelLeft, wheelModelTransformsLeft);
            for (int i = 2; i < 4; i++) DrawWheel(owner.Wheels[i], wheelModelRight, wheelModelTransformsRight);
            
            // draw the car itself.
            foreach (ModelMesh mesh in carModel_.Meshes)
            {
                DrawModelMesh(mesh, carModelTransforms_[mesh.ParentBone.Index] * owner.WorldTransform, true);
            }
        }

        private void DrawWheel(Wheel wheel, Model model, Matrix[] wheelModelTransforms_)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                DrawModelMesh(mesh, wheelModelTransforms_[mesh.ParentBone.Index] * wheel.Shape.WorldTransform, false);
            }
        }

        public override void LoadModels()
        {
            // load the car model and the wheel models
            carModel_ = game.Content.Load<Model>("defaultCar");
            texture_ = game.Content.Load<Texture2D>("Car" + colour_.ToString());
            wheelModelLeft = game.Content.Load<Model>("defaultCarWheel");
            wheelModelRight = game.Content.Load<Model>("defaultCarWheelReflected");

            // setup the transforms for each model
            carModelTransforms_ = new Matrix[carModel_.Bones.Count];
            carModel_.CopyAbsoluteBoneTransformsTo(carModelTransforms_);

            // apply corrective transform:
            for (int i = 0; i < carModel_.Bones.Count; i++)
            {
                carModelTransforms_[i] *= Matrix.CreateTranslation(0f, -0.5f, 0f);
            }

            wheelModelTransformsLeft = new Matrix[wheelModelLeft.Bones.Count];
            wheelModelLeft.CopyAbsoluteBoneTransformsTo(wheelModelTransformsLeft);

            wheelModelTransformsRight = new Matrix[wheelModelRight.Bones.Count];
            wheelModelRight.CopyAbsoluteBoneTransformsTo(wheelModelTransformsRight);
        }

        public override void ApplyCustomEffects(BasicEffect effect)
        {
            // apply the texture:
            effect.Texture = this.texture_;
            effect.SpecularPower = 32;
            effect.SpecularColor = Color.White.ToVector3();
        }

    }
}
