using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Components
{
    internal class PositionComponent : EntityComponent
    {
        // avoid passing struct clones, use refs
        public ref Matrix TransformMatrix
        {
            get
            {
                if (_dirtyTransform)
                    UpdateTransform();
                return ref _transformMatrix;
            }
        }
        public ref Matrix LocalMatrix => ref _localMatrix;
        public ref Matrix WorldMatrix => ref _worldMatrix;
        public Vector3 Position => _worldMatrix.Translation;

        private bool _dirtyTransform;
        private Matrix _transformMatrix;
        private Matrix _localMatrix;
        private Matrix _worldMatrix;

        public Action UpdatedTransforms;

        public PositionComponent(Matrix worldMatrix, Matrix localMatrix) : this()
        {
            SetWorldMatrix(worldMatrix);
            SetLocalMatrix(localMatrix);
        }

        public PositionComponent(Matrix worldMatrix) : this()
        {
            _localMatrix = Matrix.Identity;
            SetWorldMatrix(worldMatrix);
        }

        public PositionComponent(Vector3 Pos) : this()
        {
            _localMatrix = Matrix.Identity;
            _worldMatrix = Matrix.Identity;
            _worldMatrix.Translation = Pos;
        }

        public PositionComponent()
        {
            _dirtyTransform = true;
            _localMatrix = Matrix.Identity;
            _worldMatrix = Matrix.Identity;
        }

        public void SetWorldMatrix(Matrix matrix)
        {
            UpdatedTransforms?.Invoke();
            _worldMatrix = matrix;
            _dirtyTransform = true;
        }

        public void SetLocalMatrix(Matrix matrix)
        {
            UpdatedTransforms?.Invoke();
            _localMatrix = matrix;
            _dirtyTransform = true;
        }

        public void SetPosition(Vector3 vector)
        {
            UpdatedTransforms?.Invoke();
            _worldMatrix.Translation = vector;
            _dirtyTransform = true;
        }

        public void Scale(float scale)
        {
            UpdatedTransforms?.Invoke();
            Matrix s = Matrix.CreateScale(scale);
            _localMatrix *= s;
            _dirtyTransform = true;
        }

        private void UpdateTransform()
        {
            _transformMatrix = _localMatrix * _worldMatrix;
        }

    }
}
