using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems.GUI;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.MyGame.GUIElements
{
    internal class HudHealth : HudNode
    {
        float _health;
        private Entity _spaceship;
        private PrimitivePhysicsComponent _physics;
        private HudText _healthOne;

        public HudHealth(Entity spaceship, Vector2I bounds, string renderTarget) : base(bounds, renderTarget)
        {
            _spaceship = spaceship;
            _physics = spaceship.GetComponent<PrimitivePhysicsComponent>();
            _renderTarget = renderTarget;
            Visible = true;
            _health = 1;

            _healthOne = new HudText(this)
            {
                Text = "SpaceShip",
                TextScale = 2,
                TextColor = Color.Orange,
                TextOptions = TextDrawOptions.Centered,
                zOffset = 9,
            };
        }

        float time;
        public override void Draw(float deltaTime)
        {
            time += deltaTime / 10;

            _health = (float)(Math.Sin(time) + 1f) / 2;

            _healthOne.Text = $"Health: {(int)(_health * 100)}";

            DrawColoredSprite("Textures/GUI/ColorableSprite", Bounds / 2, Bounds * 2, 50, Color.Red);
            DrawColoredSprite("Textures/GUI/ColorableSprite", Bounds / 2, new Vector2I((int)(256 * _health), 256), 10, Color.Green);
        }

        public override void HandleInput(ref HudInput input)
        {

        }

        public override void Layout()
        {

        }
    }
}
