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

namespace Project2.Engine.Components
{
    internal class MeshRenderingComponent : RenderableComponent
    {
        public bool IsActive;

        public float Transparency;
        public Color Color;
        private MeshComponent _mesh;

        public MeshRenderingComponent()
        {
            Transparency = 1;
            IsActive = true;
            Color = Color.Transparent;
        }

        public override void Initalize()
        {
            _mesh = _entity.GetComponent<MeshComponent>();
        }

        public override void Draw(ref Camera cam)
        {
            if (!IsActive)
                return;

            Render.EnqueueMessage(new RenderMessageDrawMesh(_mesh.Model, Transparency, Color, _entity.Position.TransformMatrix));
        }

        private void DebugDraw()
        {
            BoundingBox bb = _mesh.AABB;
            Vector3 pos = (bb.Max + bb.Min) / 2;
            Vector3 halfExtents = (bb.Max - bb.Min) / 2;
            Matrix mat = Matrix.CreateScale(halfExtents) * Matrix.CreateTranslation(pos);
            Render.EnqueueMessage(new RenderMessageDrawBox(mat));
        }

        public override bool IsVisible(ref Camera cam)
        {
            if (!IsActive)
                return false;

            Matrix lm = _entity.Position.WorldMatrix;
            Vector3 extents = lm.HalfExtents();
            float scale = _mesh.BoundingSphere * Math.Max(extents.X, Math.Max(extents.Y, extents.Z));
            return cam.Frustum.Intersects(new BoundingSphere(_entity.Position.Position, scale));
        }
    }
}
