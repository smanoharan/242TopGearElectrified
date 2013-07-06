using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace TrackGenerator
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "TrackGenerator.TrackProcessor")]
    public class TrackProcessor : ContentProcessor<TrackDefinition, ModelContent>
    {
        public override ModelContent Process(TrackDefinition input, ContentProcessorContext context)
        {
            int lanes = 3;
            float laneThickness = 0.9f;
            float interLaneSpace = 3f;
            float trackThickness = 1f;
            float laneWidth = 0.5f;

            MeshBuilder builder = MeshBuilder.StartMesh("terrain");
            // extend the key points to vertices
            List<Vector3> trackPoints = GenerateTrackPoints(input.verts);
            List<Vector3> trackVerts = GenerateTrackVertices(trackPoints, trackThickness, lanes, laneThickness, laneWidth, interLaneSpace);
            List<Matrix> startPositions = GenerateStartingPositions(trackPoints, lanes, laneWidth, interLaneSpace);
            
            // add vertices to mesh
            foreach (Vector3 v in trackVerts) builder.CreatePosition(v);

            // Create a material TODO:point it at our terrain texture.
            BasicMaterialContent material = new BasicMaterialContent();
            String texturePath = Path.Combine(Path.GetDirectoryName(input.filename),"tex.jpg");
            material.Texture = new ExternalReference<TextureContent>(texturePath);
            material.SpecularColor = Color.Black.ToVector3();
            material.SpecularPower = 0;
            builder.SetMaterial(material);

            int texCoordId = builder.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            int PART_OFFSET = 4 * (lanes + 1);
            

            // define some constants:
            int INT = 0;                    // near face, inner side, top       point
            int INB = 1;                    // near face, inner side, bottom    point
            int ONT = 2;                    // near face, outer side, top       point
            int ONB = 3;                    // near face, outer side, bottom    point
            int IFT = INT + PART_OFFSET;    // far  face, inner side, top       point
            int IFB = INB + PART_OFFSET;    // far  face, inner side, bottom    point
            int OFT = ONT + PART_OFFSET;    // far  face, outer side, top       point
            int OFB = ONB + PART_OFFSET;    // far  face, outer side, bottom    point

            // setup the triangles:
            for (int i = 0; i < trackVerts.Count - PART_OFFSET; i += PART_OFFSET)
            {
                // triangle mapping:
                /* 
                 * See picture for vertices order 
                 */

                //float dx, dz;
                
                //// for outer:
                //dx = trackVerts[i + 1].X - trackVerts[i + 5].X;
                //dz = trackVerts[i + 1].Z - trackVerts[i + 5].Z;
                //float vertDistOuter = (float)Math.Sqrt((dx * dx) + (dz * dz)) / halfTrackWidth;

                //// for inner:
                //dx = trackVerts[i + 0].X - trackVerts[i + 4].X;
                //dz = trackVerts[i + 0].Z - trackVerts[i + 4].Z;
                //float vertDistInner = (float)Math.Sqrt((dx * dx) + (dz * dz)) / halfTrackWidth;

                // side triangles:
                for (int j = 0; j <= lanes; j++) // less than or equal, since there are n lanes plus 1 for the entire track
                {
                    int offset = i + 4 * j; // i.e. each lane has 4 points, thus offset is 4 per lane.

                    // inner side
                    builder.AddTriangle(offset, INT, IFT, INB, texCoordId);
                    builder.AddTriangle(offset, INB, IFT, IFB, texCoordId);

                    // outer side
                    builder.AddTriangle(offset, ONT, ONB, OFT, texCoordId);
                    builder.AddTriangle(offset, ONB, OFB, OFT, texCoordId);
                }

                // bottom face:
                builder.AddTriangle(i, INB, OFB + PART_OFFSET - 4, ONB + PART_OFFSET - 4, texCoordId);
                builder.AddTriangle(i, IFB, OFB + PART_OFFSET - 4, INB, texCoordId);


                // Top faces
                bool laneGroove = false;
                for (int j = 0; j < PART_OFFSET - 2; j += 2)
                {
                    int offset = i + j;

                    if (laneGroove)
                    {
                        builder.AddTriangle(offset, INB, ONB, IFB, texCoordId);
                        builder.AddTriangle(offset, ONB, OFB, IFB, texCoordId);
                    }
                    else
                    {
                        builder.AddTriangle(offset, INT, ONT, IFT, texCoordId, 0.01f, 0.01f, 0.01f, 0.99f, 0.99f, 0.99f);
                        builder.AddTriangle(offset, ONT, OFT, IFT, texCoordId, 0.01f, 0.01f, 0.01f, 0.99f, 0.99f, 0.99f);
                    }

                    // alternate groove and non-groove
                    laneGroove = !laneGroove;
                }

            }

            MeshContent terrainMesh = builder.FinishMesh();
            ModelContent mc = context.Convert<MeshContent, ModelContent>(terrainMesh,"ModelProcessor");
            mc.Tag = startPositions;
            return mc;
        }


        

        private static Vector3 CR3D(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float amount)
        {
            Vector3 result = new Vector3();

            result.X = MathHelper.CatmullRom(v1.X, v2.X, v3.X, v4.X, amount);
            result.Y = MathHelper.CatmullRom(v1.Y, v2.Y, v3.Y, v4.Y, amount);
            result.Z = MathHelper.CatmullRom(v1.Z, v2.Z, v3.Z, v4.Z, amount);

            return result;
        }

        private static List<Vector3> InterpolateCR(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int detail)
        {
            List<Vector3> list = new List<Vector3>();

            for (int i = 0; i < detail; i++)
            {
                Vector3 newPoint = CR3D(v1, v2, v3, v4, (float)i / (float)detail);
                list.Add(newPoint);
            }
            return list;
        }

        private static List<Vector3> GenerateTrackPoints(List<Vector3> basePoints)
        {
            basePoints.Add(basePoints[0]);
            basePoints.Add(basePoints[1]);
            basePoints.Add(basePoints[2]);

            List<Vector3> allPoints = new List<Vector3>();

            for (int i = 1; i < basePoints.Count - 2; i++)
            {
                List<Vector3> part = InterpolateCR(basePoints[i - 1], basePoints[i], basePoints[i + 1], basePoints[i + 2], 20);
                allPoints.AddRange(part);
            }

            return allPoints;
        }
        
        private static List<Matrix> GenerateStartingPositions(List<Vector3> trackPoints, int lanes, float laneWidth, float interLaneSpace)
        {
            float halfTrackWidth = HalfTrackWidth(lanes, laneWidth, interLaneSpace);
            List<Matrix> startPos = new List<Matrix>();

            Vector3 carDir = trackPoints[2] - trackPoints[1];

            Vector3 forward = Vector3.Forward;
            float angle = Vector3.Dot(forward, carDir) / (forward.Length() * carDir.Length());
            angle = (float)Math.Acos(angle);
            
            Vector3 sideDir = Vector3.Cross(new Vector3(0, 1, 0), carDir);
            sideDir.Normalize();

            Vector3 innerTop = trackPoints[1] - sideDir * halfTrackWidth;

            for (int j = 0; j < lanes; j++)
            {
                // foreach lane:
                Vector3 laneInnerTop = innerTop + ((j * (interLaneSpace + laneWidth)) + interLaneSpace + (0.5f * laneWidth)) * (sideDir);
                startPos.Add(Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(laneInnerTop + Vector3.UnitY * 2f));
            }

            return startPos;
        }

        private static float HalfTrackWidth(int lanes, float laneWidth, float interLaneSpace)
        {
            return ((interLaneSpace * (lanes + 1)) + (laneWidth * lanes)) / 2;
        }

        private static List<Vector3> GenerateTrackVertices(List<Vector3> trackPoints, float TrackThickness, int lanes, float laneThickness, float laneWidth, float interLaneSpace)
        {

            float halfTrackWidth = HalfTrackWidth(lanes, laneWidth, interLaneSpace);
            List<Vector3> verticesList = new List<Vector3>();

            Vector3 currentNormal = Vector3.Up;
            for (int i = 1; i < trackPoints.Count - 1; i++)
            {

                Vector3 carDir = trackPoints[i + 1] - trackPoints[i];
                Vector3 sideDir = Vector3.Cross(new Vector3(0, 1, 0), carDir);
                sideDir.Normalize();

                Vector3 outerTop = trackPoints[i] + sideDir * halfTrackWidth;
                Vector3 innerTop = trackPoints[i] - sideDir * halfTrackWidth;


                //Vector3 next2 = trackPoints[i + 1] - trackPoints[i];
                //next2.Normalize();
                //Vector3 previous = trackPoints[i] - trackPoints[i - 1];
                //previous.Normalize();

                //Vector3 split = Vector3.Cross(next2, previous);
                //Vector3 mySide = Vector3.Cross(next2, split);

                //currentNormal = currentNormal + 0.2f * Vector3.Up + mySide * 2.0f;

                ////supportList.Add(new Vector3(trackPoints[i].X, -0.5f, trackPoints[i].Z));
                ////supportList.Add(trackPoints[i]);

                //Vector3 side = Vector3.Cross(currentNormal, next2);
                //side.Normalize();
                //currentNormal = Vector3.Cross(next2, side);
                
                // calculate the tracks inner points:
                //Vector3 innerTop = trackPoints[i] - side * halfTrackWidth;                
                Vector3 innerBottom = new Vector3(innerTop.X, innerTop.Y - TrackThickness, innerTop.Z);

                verticesList.Add(innerTop);
                verticesList.Add(innerBottom);

                for (int j = 0; j < lanes; j++)
                {
                    // foreach lane:
                    Vector3 laneInnerTop = innerTop + ((j * (interLaneSpace + laneWidth)) + interLaneSpace) * (sideDir);
                    Vector3 laneOuterTop = laneInnerTop + laneWidth * sideDir;

                    Vector3 laneOuterBottom = laneOuterTop - (Vector3.UnitY * laneThickness);
                    Vector3 laneInnerBottom = laneInnerTop - (Vector3.UnitY * laneThickness);

                    verticesList.Add(laneInnerTop);
                    verticesList.Add(laneInnerBottom);
                    verticesList.Add(laneOuterTop);
                    verticesList.Add(laneOuterBottom);
                }

                //distance += next2.Length();
                //Vector3 outerTop = trackPoints[i] + side * halfTrackWidth;
                Vector3 outerBottom = new Vector3(outerTop.X, outerTop.Y - TrackThickness, outerTop.Z);
                verticesList.Add(outerTop);
                verticesList.Add(outerBottom);
                
                //VertexPositionNormalTexture vertex;
                //vertex = new VertexPositionNormalTexture(innerPoint, currentNormal, new Vector2(0, distance / textureLenght));
                //verticesList.Add(vertex);
                //vertex = new VertexPositionNormalTexture(outerPoint, currentNormal, new Vector2(1, distance / textureLenght));
                //verticesList.Add(vertex);
            }
            for (int i=0;i<4 * (lanes + 1);i++) verticesList.Add(verticesList[i]);
            //supportList.Add(Vector3.Zero);
            //supportList.Add(Vector3.Zero);
            //supportVertices = XNAUtils.VerticesFromVector3List(supportList, Color.Yellow);

            //VertexPositionNormalTexture[] trackVertices = verticesList.ToArray();

            return verticesList;
        } 
    }

    public static class MeshBuilderExtension
    {
        public static void AddTriangle(this MeshBuilder builder, int offset, int v1, int v2, int v3, int texCoordId,
            float tu1, float tv1, float tu2, float tv2, float tu3, float tv3)
        {
            builder.AddTexturedTriangleVertex(texCoordId, offset + v1, new Vector2(tu1, tv1));
            builder.AddTexturedTriangleVertex(texCoordId, offset + v2, new Vector2(tu2, tv2));
            builder.AddTexturedTriangleVertex(texCoordId, offset + v3, new Vector2(tu3, tv3));
        }

        public static void AddTriangle(this MeshBuilder builder, int offset, int v1, int v2, int v3, int texCoordId)
        {
            builder.AddTriangle(offset, v1, v2, v3, texCoordId, 0, 0, 0, 0, 0, 0);
        }

        public static void AddTexturedTriangleVertex(this MeshBuilder builder, int texCoordId, int index, Vector2 texel)
        {
            builder.SetVertexChannelData(texCoordId, texel);
            builder.AddTriangleVertex(index);
        }
    }
}