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
    internal class HudSpeed : HudNode
    {
        private HudText _text;
        private Entity _spaceship;
        private PrimitivePhysicsComponent _physics;

        public HudSpeed(Entity spaceship, Vector2I bounds, string renderTarget) : base(bounds, renderTarget)
        {
            _spaceship = spaceship;
            _physics = spaceship.GetComponent<PrimitivePhysicsComponent>();
            _renderTarget = renderTarget;
            Visible = true;
            _text = new HudText(this)
            {
                Text = "SpaceShip",
                TextScale = 3,
                TextColor = Color.White,
                TextAlignment = TextDrawOptions.Left,
            };
            _text.Position += new Vector2I(-90, -50);
        }

        public override void Draw(float deltaTime)
        {
            _text.Text = "Speed:\n" +
                $"{(int)_physics.LinearVelocity.Length()} m/s\n" +
                $"{Math.Round(_physics.AngularVelocity.Length(), 2)} w\n";
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
