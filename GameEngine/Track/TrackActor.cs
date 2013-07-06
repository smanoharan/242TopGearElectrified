using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Renderer;
using BEPUphysics;
using BEPUphysics.DataStructures;

namespace GameEngine.Track
{
    public class TrackActor : DrawableGameComponent
    {
        TrackRenderer renderer;
        string colHullPath;
        Matrix worldMatrix;
        List<Matrix> startingPos;
        public Matrix WorldMatrix { get { return worldMatrix; } }

        public TrackActor(Game game, string colHullPath, TrackRenderer renderer, Matrix worldMat)
            : base(game) 
        {
            this.renderer = renderer;
            this.colHullPath = colHullPath;
            this.worldMatrix = worldMat;
        }

        protected override void LoadContent()
        {
            // decompose collision hull into triangles 
            StaticTriangleGroup.StaticTriangleGroupVertex[] verts;
            int[] indices;
            
            Model m = Game.Content.Load<Model>(colHullPath);
            startingPos = (List<Matrix>)m.Tag;
            StaticTriangleGroup.GetVerticesAndIndicesFromModel(m, out verts, out indices);

            // convert to static mesh
            TriangleMesh triMesh = new TriangleMesh(verts, indices);
            StaticTriangleGroup triGroup = new StaticTriangleGroup(triMesh);
            //triGroup.WorldMatrix = this.worldMatrix;

            // add it to physics:
            Engine.currentWorld.Space.Add(triGroup);

            // invoke renderer loading:
            renderer.LoadModels();

            base.LoadContent();
        }

        public List<Matrix> getStartingPositions()
        {
            return startingPos;
        }

        public override void Draw(GameTime gameTime)
        {
            this.renderer.DrawTrack(this);
        }
    }
}
