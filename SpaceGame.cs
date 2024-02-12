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
    public class SpaceGame : Game
    {
        private MainMenuXNAComponent _mainMenu;

        public SpaceGame()
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

        }
        
        public void LoadIntoMainMenu()
        {

            LoadMainMenu();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.IsNewKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
            Input.UpdateState();
        }
    }
}