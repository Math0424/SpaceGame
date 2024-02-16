using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal class HudTextButton : HudButton
    {
        public string Text
        {
            get => _hudText.Text;
            set => _hudText.Text = value;
        }
        public Color TextColor
        {
            get => _hudText.TextColor;
            set => _hudText.TextColor = value;
        }

        private HudText _hudText;

        public HudTextButton(HudNode parent) : base(parent)
        {
            _hudText = new HudText(this)
            {
                Text = "HudTextButton",
                SizeAlignment = SizeAlignments.Both
            };
        }

        public override void Layout()
        {
            _hudText.ParentAlignment = ParentAlignments.Center;
            _hudText.SizeAlignment = SizeAlignments.Both;
        }

    }
}
