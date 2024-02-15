using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageDrawSprite : RenderMessageSprite
    {
        public Rectangle Rectangle { get; private set; }
        public string Texture { get; private set; }
        public RenderMessageDrawSprite(string texture, Rectangle rectangle, float depth, string RenderTarget = null) : base(depth, RenderTarget, RenderMessageType.DrawSprite)
        {
            this.Texture = texture;
            this.Rectangle = rectangle;
        }
    }
}
