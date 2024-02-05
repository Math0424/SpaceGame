using Project1.Engine;
using Project1.Engine.Systems.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.MyGame.EngineComponents
{
    internal class GolfingGUI : HudCore
    {
        public string WorldName
        {
            set => _text.Text = value;
        }
        public string StrokeCount
        {
            set => _text2.Text = value;
        }
        private HudText _text;
        private HudText _text2;

        public GolfingGUI(HudRoot root) : base(root)
        {
            Visible = true;

            var corner = new HudElement(this)
            {
                Padding = 3,
                Bounds = new Vector2I(200, 50),
                ParentAlignment = ParentAlignments.Top | ParentAlignments.Left | ParentAlignments.Inner | ParentAlignments.Padding
            };
            _text = new HudText(corner)
            {
                SizeAlignment = SizeAlignments.Both,
                Text = "WorldNameHere"
            };

            var corner2 = new HudElement(corner)
            {
                Bounds = new Vector2I(150, 30),
                ParentAlignment = ParentAlignments.Bottom | ParentAlignments.Left | ParentAlignments.InnerV 
            };
            _text2 = new HudText(corner2)
            {
                SizeAlignment = SizeAlignments.Both,
                Text = "Stroke X"
            };
        }
    }
}
