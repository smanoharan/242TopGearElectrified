using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;


    class CarSoundEmitters
    {
        private static List<AudioEmitter> emitters_ = new List<AudioEmitter>();
        public static List<AudioEmitter> AudioEmitters { get { return emitters_; } set { emitters_ = value; } }
    }

