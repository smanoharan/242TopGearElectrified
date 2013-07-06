using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Cameras;
using GameEngine.Input;
using GameEngine.Network;
using GameEngine.Particles;
using GameEngine.Physics;
using GameEngine.Renderer;
using GameEngine.Track;
using GameEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

//this is the only reference to which physics engine is being used.
using defaultCarActor = GameEngine.Physics.BepuPhysicsAdapter.BepuCarActor;
using defaultPhysicsSystem = GameEngine.Physics.BepuPhysicsAdapter.BepuPhysicsSystem;


namespace GameEngine
{

    /// <summary>
    /// Holds and runs the playable part of the project, i.e. everything except the menus.
    /// The external 'Game' object must call Update and Draw as per appropriate
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// The current physics system in use.
        /// </summary>
        public static PhysicsSystem currentWorld;

               
        #region Members and Properties
        private PhysicsSystem world;

        private Game game;

        private List<GameComponent> Components;
        private List<DrawableGameComponent> DrawableComponents;
        private List<DrawableGameComponent> LateDrawComponents; // components which must be drawn last for each pane
        private List<DrawableGameComponent> OverlayComponents;  // components which must be drawn last for each frame
        private List<CarActor> carActors;
        private TrackActor track;
        private List<CameraController> cameraControllerList;
        #endregion


        #region Factory Methods and Constructor
        // private constructor: use factory methods instead
        private Engine(Game game, List<GamerProfile> sortedProfiles, 
            List<KeyConfig> localPlayerInputConfigs, out Dictionary<byte, LinkedList<CarActor>> idCarActorMap)
        {
            this.game = game;
            
            // create a physics system:
            world = new defaultPhysicsSystem();

            // set it as the current
            Engine.currentWorld = world;

            // setup lists for components, cameras and for networking:
            Components = new List<GameComponent>();
            DrawableComponents = new List<DrawableGameComponent>();
            LateDrawComponents = new List<DrawableGameComponent>();
            OverlayComponents = new List<DrawableGameComponent>();
            carActors = new List<CarActor>();
            cameraControllerList = new List<CameraController>();
            idCarActorMap = new Dictionary<byte, LinkedList<CarActor>>();

            // create the track
            int totalCars = 0; 
            foreach (GamerProfile gp in sortedProfiles) totalCars += gp.numCars;
            track = TrackManager.CreateTrack(game, totalCars);
            this.AddComponent(track);

            // create the car Actors & players
            int carCount = 0;
            foreach (GamerProfile profile in sortedProfiles)
            {
                LinkedList<CarActor> cars = new LinkedList<CarActor>();

                // create the carActors
                for (int i = 0; i < profile.numCars; i++)
                {
                    // create the carRenderer
                    CarModelRenderer carModelRenderer = new DefaultCarModelRenderer(game, (DefaultCarModelRenderer.TextureColourScheme)(carCount + 2  % (int)(DefaultCarModelRenderer.TextureColourScheme.Count)));

                    // add the car to the list
                    CarActor carActor = new defaultCarActor(game, carModelRenderer, Matrix.Identity); // all start off at origin. Moved at Initialize
                    carActors.Add(carActor);
                    cars.AddLast(carActor);
                    AddComponent(carActor);
                    carCount++;

                    // decorate the carActor with a particle system
                    AddComponent(new ParticleController(game, carActor));

                    //add HUD for player
                    //HUD hud = new HUD(game, carActor);
                    //OverlayComponents.Add(hud);
                    //Components.Add(hud);

                    // if player is local, setup the local cameras and input schemes:
                    if (profile.isLocal)
                    {
                        CameraController c = new CameraController(game, carActor);
                        this.AddComponent(c);
                        //if (cameraControllerList.Count == 0)
                        cameraControllerList.Add(c);
                        CarSoundManager soundManager = new CarSoundManager(carActor, c, game);
                        AddComponent(new Player(game, carActor, localPlayerInputConfigs[i], c));
                        AddComponent(soundManager);                       
                    }
                }

                // register gamer into map
                idCarActorMap.Add(profile.gamerId, cars);
            }
            
            // setup split screen
            DisplayController.CreateHorizontalScreens(game, cameraControllerList, MathHelper.PiOver4);
            AddComponent(DisplayController.Display);

            

            // draw skybox, then bloom, then fps on top
            LateDrawComponents.Add(new SkyBoxActor(game));


            //FrameRateCounter fpsDisplay = new FrameRateCounter(game);
            //OverlayComponents.Add(fpsDisplay);
            //Components.Add(fpsDisplay);

        }

        public static Engine CreateNetworkedEngine(Game game, List<KeyConfig> localInputKeyConfigs, NetworkSession session)
        {

            // setup GamerProfiles
            List<GamerProfile> profiles = new List<GamerProfile>();
            foreach (NetworkGamer gamer in session.AllGamers) profiles.Add(new GamerProfile(gamer.Id, (int)gamer.Tag, gamer.IsLocal));

            Dictionary<byte, LinkedList<CarActor>> idCarActorMap;
            Engine gameEngine = new Engine(game, profiles, localInputKeyConfigs, out idCarActorMap);
            
            // setup network agent:
            gameEngine.AddComponent(new PeerToPeerNetworkAgent(game, idCarActorMap, session.LocalGamers[0]));

            return gameEngine;
        }

