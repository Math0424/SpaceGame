using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project1.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems
{
    internal class SpectatorMovement : SystemComponent
    {
        private float yaw = 0, pitch = 0;
        public bool Controlling = true;
        private int JustFocused = 0;
        private World _world;
        private Camera _camera;

        public SpectatorMovement(World world, RenderingSystem render, Camera camera)
        {
            _camera = camera;
            _world = world;
            _world.GameFocused += CenterCursor;
        }

        public override void Close()
        {
            _world.GameFocused -= CenterCursor;
        }

        private void CenterCursor()
        {
            JustFocused = 5;
        }

        public override void Update(GameTime deltaTime)
        {
            if (!Controlling || !_world.Game.IsActive)
                return;

            float delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;
            
            var bounds = _world.Game.GraphicsDevice.Viewport.Bounds;
            var mousePos = Mouse.GetState().Position;
            float xdelta = (mousePos.X - (bounds.Width / 2)) * delta * 3f;
            float ydelta = (mousePos.Y - (bounds.Height / 2)) * delta * 3f;
            Mouse.SetPosition(bounds.Width / 2, bounds.Height / 2);

            if (JustFocused > 0)
            {
                JustFocused--;
                return;
            }

            yaw -= xdelta;
            pitch -= ydelta;

            if (pitch > 89.0f)
                pitch = 89.0f;
            if (pitch < -89.0f)
                pitch = -89.0f;

            Matrix m = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw), MathHelper.ToRadians(pitch), 0);
            m.Translation = _camera.Translation;

            delta *= 2.0f;
            if (Input.IsKeyDown(Keys.LeftShift))
                delta *= 10.0f;

            if (Input.IsKeyDown(Keys.W))
                m.Translation += _camera.Forward * delta;
            if (Input.IsKeyDown(Keys.S))
                m.Translation += _camera.Backward * delta;

            if (Input.IsKeyDown(Keys.A))
                m.Translation += _camera.Left * delta;
            if (Input.IsKeyDown(Keys.D))
                m.Translation += _camera.Right * delta;

            if (Input.IsKeyDown(Keys.Space))
                m.Translation += _camera.Up * delta;
            if (Input.IsKeyDown(Keys.C))
                m.Translation += _camera.Down * delta;

            if (Input.IsNewMouseDown(Input.MouseButtons.RightButton))
            {
                var ent = _world.CreateEntity()
                    .AddComponent(new PositionComponent(_camera.WorldMatrix.Translation + _camera.WorldMatrix.Forward * 5))
                    .AddComponent(new MeshComponent("models/sphere"))
                    .AddComponent(new PrimitivePhysicsComponent(RigidBodyType.Sphere, .08f, .3f));
                ent.Position.SetLocalMatrix(Matrix.CreateScale(.2f));
            }

            //if (Input.MouseWheelDelta() != 0)
            //{
            //    _camera.SetFOV(_camera.FOV - (Input.MouseWheelDelta() / 50f));
            //}
            _camera.SetWorldMatrix(m);
        }
    }
}
