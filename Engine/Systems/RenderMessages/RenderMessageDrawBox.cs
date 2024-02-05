using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageDrawBox : RenderMessage
    {
        public Matrix Matrix { get; private set; }
        public RenderMessageDrawBox(Matrix boxMatrix) : base(RenderMessageType.DrawBox)
        {
            Matrix = boxMatrix;
        }
    }
}
