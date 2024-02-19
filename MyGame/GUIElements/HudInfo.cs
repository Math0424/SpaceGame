using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems.GUI;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine;
using Project2.MyGame.EngineComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.MyGame.GUIElements
{
    internal class HudInfo : HudNode
    {
        private HudText _text, _points, _time;
        private GameStateManager _manager;

        public HudInfo(GameStateManager manager, Vector2I bounds, string renderTarget) : base(bounds, renderTarget)
        {
            _manager = manager;
            _renderTarget = renderTarget;
            Visible = true;
            _text = new HudText(this)
            {
                Text = "SpaceShip",
                TextScale = 2,
                TextColor = Color.White,
                TextAlignment = TextDrawOptions.Centered,
            };
            _text.Position += new Vector2I(0, 22);
            _points = new HudText(this)
            {
                Text = "SpaceShip",
                TextScale = 1,
                TextColor = Color.White,
                TextAlignment = TextDrawOptions.Centered,
            };
            _points.Position += new Vector2I(0, -5);

            _time = new HudText(this)
            {
                Text = "0:00:000",
                TextScale = .75f,
                TextColor = Color.White,
                TextAlignment = TextDrawOptions.Centered,
            };
            _time.Position += new Vector2I(0, -32);
        }

        public override void Draw(float deltaTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(_manager.ElapsedTime);
            _time.Text = $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:D2}:{timeSpan.Milliseconds:D3}";
            _text.Text = $"Rings: {_manager.CompletedRings}/{_manager.TotalRings + 1}";
            _points.Text = _manager.Points.ToString();
            DrawColoredSprite("Textures/GUI/ColorableSprite", Vector2I.Zero, Bounds * 3, 10, Color.Black);
        }

        public override void HandleInput(ref HudInput input)
        {

        }

        public override void Layout()
        {

        }
    }
}
