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
    internal class HudInfo : HudNode
    {
        private HudText _text;
        private Entity _spaceship;
        private PrimitivePhysicsComponent _physics;

        public HudInfo(Entity spaceship, Vector2I bounds, string renderTarget) : base(bounds, renderTarget)
        {
            _spaceship = spaceship;
            _physics = spaceship.GetComponent<PrimitivePhysicsComponent>();
            _renderTarget = renderTarget;
            Visible = true;
            _text = new HudText(this)
            {
                Text = "SpaceShip",
                TextScale = 2,
                TextColor = Color.White,
                TextOptions = TextDrawOptions.Centered,
            };
            _text.Position += new Vector2I(0, 20);
        }

        float time = 0;
        public override void Draw(float deltaTime)
        {
            time += deltaTime / 5;
            float delta = (float)(Math.Sin(time) + 1) / 2;
            _text.Text = $"Rings: {(int)(delta * 10)}/10";
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
