using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HyperLinkUI.Scenes;
using HyperLinkUI.Engine.GUI;
using System.Reflection;
using System;

namespace SummerProject_CAST
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameSettings Settings;
        private SceneManager SceneManager;
        private Scene HomeScene;
        private UIRoot UI;
        public static GameWindow GameWindow = GameWindow;
        private MenuScene ms;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content/GUI/";
            UIEventHandler.OnKeyPressed += Game1_OnKeyPressed;
            UIEventHandler.OnButtonClick += Game1_OnButtonClick;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Game1_OnResize;
            IsMouseVisible = true;
        }
        public void Game1_OnResize (object sender, EventArgs e)
        {
            
        }
        protected override void Initialize()
        {
            Settings = new GameSettings();
            // TODO: Add your initialization logic here
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Settings = GameSettings.TryLoadSettings("Content/GUI/Saves/", "settings.xml");
            Settings.LoadAllContent(Content);
            _graphics.PreferredBackBufferWidth = Settings.WindowWidth;
            _graphics.PreferredBackBufferHeight = Settings.WindowHeight;
            _graphics.ApplyChanges();
            SceneManager = new SceneManager(Settings, "Settings", Content, _graphics, Window);
            SceneManager.CreateScenesFromFolder("Content/GUI/Scenes/");
            SceneManager.LoadScene("default.scene");
            UI = new UIRoot(_graphics, Settings);
            ms = new MenuScene();
            ms.Load(UI);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //SceneManager.Update(gameTime);
            UI.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(rasterizerState: new RasterizerState { ScissorTestEnable = true });
            // draw a nice background here
            //SceneManager.Draw(_spriteBatch);
            UI.Draw(_spriteBatch);
            
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        public void Game1_OnKeyPressed (object sender, KeyPressedEventArgs e)
        {
            if (e.first_key_as_string == "F5")
            {
                Settings = GameSettings.TryLoadSettings("Content/GUI/Saves/", "settings.xml");
                Settings.LoadAllContent(Content);
                _graphics.PreferredBackBufferWidth = Settings.WindowWidth;
                _graphics.PreferredBackBufferHeight = Settings.WindowHeight;
                _graphics.ApplyChanges();
                //SceneManager.LoadScene(SceneManager.ActiveScene.Name);
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