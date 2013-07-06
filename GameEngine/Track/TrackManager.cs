using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameEngine.Renderer;

namespace GameEngine.Track
{
    /// <summary>
    /// Purpose: To load a track from content, Set it up, and 
    ///     also to generate the starting position of the cars
    ///     
    /// 
    /// </summary>
    public class TrackManager
    {
        const string TrackPath = "Tracks/track";
        public static TrackActor CreateTrack(Game game, int carCount)
        {

            return new TrackActor(game, TrackPath, new TrackRenderer(TrackPath, game), Matrix.Identity);

            //// populate the carStartingPositions with carCount items;
            //carStartingPositions = new List<Matrix>();
            //for (int i = 0; i < carCount; i++)
            //{
            //    carStartingPositions.Add(Matrix.CreateTranslation(new Vector3((-5f * i), 2f, (5f * 0))));
            //}

            //return track;
        }


    }
}
