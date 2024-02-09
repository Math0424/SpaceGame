﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project1.Engine;
using Project1.Engine.Components;
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
        private bool _cameraRotation;
        
        private PrimitivePhysicsComponent _physics;
        private Matrix _localCameraPos;
        private int _justFocused;
        private const float ROTATION_SPEED = 6;
        private const float ACCELERATION_SPEED = 6;

        public SpaceshipController(Matrix localCameraMatrix)
        {
            IsActive = true;
            _shipRotation = true;
            _cameraRotation = false;
           _localCameraPos = localCameraMatrix;
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
        }

        public override void Update(GameTime deltaTime)
        {
            float delta = (float)deltaTime.ElapsedGameTime.TotalSeconds;
            var pos = _entity.Position;
            var cam = _entity.World.Render.Camera;

            Quaternion rot = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);
            Quaternion cRot;
            Vector3 cPos;
            (_localCameraPos * pos.WorldMatrix).Decompose(out _, out cRot, out cPos);

            cam.SetWorldMatrix(Matrix.CreateFromQuaternion(cRot * rot) * Matrix.CreateTranslation(cPos));

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

            if (_cameraRotation)
            {
                var bounds = _entity.World.Game.GraphicsDevice.Viewport.Bounds;
                var mousePos = Mouse.GetState().Position;
                float xdelta = (mousePos.X - (bounds.Width / 2)) * delta * 0.1f;
                float ydelta = (mousePos.Y - (bounds.Height / 2)) * delta * 0.1f;

                Mouse.SetPosition(bounds.Width / 2, bounds.Height / 2);

                yaw -= xdelta;
                pitch -= ydelta;

                yaw = Math.Clamp(yaw, -1.5f, 1.5f);
                pitch = Math.Clamp(pitch, -1.5f, 1.5f);
            }

            if (Input.IsMouseDown(Input.MouseButtons.RightButton))
            {
                _shipRotation = false;
                _cameraRotation = true;
                if (Input.IsNewMouseDown(Input.MouseButtons.RightButton))
                {
                    yaw = 0;
                    pitch = 0;
                }
            } 
            else
            {
                _shipRotation = true;
                _cameraRotation = false;
                yaw /= 1 + (delta * 20);
                pitch /= 1 + (delta * 20);
            }

            if (Input.IsKeyDown(Keys.W))
                _physics.AddForce(pos.WorldMatrix.Forward * delta * ACCELERATION_SPEED);
            if (Input.IsKeyDown(Keys.S))
                _physics.AddForce(pos.WorldMatrix.Backward * delta * ACCELERATION_SPEED);
            if (Input.IsKeyDown(Keys.A))
                _physics.AddForce(pos.WorldMatrix.Left * delta * ACCELERATION_SPEED);
            if (Input.IsKeyDown(Keys.D))
                _physics.AddForce(pos.WorldMatrix.Right * delta * ACCELERATION_SPEED);

            if (Input.IsKeyDown(Keys.Space))
                _physics.AddForce(pos.WorldMatrix.Up * delta * ACCELERATION_SPEED);
            if (Input.IsKeyDown(Keys.C))
                _physics.AddForce(pos.WorldMatrix.Down * delta * ACCELERATION_SPEED);

            if (Input.IsKeyDown(Keys.E))
                _physics.AddTorque(pos.WorldMatrix.Forward * delta * ROTATION_SPEED);
            if (Input.IsKeyDown(Keys.Q))
                _physics.AddTorque(pos.WorldMatrix.Backward * delta * ROTATION_SPEED);

        }

        public override void Close()
        {
            base.Close();
            _entity.World.GameFocused -= CenterCursor;
        }
    }
}