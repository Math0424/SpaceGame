using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Engine.Systems;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Components
{
    internal class SpriteRenderingComponent : RenderableComponent
    {
        private string _assetName;

        public SpriteRenderingComponent(string assetName)
        {
            _assetName = assetName;
        }

        public override void Initalize()
        {

        }

        public override void Draw(ref Camera cam)
        {
            var newPos = cam.WorldToScreen(_entity.Position.Position);
            float depth = Vector3.DistanceSquared(_entity.Position.Position, cam.Translation);
            Rectangle r = new Rectangle((int)newPos.X, (int)newPos.Y, (int)(30 * (1 - newPos.Z)), (int)(30 * (1 - newPos.Z)));
            Render.EnqueueMessage(new RenderMessageDrawSprite(_assetName, r, depth));
        }

        // public void DebugDraw(ref SpriteBatch batch, ref GraphicsDevice graphics, ref Camera cam)
        // {
        //     var pos = _entity.Position;
        //     Vector3 posx = pos.Position;
        //     Vector3 screen = cam.WorldToScreen(ref posx);
        //     batch.DrawString(_font, $"Sprite\nID: {_entity.Id}", new Vector2(screen.X, screen.Y), Color.Transparent, 0, Vector2.Zero, 1 - screen.Z, default, 0);
        // }

        public override bool IsVisible(ref Camera cam)
        {
            return cam.IsInFrustum(_entity.Position.Position);
        }
    }
}
