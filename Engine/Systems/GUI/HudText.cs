using Microsoft.Xna.Framework;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal class HudText : HudNode
    {
        public Color TextColor;
        public string Text;
        public float TextScale;
        public Vector2I TextOffset;
        public TextDrawOptions TextAlignment;
        public string TextFont;

        public HudText(HudNode parent) : base(parent)
        {
            TextFont = "Fonts/Debug";
            TextColor = Color.Black;
            TextScale = 1;
            Text = "HudText";
            TextAlignment = TextDrawOptions.Centered;
        }

        public override void Draw(float deltaTime)
        {
            if (Text != null)
                DrawText(TextFont, Text, TextScale, zOffset, Position - TextOffset, TextColor, TextAlignment);
        }

        public override void HandleInput(ref HudInput input)
        {
        }

        public override void Layout()
        {
        }
    }
}
