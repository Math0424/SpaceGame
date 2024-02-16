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
        public TextDrawOptions TextOptions;

        public HudText(HudNode parent) : base(parent)
        {
            TextColor = Color.Black;
            TextScale = 1;
            Text = "HudText";
            TextOptions = TextDrawOptions.Centered;
        }

        public override void Draw(float deltaTime)
        {
            if (Text != null)
                DrawText("Fonts/Debug", Text, TextScale, zOffset, Position - TextOffset, TextColor, TextOptions);
        }

        public override void HandleInput(ref HudInput input)
        {
        }

        public override void Layout()
        {
        }
    }
}
