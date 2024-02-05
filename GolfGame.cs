using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems;
using Project1.Engine.Systems.GUI;
using Project1.MyGame;
using System;
using System.Collections.Generic;
using System.IO;

namespace Project1
{
    public class GolfGame : Game
    {
        private MainMenuXNAComponent _mainMenu;
        private GolfingGameXNAComponent _golfGame;

        public GolfGame()
        {
            Content.RootDirectory = "Content";
            //Window.AllowUserResizing = true;

            IsMouseVisible = true;
            LoadMainMenu();
        }

        public void LoadMainMenu()
        {
            _mainMenu = new MainMenuXNAComponent(this);
            _mainMenu.StartGame += LoadIntoGame;
            Components.Add(_mainMenu);
        }

        public void LoadIntoGame(string worldName, int playerCount)
        {
            Components.Remove(_mainMenu);
            _mainMenu.Dispose();
            _mainMenu = null;

            _golfGame = new GolfingGameXNAComponent(this, worldName, playerCount);
            _golfGame.InvokeMainMenu += LoadIntoMainMenu;
            Components.Add(_golfGame);
        }
        
        public void LoadIntoMainMenu()
        {
            _golfGame.Dispose();
            Components.Remove(_golfGame);
            _golfGame = null;

            LoadMainMenu();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_golfGame == null && Input.IsNewKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
            Input.UpdateState();
        }
    }
}