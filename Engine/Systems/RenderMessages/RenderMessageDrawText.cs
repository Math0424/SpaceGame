using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    public enum TextDrawOptions
    {
        Default,
        Centered,
    }

    internal class RenderMessageDrawText : RenderMessageSprite
    {
        public string Text { get; private set; }
        public string Font { get; private set; }
        public float Scale { get; private set; }
        public Vector2 Pos { get; private set; }
        public Color Color { get; private set; }
        public TextDrawOptions DrawOptions { get; private set; }
        public RenderMessageDrawText(string font, string text, float scale, float depth, Vector2 pos, Color color, TextDrawOptions drawOptions = TextDrawOptions.Default, string RenderTarget = null) : base(depth, RenderTarget, RenderMessageType.DrawText)
        {
            this.Font = font;
            this.Text = text;
            this.Scale = scale;
            this.Pos = pos;
            this.Color = color;
            DrawOptions = drawOptions;
        }
    }
}
