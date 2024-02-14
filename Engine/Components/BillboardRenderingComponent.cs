using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Engine.Systems;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Components
{
    public enum BillboardOption
    {
        CameraFacing,
        EntityFacing,
    }

    internal class BillboardRenderingComponent : RenderableComponent
    {
        private string _assetName;
        private BillboardOption _option;

        public BillboardRenderingComponent(string assetName, BillboardOption option = BillboardOption.CameraFacing)
        {
            _option = option;
            _assetName = assetName;
        }

        public override void Initalize()
        {
            _entity.World.Render.EnqueueMessage(new RenderMessageLoadTexture(_assetName));
        }

        public override void Draw(RenderingSystem rendering, ref Camera cam)
        {
            Matrix mat;
            switch (_option)
            {
                case BillboardOption.CameraFacing:
                    mat = cam.WorldMatrix;
                    mat.Translation = _entity.Position.Position;
                    rendering.EnqueueMessage(new RenderMessageDrawQuad(_assetName, false, mat));
                    break;
                case BillboardOption.EntityFacing:
                    mat = _entity.Position.TransformMatrix;
                    rendering.EnqueueMessage(new RenderMessageDrawQuad(_assetName, true, mat));
                    break;
            }
        }

        // public override void DebugDraw(ref SpriteBatch batch, ref GraphicsDevice graphics, ref Camera cam)
        // {
        //     var pos = _entity.Position; 
        //     Vector3 posx = pos.Position;
        //     Vector3 screen = cam.WorldToScreen(ref posx);
        //     batch.DrawString(_font, $"Billboard\nID: {_entity.Id}\nTxt: {_assetName}", new Vector2(screen.X, screen.Y), Color.Black, 0, Vector2.Zero, 1 - screen.Z, default, 0);
        // }

        public override bool IsVisible(ref Camera cam)
        {
            return true;// cam.IsInFrustum(_entity.Position.Position);
        }
    }
}
