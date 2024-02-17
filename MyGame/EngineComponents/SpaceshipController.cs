using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems;
using Project2.MyGame.GUIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.MyGame.EngineComponents
{
    internal class SpaceshipController : EntityUpdateComponent
    {
        private float yaw = 0, pitch = 0;
        private bool _shipRotation;
        private bool _shotgunControl;
        
        private PrimitivePhysicsComponent _physics;
        private Matrix _localCameraPos;
        private int _justFocused;

        public float Health;

        private const float DAMPENING_SPEED = 10;
        private const float ROTATION_SPEED = 10;
        private const float ACCELERATION_SPEED = 100;

        public SpaceshipController(Matrix localCameraMatrix)
        {
            IsActive = true;
            _shipRotation = true;
            _shotgunControl = false;
           _localCameraPos = localCameraMatrix;
            Health = 1;
        }

        private void CenterCursor()
        {
            _justFocused = 5;
            var bounds = _entity.World.Game.GraphicsDevice.Viewport.Bounds;
            Mouse.SetPosition(bounds.Width / 2, bounds.Height / 2);
        }

        public override void Initalize()
        {
            _entity.World.GameFocused += CenterCursor;
            _physics = _entity.GetComponent<PrimitivePhysicsComponent>();
            _entity.World.GetSystem<PhysicsSystem>().Collision += Collision;
        }

        public void Collision(int ent, int with, Vector3 pos, Vector3 normal, float val)
        {
            if (ent == _physics.EntityId)
                if (val > 3)
                    Health -= val / 600;
        }

        public override void Update(GameTime deltaTime)
        {
            float delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;
            var pos = _entity.Position;
            var cam = _entity.World.Render.Camera;

            Quaternion shipRotation;
            pos.WorldMatrix.Decompose(out _, out shipRotation, out _);

            Quaternion rot = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);
            Quaternion cRot;
            Vector3 cPos;
            (_localCameraPos * pos.WorldMatrix).Decompose(out _, out cRot, out cPos);

            // the camera is one frame behind, just translate it forward by one frame
            // this can be fixed, but I dont want to go though the pain
            cam.SetWorldMatrix(Matrix.CreateFromQuaternion(cRot * rot) * Matrix.CreateTranslation(cPos + (_physics.LinearVelocity * delta)));

            if (!_entity.World.Game.IsActive || _justFocused > 0)
            {
                _justFocused--;
                return;
            }

            if (_shipRotation)
            {
                var bounds = _entity.World.Game.GraphicsDevice.Viewport.Bounds;
                var mousePos = Mouse.GetState().Position;
                float xdelta = (mousePos.X - (bounds.Width / 2)) * delta * 3f;
                float ydelta = (mousePos.Y - (bounds.Height / 2)) * delta * 3f;

                Mouse.SetPosition(bounds.Width / 2, bounds.Height / 2);

                _physics.AddTorque(pos.WorldMatrix.Left * ydelta * delta * ROTATION_SPEED);
                _physics.AddTorque(pos.WorldMatrix.Down * xdelta * delta * ROTATION_SPEED);
            }

            if (_shotgunControl)
            {
                var bounds = _entity.World.Game.GraphicsDevice.Viewport.Bounds;
                var mousePos = Mouse.GetState().Position;
                float xdelta = (mousePos.X - (bounds.Width / 2)) * delta * 0.1f;
                float ydelta = (mousePos.Y - (bounds.Height / 2)) * delta * 0.1f;

                Mouse.SetPosition(bounds.Width / 2, bounds.Height / 2);

                yaw -= xdelta;
                pitch -= ydelta;

                yaw = Math.Clamp(yaw, -1.5f, 1.5f);
                pitch = Math.Clamp(pitch, -0.4f, 1.5f);

                if (Input.IsNewMouseDown(Input.MouseButtons.LeftButton))
                {
                    _physics.AddImpulse(cam.Backward * 20, Vector3.Transform(new Vector3(0, 0.5f, 0), shipRotation));
                }
            }

            if (Input.IsMouseDown(Input.MouseButtons.RightButton))
            {
                _shipRotation = false;
                _shotgunControl = true;
                if (Input.IsNewMouseDown(Input.MouseButtons.RightButton))
                {
                    yaw = 0;
                    pitch = 0;
                }
            } 
            else
            {
                _shipRotation = true;
                _shotgunControl = false;
                yaw /= 1 + (delta * 20);
                pitch /= 1 + (delta * 20);
            }

            Vector3 addedForce = Vector3.Zero;
            if (Input.IsKeyDown(Keys.W))
                addedForce += (pos.WorldMatrix.Forward * delta * ACCELERATION_SPEED);
            if (Input.IsKeyDown(Keys.S))
                addedForce += (pos.WorldMatrix.Backward * delta * ACCELERATION_SPEED);
            if (Input.IsKeyDown(Keys.A))
                addedForce += (pos.WorldMatrix.Left * delta * ACCELERATION_SPEED);
            if (Input.IsKeyDown(Keys.D))
                addedForce += (pos.WorldMatrix.Right * delta * ACCELERATION_SPEED);

            if (Input.IsKeyDown(Keys.Space))
                addedForce += (pos.WorldMatrix.Up * delta * ACCELERATION_SPEED);
            if (Input.IsKeyDown(Keys.C))
                addedForce += (pos.WorldMatrix.Down * delta * ACCELERATION_SPEED);

            if (addedForce.LengthSquared() == 0)
                _physics.AddForce(-_physics.LinearVelocity * delta * DAMPENING_SPEED);
            else
                _physics.AddForce(addedForce);


            if (Input.IsKeyDown(Keys.E))
                _physics.AddTorque(pos.WorldMatrix.Forward * delta * ROTATION_SPEED);
            if (Input.IsKeyDown(Keys.Q))
                _physics.AddTorque(pos.WorldMatrix.Backward * delta * ROTATION_SPEED);
            _physics.AddTorque(-_physics.AngularVelocity * delta * DAMPENING_SPEED);


        }

        public override void Close()
        {
            base.Close();
            _entity.World.GameFocused -= CenterCursor;
        }
    }
}
