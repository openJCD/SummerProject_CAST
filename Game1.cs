using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HyperLinkUI.Scenes;
using HyperLinkUI.Engine.GUI;
using System.Reflection;
using System;
using HyperLinkUI.Engine;
using MonoTween;
using System.Linq;

namespace SummerProject_CAST
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SceneManager SceneManager;
        private Scene HomeScene;
        private UIRoot UI;
        public static GameWindow GameWindow;
        private MenuScene ms;
        Background BG;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content/GUI";
            UIEventHandler.OnKeyPressed += Game1_OnKeyPressed;
            UIEventHandler.OnButtonClick += Game1_OnButtonClick;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Game1_OnResize;
            GameWindow = Window;
            IsMouseVisible = true;
        }
        public void Game1_OnResize (object sender, EventArgs e)
        {
            
        }
        protected override void Initialize()
        {
            //Settings = new GameSettings();
            // TODO: Add your initialization logic here
            SceneManager = Core.Init(Content, _graphics, GameWindow, "Content/GUI/Saves/skin.ini");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            UI = new UIRoot();
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            Core.LoadAll(SceneManager, "Content/GUI/Scenes", "default.scene");
            Background.Import("Content/backgrounds/crosses.json", Content, out BG);
            new MenuScene().Load(UI, Content);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (IsActive)
            {
                // just there for the nice move-in animation at the start
                TweenManager.TickAllTweens((float)gameTime.ElapsedGameTime.TotalSeconds);
                //update UIRoot (Menu Scene) and ensure that everything stays centered when the client resizes
                UI.Update();
                UI.Width = GameWindow.ClientBounds.Width;
                UI.Height = GameWindow.ClientBounds.Height;
                // scroll the background for visual flare
                BG.Animate(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                GraphicsDevice.Clear(Color.Black);

                // separate calls for background and GUI
                _spriteBatch.Begin();
                BG.Draw(_spriteBatch, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
                _spriteBatch.End();

                // rasterizer clipping enabled to cut off pixels outside containers
                _spriteBatch.Begin(rasterizerState: new RasterizerState { ScissorTestEnable = true });
                UI.Draw(_spriteBatch);
                _spriteBatch.End();
            
                base.Draw(gameTime);
            }
        }

        public void Game1_OnKeyPressed (object sender, KeyPressedEventArgs e)
        {
        }

        public void Game1_OnButtonClick (object sender, OnButtonClickEventArgs e)
        {
            // implement quit event here, otherwise Quit main menu button does not work
            if (e.event_type == EventType.QuitGame)
            {
                Exit();
            }
        }
    }
}