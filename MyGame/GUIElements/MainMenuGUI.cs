using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Systems.GUI;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine.Systems.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.MyGame.GUIElements
{
    internal class MainMenuGUI : HudNode
    {
        private HudButton _startGame, _btnEasy, _btnNormal, _btnHard;
        private HudSlider _sliderCheckpoints, _sliderDifficulty, _sliderSeed;
        private HudText _checkPointsText, _difficultyText, _seedText;

        private HudText _saves;

        private const float MAX_CHECKPOINTS = 100;

        public MainMenuGUI(Vector2I screenBounds) : base(screenBounds, "default")
        {
            _startGame = new HudTextButton(this)
            {
                Bounds = new Vector2I(400 - 80, 75),
                Padding = 40,
                Text = "Start Game",
                ParentAlignment = ParentAlignments.Left | ParentAlignments.Bottom | ParentAlignments.Inner,
            };

            new HudText(this)
            {
                Padding = 180,
                Text = "Space Race",
                TextScale = 3,
                ParentAlignment = ParentAlignments.Top | ParentAlignments.Left | ParentAlignments.Inner,
            };

            _saves = new HudText(this)
            {
                Padding = 100,
                TextFont = "Fonts/Monospace",
                Text = "    Time   |    Score    |   Level",
                TextScale = 1,
                TextColor = Color.White,
                TextAlignment = TextDrawOptions.Left,
                ParentAlignment = ParentAlignments.Top | ParentAlignments.Inner,
            };

            // Random r = new Random();
            // for(int i = 0; i < 20; i++)
            // {
            //     GameSaverLoader.SaveGame(new GameSaverLoader.Save
            //     {
            //         LevelData = r.Next(),
            //         Points = r.Next(),
            //         Time = r.NextInt64(),
            //     });
            // }

            foreach (var x in GameSaverLoader.GetSaves())
                _saves.Text += $"\n{TimeSpan.FromTicks(x.Time).ToString("mm\\:ss\\:fff"), -10} | {x.Points, 11} | {x.LevelData}";

            _sliderSeed = new HudSlider(_startGame)
            {
                Bounds = new Vector2I(400 - 80, 20),
                Padding = 20,
                ScrollbarWidth = 20,
                ParentAlignment = ParentAlignments.Top,
            };
            _seedText = new HudText(_sliderSeed)
            {
                Padding = 15,
                TextAlignment = TextDrawOptions.Left,
                ParentAlignment = ParentAlignments.Top | ParentAlignments.Left | ParentAlignments.InnerV,
            };

            _sliderDifficulty = new HudSlider(_startGame)
            {
                Bounds = new Vector2I(400 - 80, 20),
                Padding = 80,
                ScrollbarWidth = 20,
                ParentAlignment = ParentAlignments.Top,
            };
            _difficultyText = new HudText(_sliderDifficulty)
            {
                Padding = 15,
                TextAlignment = TextDrawOptions.Left,
                ParentAlignment = ParentAlignments.Top | ParentAlignments.Left | ParentAlignments.InnerV,
            };

            _sliderCheckpoints = new HudSlider(_startGame)
            {
                Bounds = new Vector2I(400 - 80, 20),
                Padding = 140,
                ScrollbarWidth = 20,
                ParentAlignment = ParentAlignments.Top,
            };
            _checkPointsText = new HudText(_sliderCheckpoints)
            {
                Padding = 15,
                TextAlignment = TextDrawOptions.Left,
                ParentAlignment = ParentAlignments.Top | ParentAlignments.Left | ParentAlignments.InnerV,
            };

            _btnEasy = new HudTextButton(this)
            {
                Text = "Easy",
                Padding = 40,
                Bounds = new Vector2I(70, 60),
                ParentAlignment = ParentAlignments.Bottom | ParentAlignments.Left | ParentAlignments.Inner,
            };
            _btnEasy.Position += new Vector2I(0, -280);
            _btnEasy.OnLeftClicked += (e) => { SetDifficulty(20, 0.1f); };

            _btnNormal = new HudTextButton(_btnEasy)
            {
                Text = "Normal",
                Padding = 5,
                Bounds = new Vector2I(70, 60),
                ParentAlignment = ParentAlignments.Right,
            };
            _btnNormal.OnLeftClicked += (e) => { SetDifficulty(50, 0.5f); };

            _btnHard = new HudTextButton(_btnNormal)
            {
                Text = "Hard",
                Bounds = new Vector2I(70, 60),
                Padding = 5,
                ParentAlignment = ParentAlignments.Right,
            };
            _btnHard.OnLeftClicked += (e) => { SetDifficulty(70, 1); };

        }

        private void SetDifficulty(byte checkpoints, float difficulty)
        {
            Random random = new Random();
            _sliderCheckpoints.ScrollbarPosition = checkpoints / MAX_CHECKPOINTS;
            _sliderDifficulty.ScrollbarPosition = difficulty;
            _sliderSeed.ScrollbarPosition = (float)random.NextDouble();
        }

        public override void Draw(float deltaTime)
        {
            _checkPointsText.Text = $"Checkpoints: {Math.Max((int)(MAX_CHECKPOINTS * _sliderCheckpoints.ScrollbarPosition), 1)}";
            _difficultyText.Text = $"Difficulty: {Math.Max(Math.Round(_sliderDifficulty.ScrollbarPosition, 2), 0.01f)}";
            _seedText.Text = $"Seed: {(int)(ushort.MaxValue * _sliderSeed.ScrollbarPosition)}";

            DrawColoredSprite("Textures/GUI/ColorableSprite", new Vector2I(200, Bounds.Y / 2), new Vector2I(400, Bounds.Y), zOffset + 1, Color.LightSlateGray);
        }

        public override void HandleInput(ref HudInput input)
        {

        }

        public override void Layout()
        {

        }
    }
}
