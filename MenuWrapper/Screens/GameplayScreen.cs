#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using GameEngine;
using GameEngine.Input;
#endregion

namespace NetworkStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        NetworkSession networkSession;
        Engine gameEngine;
        #endregion

        #region Properties


        /// <summary>
        /// The logic for deciding whether the game is paused depends on whether
        /// this is a networked or single player game. If we are in a network session,
        /// we should go on updating the game even when the user tabs away from us or
        /// brings up the pause menu, because even though the local player is not
        /// responding to input, other remote players may not be paused. In single
        /// player modes, however, we want everything to pause if the game loses focus.
        /// </summary>
        new bool IsActive
        {
            get
            {
                if (networkSession == null)
                {
                    // Pause behavior for single player games.
                    return base.IsActive;
                }
                else
                {
                    // Pause behavior for networked games.
                    return !IsExiting;
                }
            }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(NetworkSession networkSession)
        {
            this.networkSession = networkSession;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);


            // setup keyboard configs:
            List<KeyConfig> keyConfigs = new List<KeyConfig>();
            keyConfigs.Add(new KeyConfig(Keys.W, Keys.S, Keys.A, Keys.D, Keys.C, Keys.Space));
            keyConfigs.Add(new KeyConfig(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.RightControl, Keys.End));
            //keyConfigs.Add(new KeyConfig(Keys.Q, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U));


            gameEngine = (networkSession == null) ?
                Engine.CreateLocalEngine(NetworkStateManagementGame.MainGame, keyConfigs) :
                Engine.CreateNetworkedEngine(NetworkStateManagementGame.MainGame, keyConfigs, networkSession);
            
            gameEngine.Initialize();
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            //if (content == null) content = new ContentManager(ScreenManager.Game.Services, "Content");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            // Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            //gameEngine.Dispose();
            //content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {

            // If we are in a network game, check if we should return to the lobby.
            if ((networkSession != null) && !IsExiting)
            {
                if (networkSession.SessionState == NetworkSessionState.Lobby)
                {
                    LoadingScreen.Load(ScreenManager, true, null,
                                       new BackgroundScreen(),
                                       new LobbyScreen(networkSession));
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsExiting) return;
            gameEngine.Update(gameTime);
        }
        
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            gameEngine.Draw(gameTime);
        }

        /// <summary>
        /// Handles input for the specified player. In local game modes, this is called
        /// just once for the controlling player. In network modes, it can be called
        /// more than once if there are multiple profiles playing on the local machine.
        /// Returns true if we should continue to handle input for subsequent players,
        /// or false if this player has paused the game.
        /// </summary>
        bool HandlePlayerInput(InputState input, PlayerIndex playerIndex)
        {
            // Look up inputs for the specified player profile.
            KeyboardState keyboardState = input.CurrentKeyboardStates[(int)playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[(int)playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[(int)playerIndex];

            if (input.IsPauseGame(playerIndex) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(networkSession), playerIndex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (ControllingPlayer.HasValue)
            {
                // In single player games, handle input for the controlling player.
                HandlePlayerInput(input, ControllingPlayer.Value);
            }
            else if (networkSession != null)
            {
                // In network game modes, handle input for all the
                foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
                {
                    if (!HandlePlayerInput(input, gamer.SignedInGamer.PlayerIndex))
                        break;
                }
            }
        }
        #endregion
    }
}
