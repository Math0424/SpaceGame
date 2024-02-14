using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using Project2.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.MyGame.EngineComponents
{
    internal class HideWhenClose : EntityUpdateComponent
    {
        private MeshRenderingComponent _render;

        public HideWhenClose()
        {
            IsActive = true;
        }

        public override void Initalize()
        {
            _render = _entity.GetComponent<MeshRenderingComponent>();
        }

        public override void Update(GameTime deltaTime)
        {
            float dist = Vector3.DistanceSquared(_entity.Position.Position, _entity.World.Render.Camera.Translation);
            if (dist < 25)
                _render.Visible = false;
            else
            {
                _render.Visible = true;
                _render.Transparency = Math.Clamp(dist / 3000, 0, 0.5f);
            }
        }
    }
}
