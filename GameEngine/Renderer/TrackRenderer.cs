using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Track;

namespace GameEngine.Renderer
{
    public class TrackRenderer : IRenderer
    {
        string modelPath;
        Model model;
        Matrix[] transforms;
        Game game;

        public TrackRenderer(string modelPath, Game game)
        {
            this.modelPath = modelPath;
            this.game = game;
        }

        public override void LoadModels()
        {
            model = game.Content.Load<Model>(modelPath);
            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
        }

        public override void ApplyCustomEffects(BasicEffect effect)
        {
            // do nothing
        }

        public void DrawTrack(TrackActor owner)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                DrawModelMesh(mesh, this.transforms[mesh.ParentBone.Index] * owner.WorldMatrix, false);
            }
        }
    }
}
