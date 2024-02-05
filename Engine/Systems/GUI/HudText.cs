using Microsoft.Xna.Framework;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal class HudText : HudElement
    {
        public Color TextColor;
        public string Text;
        public float TextScale;
        public Vector2I TextOffset;
        public TextDrawOptions TextOptions;

        public HudText(HudElement parent) : base(parent)
        {
            TextColor = Color.Black;
            TextScale = 1;
            Text = "HudText";
            TextOptions = TextDrawOptions.Centered;
        }

        public override void Draw(float deltaTime)
        {
            if (Text != null)
                _core.Root.DrawText("Fonts/Debug", Text, TextScale, zOffset, Position - TextOffset, TextColor, TextOptions);
        }
    }
}
