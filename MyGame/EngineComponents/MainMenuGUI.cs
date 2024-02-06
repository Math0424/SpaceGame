using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Systems.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.MyGame
{
    internal class MainMenuGUI : HudCore
    {
        public Action<string, int> StartGame;

        private HudTextButton _btn2;
        private HudTextButton _sourceCodeBtn;

        public MainMenuGUI(HudRoot root) : base(root)
        {
            Visible = true;

            var text = new HudText(this)
            {
                Text = "Space Game",
                TextScale = 3,
                TextColor = Color.White,
            };
            text.Position = text.Position - new Vector2I(0, 180);

            _sourceCodeBtn = new HudTextButton(this)
            {
                Bounds = new Vector2I(200, 50),
                Padding = 20,
                Text = "Source Code",
                ParentAlignment = ParentAlignments.Left | ParentAlignments.Bottom | ParentAlignments.Inner | ParentAlignments.Padding
            };
            _sourceCodeBtn.OnLeftClicked += (e) => { Process.Start("explorer", "https://github.com/Math0424/GolfingGame"); };

            _btn2 = new HudTextButton(_sourceCodeBtn)
            {
                Padding = 10,
                Text = "Join Game",
                Bounds = new Vector2I(200, 50),
                ParentAlignment = ParentAlignments.Top | ParentAlignments.Padding
            };
            _btn2.OnLeftClicked += (e) =>
            {
                //StartGame?.Invoke(null, _playerCount);
            };

            var _btn3 = new HudTextButton(_btn2)
            {
                Padding = 10,
                Text = "Create Lobby",
                Bounds = new Vector2I(200, 50),
                ParentAlignment = ParentAlignments.Top | ParentAlignments.Padding
            };
            _btn2.OnLeftClicked += (e) =>
            {
                //StartGame?.Invoke(null, _playerCount);
            };

            var _btn4 = new HudTextButton(_btn3)
            {
                Padding = 10,
                Text = "Start Singleplayer",
                Bounds = new Vector2I(200, 50),
                ParentAlignment = ParentAlignments.Top | ParentAlignments.Padding
            };
            _btn2.OnLeftClicked += (e) =>
            {
                //StartGame?.Invoke(null, _playerCount);
            };

        }
    }
}
