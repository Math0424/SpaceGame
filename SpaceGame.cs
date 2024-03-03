using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project1.Engine;
using Project1.Engine.Systems;
using Project1.Engine.Systems.GUI;
using Project1.MyGame;
using Project2.Engine;
using Project2.MyGame.EngineComponents;

namespace Project1
{
    public class SpaceGame : Game
    {
        private MainMenuXNAComponent _mainMenu;
        private World _gameWorld;

        public SpaceGame()
        {
            Content.RootDirectory = "Content";

            Render.Initalize(this);
            LoadMainMenu();
        }

        public void LoadMainMenu()
        {
            _mainMenu = new MainMenuXNAComponent(this);
            _mainMenu.StartGame += LoadIntoGame;
            Components.Add(_mainMenu);
            IsMouseVisible = true;
        }

        public void LoadIntoGame(byte checkpoints, byte difficulty, ushort seed)
        {
            Components.Remove(_mainMenu);
            _mainMenu.Dispose();
            _mainMenu = null;
            IsMouseVisible = false;

            _gameWorld = new World(this)
                .AddSystem<Camera>()
                .AddSystem<RenderingSystem>()
                .AddSystem<WorldGenerationSystem>()
                .AddSystem<PhysicsSystem>()
                .AddSystem<HudSystem>()
                .AddSystem<GameStateManager>();
            Components.Add(_gameWorld);
            var gamestate = _gameWorld.GetSystem<GameStateManager>();
            gamestate.CreateWorld(checkpoints, difficulty, seed);
            gamestate.Exit += LoadIntoMainMenu;
        }
        
        public void LoadIntoMainMenu()
        {
            Components.Remove(_gameWorld);
            _gameWorld.Dispose();
            _gameWorld = null;
            LoadMainMenu();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.IsNewKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
            if (this.IsActive)
                Input.UpdateState();
        }
    }
}