        /// <summary>
        /// Create a new single player game
        /// KeyConfig: Key configuration for each player
        ///     The number of carActors are inferred from the number of KeyConfigs
        /// </summary>
        /// <param name="game"></param>
        /// <param name="KeyConfigs"></param>
        /// <returns></returns>
        public static Engine CreateLocalEngine(Game game, List<KeyConfig> KeyConfigs)
        {
            // create a gamer profile for a single host, with n cars:
            List<GamerProfile> profiles = new List<GamerProfile>();
            profiles.Add(new GamerProfile(0, KeyConfigs.Count, true));

            // setup the game
            Dictionary<byte, LinkedList<CarActor>> idCarActorMap;
            return new Engine(game, profiles, KeyConfigs, out idCarActorMap);
        }

        private void AddComponent(GameComponent component)
        {
            Components.Add(component);
        }
        private void AddComponent(DrawableGameComponent component)
        {
            Components.Add(component);
            DrawableComponents.Add(component);
        }
        #endregion

        #region Init, Update and Draw
        public void Initialize()
        {


            foreach (DrawableGameComponent dgc in DrawableComponents) dgc.Initialize();


            List<Matrix> startPositions = track.getStartingPositions();
            
            while (startPositions.Count < carActors.Count)
            {
                startPositions.Add(Matrix.CreateTranslation(1000f, 0f, 1000f));
            }
            
            for (int i = 0; i < carActors.Count; i++)
            {
                carActors[i].startingPosition = startPositions[i];
                carActors[i].TeleportToStartingPosition();
            }

            //int currentCarActorIndex = 0;
            // setup HUDs
            //do
            //{
            //    Viewport currentView = DisplayController.Display.CurrentView.ViewPort;
            //    int xEdge = currentView.X + currentView.Width - 1;
            //    int yEdge = currentView.Y + currentView.Height - 1;
                
            //}
            //while (DisplayController.Display.NextView());
            do { } while (DisplayController.Display.NextView());

            // add HUDS
            do
            {
                Viewport v = DisplayController.Display.CurrentView.ViewPort;
                HUD h = new HUD(game, DisplayController.Display.CurrentView.Owner, v.X + v.Width -1, v.Height - 1);
                Components.Add(h);
                OverlayComponents.Add(h);
            }
            while (DisplayController.Display.NextView());

            // bloom is on top of huds
            OverlayComponents.Add(new BloomComponent(game));

            // init separately drawn components:
            foreach (DrawableGameComponent dgc in LateDrawComponents) dgc.Initialize();
            foreach (DrawableGameComponent dgc in OverlayComponents) dgc.Initialize();

        }
        public void Update(GameTime gameTime)
        {
            // update physics
            int GameSpeed = 4;
            for (int i=0; i<GameSpeed; i++) world.Update(gameTime);
            
            // update every component;
            foreach (GameComponent gc in Components) gc.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            GraphicsDevice device = game.GraphicsDevice;

            // save the current viewport:
            Viewport current = device.Viewport;
            device.Clear(Color.CornflowerBlue);
            device.RenderState.CullMode = CullMode.None;
            device.RenderState.FillMode = FillMode.Solid;


            // foreach camera controller (each pane of split screen):
            do
            {
                //device.RenderState.FillMode = wf ? FillMode.Solid : FillMode.WireFrame;
                //wf = !wf;

                // setup device state:
                device.RenderState.DepthBufferEnable = true;
                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.AlphaTestEnable = false;
                device.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
                device.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
                device.Viewport = DisplayController.Display.CurrentView.ViewPort;

                // draw every drawable component:
                foreach (DrawableGameComponent dgc in DrawableComponents) dgc.Draw(gameTime);

                // draw hud and other top elements
                foreach (DrawableGameComponent dgc in LateDrawComponents) dgc.Draw(gameTime);

            }
            while (DisplayController.Display.NextView());

            // reset the viewport
            device.Viewport = current;
            foreach (DrawableGameComponent dgc in OverlayComponents) dgc.Draw(gameTime);
            

            
        }
        #endregion

    }


    /// <summary>
    /// To collectively represent information about carActors
    /// </summary>
    struct GamerProfile : IComparable<GamerProfile>
    {
        public byte gamerId;
        public int numCars;
        public bool isLocal;

        public GamerProfile(byte gamerId, int numCars, bool isLocal)
        {
            this.gamerId = gamerId;
            this.numCars = numCars;
            this.isLocal = isLocal;
        }

        public int CompareTo(GamerProfile other)
        {
            return this.gamerId - other.gamerId;
        }
    }
}
