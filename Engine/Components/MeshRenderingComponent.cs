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

        public bool ColorOverride;
        public Color DrawColor;

        private MeshComponent _mesh;

        public MeshRenderingComponent()
        {
            IsActive = true;
        }

        public MeshRenderingComponent(float transparency) : base()
        {
            Transparency = transparency;
        }

        public override void Initalize()
        {
            _mesh = _entity.GetComponent<MeshComponent>();
        }

        public override void Draw(RenderingSystem system, ref Camera cam)
        {
            if (!IsActive)
                return;

            RenderType renderType = RenderType.Default;
            if (ColorOverride)
            {
                renderType = RenderType.OverrideColor;
            }

            if (_mesh.Model.Texture_CM != null && _mesh.Model.Texture_ADD != null)
            {
                renderType = RenderType.ColorMetalAdd;
            }

            system.EnqueueMessage(new RenderMessageDrawMesh(_mesh.Model, renderType, Transparency, DrawColor, _entity.Position.TransformMatrix));
        }

        private void DebugDraw()
        {
            BoundingBox bb = _mesh.AABB;
            Vector3 pos = (bb.Max + bb.Min) / 2;
            Vector3 halfExtents = (bb.Max - bb.Min) / 2;
            Matrix mat = Matrix.CreateScale(halfExtents) * Matrix.CreateTranslation(pos);
            _entity.World.Render.EnqueueMessage(new RenderMessageDrawBox(mat));
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
