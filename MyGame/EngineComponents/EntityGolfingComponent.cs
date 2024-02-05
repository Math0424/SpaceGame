using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.MyGame
{
    internal class EntityGolfingComponent : EntityUpdateComponent
    {

        public int Strokes { get; private set; }
        public bool TurnComplete { get; private set; }
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                _hasStroked = false;
                TurnComplete = false;
            }
        }
        public string Name { get; private set; }
        
        private Vector3 _resetPos;
        private float _killLevel;
        private Vector2 _mouseDragStart;
        private bool _hasStroked;
        private bool _isActive;
        private Color _color;

        public EntityGolfingComponent(Vector3 resetPos, float killLevel, string name, Color color)
        {
            _color = color;
            Name = name;
            _resetPos = resetPos;
            _killLevel = killLevel;
            IsActive = false;
        }

        public override void Update(GameTime deltaTime)
        {
            var camera = _entity.World.Render.Camera;
            var physics = _entity.GetComponent<PrimitivePhysicsComponent>();

            if (!IsActive)
            {
                DrawingUtils.DrawWorldText(_entity.World.Render, $"{Name}\n{Strokes} strokes", _entity.Position.Position + camera.Up, _color, TextDrawOptions.Centered);
                return;
            }


            var matrix = camera.WorldMatrix;
            matrix.Translation = _entity.Position.Position;
            camera.SetWorldMatrix(matrix);

            if (_entity.Position.Position.Y < _killLevel)
            {
                Strokes++;
                physics.SetWorldMatrix(Matrix.CreateTranslation(_resetPos));
                physics.Stop();
            }

            if (physics.IsSleeping)
            {
                if (_hasStroked)
                {
                    IsActive = false;
                    TurnComplete = true;
                    _hasStroked = false;
                    return;
                }

                if (_mouseDragStart == Vector2.Zero)
                {
                    Vector3 screen = _entity.World.Render.Camera.WorldToScreen(_entity.Position.Position);
                    Rectangle rect = new Rectangle((int)screen.X - 50, (int)screen.Y - 50, 100, 100);
                    _entity.World.Render.EnqueueMessage(new RenderMessageDrawColoredSprite("Textures/GUI/circle", rect, 0, _color));
                }

                if (Input.IsNewMouseDown(Input.MouseButtons.LeftButton))
                    _mouseDragStart = Input.MousePosition();

                if (_mouseDragStart != Vector2.Zero)
                {
                    if (Input.IsNewMouseUp(Input.MouseButtons.LeftButton))
                    {
                        Vector2 deltaMouse = Input.MousePosition() - _mouseDragStart;
                        Vector3 val = -new Vector3(deltaMouse.X, 0, deltaMouse.Y) / 600;
                        physics.AddForce(val);
                        _hasStroked = true;
                        _mouseDragStart = Vector2.Zero;
                        _entity.World.GetSystem<SoundSystem>().PlaySoundEffect("Audio/hit_ball");
                        Strokes++;
                    }

                    if (Input.IsMouseDown(Input.MouseButtons.LeftButton))
                    {
                        _entity.World.Game.IsMouseVisible = false;
                        Vector2 deltaMouse = Input.MousePosition() - _mouseDragStart;
                        Vector3 val = new Vector3(deltaMouse.X, 0, deltaMouse.Y);
                        _entity.World.Render.EnqueueMessage(new RenderMessageDrawLine(_entity.Position.Position, _entity.Position.Position + val / 10, Color.Red));
                    }
                    else
                        _entity.World.Game.IsMouseVisible = true;
                }
            }

        }
    }
}
