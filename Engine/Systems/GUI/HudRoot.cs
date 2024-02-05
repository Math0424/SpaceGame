using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    class HudRoot : HudNode
    {
        public HudSystem System;
        public HudRoot(HudSystem system)
        {
            this.System = system;
        }

        public override void Draw(float deltaTime)
        {

        }

        public override void HandleInput(ref HudInput input)
        {

        }

        public override void Layout()
        {

        }
    }
}
