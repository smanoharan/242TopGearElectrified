using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Physics;

namespace GameEngine.Renderer
{
    /// <summary>
    /// This interface must be implemented by each concrete car model renderer.
    /// This interface is required as each car model (coming from different sources)
    ///     has different ways of representing the model parts (e.g. wheels) and 
    ///     thus must be drawn differently.
    /// </summary>
    public abstract class CarModelRenderer : IRenderer
    {
        public abstract void DrawCar(CarActor owner);
    }
}
