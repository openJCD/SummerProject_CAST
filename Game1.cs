using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HyperLinkUI.Scenes;
using HyperLinkUI.Engine.GUI;
using System.Reflection;
using System;
using HyperLinkUI.Engine;
using MonoTween;

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

            // TODO: Add your update logic here
            //SceneManager.Update(gameTime);
            if (IsActive)
            {
                TweenManager.TickAllTweens((float)gameTime.ElapsedGameTime.TotalSeconds);
                UI.Update();
                UI.Width = GameWindow.ClientBounds.Width;
                UI.Height = GameWindow.ClientBounds.Height;
                BG.Animate(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                GraphicsDevice.Clear(Color.Black);
                _spriteBatch.Begin();
                BG.Draw(_spriteBatch, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
                _spriteBatch.End();
                // TODO: Add your drawing code here
                _spriteBatch.Begin(rasterizerState: new RasterizerState { ScissorTestEnable = true });
                // draw a nice background here
                //SceneManager.Draw(_spriteBatch);
                UI.Draw(_spriteBatch);
                
                _spriteBatch.End();
            
                base.Draw(gameTime);
            }
        }

        public void Game1_OnKeyPressed (object sender, KeyPressedEventArgs e)
        {
            if (e.first_key_as_string == "F5")
            {
                UI.ChildContainers.Clear();
                new MenuScene().Load(UI, Content);
                Core.ReloadAt(SceneManager, "default.scene");
            }
        }

        public void Game1_OnButtonClick (object sender, OnButtonClickEventArgs e)
        {
            if (e.event_type == EventType.QuitGame)
            {
                Exit();
            }
        }

        public static void RegisterLuaMethod(Scene scene, MethodInfo method)
        {
            scene.ScriptHandler.RegisterFunction(method.Name, method);
        }
    }
}