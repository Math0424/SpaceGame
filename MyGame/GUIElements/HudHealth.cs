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
    internal class HudHealth : HudNode
    {
        private SpaceshipController _spaceship;
        private HudText _healthOne;

        public HudHealth(Entity spaceship, Vector2I bounds, string renderTarget) : base(bounds, renderTarget)
        {
            _spaceship = spaceship.GetComponent<SpaceshipController>();
            _renderTarget = renderTarget;
            Visible = true;

            _healthOne = new HudText(this)
            {
                Text = "SpaceShip",
                TextScale = 2,
                TextColor = Color.Orange,
                TextAlignment = TextDrawOptions.Centered,
                zOffset = 9,
            };
        }

        public override void Draw(float deltaTime)
        {
            _healthOne.Text = $"Health: {(int)(_spaceship.Health * 100)}";
            DrawColoredSprite("Textures/GUI/ColorableSprite", Bounds / 2, Bounds * 2, 50, Color.Red);
            DrawColoredSprite("Textures/GUI/ColorableSprite", Bounds / 2, new Vector2I((int)(256 * _spaceship.Health), 256), 10, Color.Green);
        }

        public override void HandleInput(ref HudInput input)
        {

        }

        public override void Layout()
        {

        }
    }
}
