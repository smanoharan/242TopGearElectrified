using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace TrackGenerator
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".trk", DisplayName = "Track Importer", DefaultProcessor = "TrackProcessor")]
    public class TrackImporter : ContentImporter<TrackDefinition>
    {
        public override TrackDefinition Import(string filename, ContentImporterContext context)
        {

            List<Vector3> trackPoints = new List<Vector3>();
            using (StreamReader reader = new StreamReader(filename))
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                {

                    // split the line into 3 vector 3s:
                    String[] parts = line.Split(',');
                    trackPoints.Add(new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2])));

                }
            }

            TrackDefinition trackDef = new TrackDefinition();
            trackDef.filename = filename;
            trackDef.verts = trackPoints;
            return trackDef;
        }
    }
}
