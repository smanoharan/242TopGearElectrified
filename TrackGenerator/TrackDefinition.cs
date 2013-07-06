using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace TrackGenerator
{
    /// <summary>
    /// Purpose: To store information to pass between track importer and track processor
    /// </summary>
    public struct TrackDefinition
    {
        public List<Vector3> verts;
        public string filename;
    }
}
