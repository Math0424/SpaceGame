using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageDrawColoredSprite : RenderMessageDepth
    {
        public Rectangle Rectangle { get; private set; }
        public string Texture { get; private set; }
        public Color Color { get; private set; }
        public RenderMessageDrawColoredSprite(string texture, Rectangle rectangle, float depth, Color color) : base(depth, RenderMessageType.DrawColoredSprite)
        {
            this.Texture = texture;
            this.Rectangle = rectangle;
            this.Color = color;
        }
    }
}
