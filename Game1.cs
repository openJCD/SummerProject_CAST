using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HyperLinkUI.Scenes;
using HyperLinkUI.Engine.GUI;

namespace SummerProject_CAST
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameSettings Settings;
        private SceneManager SceneManager;
        private Scene HomeScene;

        public static GameWindow GameWindow = GameWindow;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content/GUI/";
            UIEventHandler.OnKeyPressed += Game1_OnKeyPressed;
            UIEventHandler.OnButtonClick += Game1_OnButtonClick;
            IsMouseVisible = true;
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
            SceneManager = new SceneManager(Settings, "Settings", Content, _graphics, Window);
            SceneManager.CreateScenesFromFolder("Content/GUI/Scenes/");
            SceneManager.LoadScene("default.scene");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            SceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(rasterizerState: new RasterizerState { ScissorTestEnable = true });
            SceneManager.Draw(_spriteBatch);
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        public void Game1_OnKeyPressed (object sender, KeyPressedEventArgs e)
        {
            if (e.first_key_as_string == "F5")
            {
                Settings = GameSettings.TryLoadSettings("Content/GUI/Saves/", "settings.xml");
                Settings.LoadAllContent(Content);
                SceneManager.LoadScene(SceneManager.ActiveScene.Name);
            }
        }

        public void Game1_OnButtonClick (object sender, OnButtonClickEventArgs e)
        {
            if (e.event_type == EventType.QuitGame)
            {
                Exit();
            }
        }
    }
}