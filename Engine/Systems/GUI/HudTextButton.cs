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

        public override Vector2I Bounds { 
            get => base.Bounds; 
            set {
                base.Bounds = value;
                if (_hudText != null)
                    _hudText.Bounds = value;
            } 
        }

        public override Vector2I Position {
            get => base.Position;
            set
            {
                base.Position = value;
                if (_hudText != null)
                    _hudText.ParentAlignment = ParentAlignments.Center;
            } 
        }

        private HudText _hudText;

        public HudTextButton(HudNode parent) : base(parent)
        {
            _hudText = new HudText(this)
            {
                Text = "HudTextButton",
                SizeAlignment = SizeAlignments.Both,
                ParentAlignment = ParentAlignments.Center,
            };
        }

        public override void Layout()
        {

        }

    }
}
