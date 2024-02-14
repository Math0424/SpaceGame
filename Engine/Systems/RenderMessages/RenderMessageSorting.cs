using Microsoft.Xna.Framework;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.Engine.Systems.RenderMessages
{
    internal abstract class RenderMessageSorting : RenderMessage
    {
        public Matrix Matrix { get; protected set; }
        public RenderMessageSorting(RenderMessageType type) : base(type)
        {
        }
    }
}
