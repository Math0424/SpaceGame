using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageDrawSprite : RenderMessageDepth
    {
        public Rectangle Rectangle { get; private set; }
        public string Texture { get; private set; }
        public RenderMessageDrawSprite(string texture, Rectangle rectangle, float depth) : base(depth, RenderMessageType.DrawSprite)
        {
            this.Texture = texture;
            this.Rectangle = rectangle;
        }
    }
}
