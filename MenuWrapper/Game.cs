#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
#endregion

namespace NetworkStateManagement
{
    /// <summary>
    /// Sample showing how to manage the different game states involved in
    /// implementing a networked game, with menus for creating, searching,
    /// and joining sessions, a lobby screen, and the game itself. This main
    /// game class is extremely simple: all the interesting stuff happens
    /// in the ScreenManager component.
    /// </summary>
    public class NetworkStateManagementGame : Microsoft.Xna.Framework.Game
    {

        public static NetworkStateManagementGame MainGame;

        //private const int SCREEN_WIDTH = 1920;
        //private const int SCREEN_HEIGHT = 1080;
        private const bool FULL_SCREEN = true;


        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        #endregion

        #region Initialization


        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
            //graphics.PreferredBackBufferWidth = 400;
            //graphics.PreferredBackBufferHeight = 300;
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            //graphics.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Anisotropic;
            //graphics.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Linear;
            //graphics.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Linear;
            //graphics.GraphicsDevice.SamplerStates[0].MaxAnisotropy = 16;
            //graphics.PreferredBackBufferFormat = SurfaceFormat.Bgr32;
            graphics.IsFullScreen = FULL_SCREEN;
            graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public NetworkStateManagementGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            
            // Create components.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);
            Components.Add(new MessageDisplayComponent(this));
            Components.Add(new GamerServicesComponent(this));

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);

            // Listen for invite notification events.
            NetworkSession.InviteAccepted += (sender, e) => NetworkSessionComponent.InviteAccepted(screenManager, e);

            // To test the trial mode behavior while developing your game,
            // uncomment this line:

            // Guide.SimulateTrialMode = true;

            if (NetworkStateManagementGame.MainGame == null) NetworkStateManagementGame.MainGame = this;
        }


        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (NetworkStateManagementGame game = new NetworkStateManagementGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
