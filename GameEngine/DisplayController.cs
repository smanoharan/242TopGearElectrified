using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Util;
using GameEngine.Physics;

namespace GameEngine
{
    /// <summary>
    /// Purpose : To support split screen
    /// 
    /// This class is a singleton
    /// 
    /// 
    /// Usage : The mainGame will get a list of cameras from The static DisplayController.
    ///         It will iterate through, set the camera as default, and call base.draw();
    /// 
    /// Detail: Each view in DisplayController has a cameraController. The view is responsible for setting up the camera's FOV and Aspect Ratio
    /// </summary>

    public class DisplayController : DrawableGameComponent

    {  
        protected static DisplayController displayController_;
        public static DisplayController Display { get { return displayController_; } }

        protected View[] views_;
        protected int currentViewIndex_;
        public View CurrentView { get { return views_[currentViewIndex_]; } }
        
        
        /// <summary>
        /// Switch the currentView to the next view.
        /// Returns whether or not there are more views left.
        /// 
        /// If currently at the last view, the first view is switched to.
        /// 
        /// Guarantee: No matter how many times this method is called, the currentView will always be a valid view.
        /// 
        /// NOTE: Views are cycled in reverse (but are still drawn in correct positions), to simplify comparisons
        /// </summary>
        /// <returns>True if there is at least one view left</returns>
        public bool NextView()
        {
            // if currently at the last screen to be drawn, switch back to the first view
            if (currentViewIndex_ == 0) currentViewIndex_ = cameraControllers_.Count;

            // iterate to the next view
            currentViewIndex_--;       
     
            // if at the last view now, return true.
            return (currentViewIndex_ != 0);
        }

        public int NumberOfViews { get { return views_.Length; } }

        private Game game_;
        private float totalFOV_;
        private List<CameraController> cameraControllers_;

        private DisplayController(Game game, List<CameraController> cameraControllers, float totalFOV)
            : base(game)
        {
            // just store the variables
            views_ = new View[cameraControllers.Count];
            game_ = game;
            totalFOV_ = totalFOV;
            cameraControllers_ = cameraControllers;
            currentViewIndex_ = cameraControllers.Count - 1;
        }

        protected override void LoadContent()
        {
            int numberOfScreens = cameraControllers_.Count;
            int width = game_.GraphicsDevice.Viewport.Width / numberOfScreens;
            float fov = totalFOV_ / numberOfScreens;

            int currentX = 0;
            int currentIndex = 0;
            foreach (CameraController control in cameraControllers_)
            {
                // Viewport is a struct, so it will be copied:
                Viewport v = game_.GraphicsDevice.Viewport;

                // adjust width and position on screen
                v.Width = width;
                v.X = currentX;
                currentX += width + 1;

                // create a new view and add it to the list
                views_[currentIndex] = new View(control, fov, v);
                currentIndex++;
            }
        }

        /// <summary>
        /// Create horizontal split screen with n segments. The number of segments is controlled by the number of cameraControllers passed in.
        /// This should only be called once (it is not possible to change the split screen config at runtime, at least, not right now.
        /// If called twice or more, all but the first method calls are ignored.
        /// </summary>
        /// <param name="game">The game this DisplayController belongs to</param>
        /// <param name="cameraControllers">The cameras to place in the split screen, in order</param>
        /// <param name="totalFOV">The total FOV of the screen</param>
        public static void CreateHorizontalScreens(Game game, List<CameraController> cameraControllers, float totalFOV)
        {
            if (displayController_ == null) displayController_ = new DisplayController(game, cameraControllers, totalFOV);
        }
    }

    /// <summary>
    /// A container for the specific attributes of a pane in split screen view.
    /// Contains a cameraController and a Viewport 
    /// </summary>
    public struct View
    {
        private CameraController cameraController_;
        public Camera Camera { get { return cameraController_.CurrentCamera; } }
        public CarActor Owner { get { return cameraController_.owner; } }
        
        private Viewport viewPort_;
        public Viewport ViewPort { get { return viewPort_; } }


        public View(CameraController cameraController, float FOV, Viewport viewport)
        {
            this.cameraController_ = cameraController;
            this.viewPort_ = viewport;

            // setup cameras:
            this.cameraController_.CurrentCamera.SetViewParameters(FOV, (float)viewport.Width/ viewport.Height);
        }
    }
}
