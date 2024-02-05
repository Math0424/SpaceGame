using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal class HudCore : HudElement
    {
        public HudSystem Root { get; private set; }

        public HudCore(HudRoot root) : base(root)
        {
            _core = this;
            Root = root.System;
            Position = Root.ScreenCenter;
            Bounds = new Vector2I(Position.X * 2, Position.Y * 2);
        }

        public override void Draw(float deltaTime)
        {

        }

        public override void Layout()
        {

        }
    }
}
