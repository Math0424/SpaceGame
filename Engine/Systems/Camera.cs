using Microsoft.Xna.Framework;
using Project1.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems
{
    internal class Camera : SystemComponent
    {
        public ref BoundingFrustum Frustum => ref _frustum;
        public ref Matrix ViewMatrix => ref _viewMatrix;
        public ref Matrix WorldMatrix => ref _worldMatrix;
        public ref Matrix ProjectionMatrix => ref _projectionMatrix;
        public float FOV { get; private set; }

        private BoundingFrustum _frustum;
        private Matrix _worldToScreen;
        private Matrix _viewMatrix;
        private Matrix _worldMatrix;
        private Matrix _projectionMatrix;
        private float _aspectRatio;
        private int _height;
        private int _width;
        private float _nearPlane;
        private float _farPlane;

        public Vector3 Right => _worldMatrix.Right;
        public Vector3 Left => _worldMatrix.Left;
        public Vector3 Forward => _worldMatrix.Forward;
        public Vector3 Backward => _worldMatrix.Backward;
        public Vector3 Up => _worldMatrix.Up;
        public Vector3 Down => _worldMatrix.Down;
        public Vector3 Translation => _worldMatrix.Translation;

        private bool _isOrthographic;

        public Camera()
        {
            _height = 480;
            _width = 800;
            _nearPlane = 0.1f;
            _farPlane = 800f;
            SetViewMatrix(Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY));
            SetupProjection(_width, _height, 1);
        }

        public Camera SetupProjection(int width, int height, float FOV)
        {
            _isOrthographic = false;
            _aspectRatio = (float)width / height;
            this._height = height;
            this._width = width;
            this.FOV = FOV;

            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), _aspectRatio, _nearPlane, _farPlane);
            return this;
        }

        public Camera SetupProjection(float FOV)
        {
            SetupProjection(_width, _height, FOV);
            return this;
        }

        public Camera SetupOrthographic(float width, float height, float nearPlane, float farPlane)
        {
            _isOrthographic = true;
            _nearPlane = nearPlane;
            _farPlane = farPlane;
            _projectionMatrix = Matrix.CreateOrthographic(width, height, _nearPlane, _farPlane);
            return this;
        }

        public void SetViewMatrix(Matrix matrix)
        {
            _viewMatrix = matrix;
            Matrix.Invert(ref _viewMatrix, out _worldMatrix);
            _worldToScreen = _viewMatrix * _projectionMatrix;
            _frustum = new BoundingFrustum(_worldToScreen);
        }

        public void SetWorldMatrix(Matrix matrix)
        {
            _worldMatrix = matrix;
            Matrix.Invert(ref _worldMatrix, out _viewMatrix);
            _worldToScreen = _viewMatrix * _projectionMatrix;
            _frustum = new BoundingFrustum(_worldToScreen);
        }

        public bool IsInFrustum(Vector3 point)
        {
            return _frustum.Contains(point) == ContainmentType.Contains;
        }

        public bool IsInFrustum(ref BoundingBox box)
        {
            return _frustum.Intersects(box);
        }

        public void SetFOV(float fov)
        {
            SetupProjection(_width, _height, Math.Clamp(fov, 1, 179));
        }

        public Vector3 WorldToScreen(Vector3 worldPos)
        {
            var pos = Vector4.Transform(new Vector4(worldPos, 1), _worldToScreen);
            pos.X /= pos.W;
            pos.Y /= pos.W;
            return new Vector3((float)((pos.X + 1) * 0.5 * _width), (float)((1 - pos.Y) * 0.5 * _height), pos.W / _farPlane);
        }

        // TODO : fix this
        public Vector3 ScreenToWorld(ref Vector3 screenPos, float depth)
        {
            Vector4 pos = Vector4.Transform(new Vector4(screenPos.X, screenPos.Y, depth, 1), Matrix.Invert(_worldToScreen));
            pos.X /= pos.W;
            pos.Y /= pos.W;
            pos.Z /= pos.W;
            return new Vector3(pos.X, pos.Y, pos.Z);
        }

        public override void Update(GameTime delta)
        {

        }
    }


}
