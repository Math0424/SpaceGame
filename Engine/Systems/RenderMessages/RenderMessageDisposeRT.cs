using Project1.Engine;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.Engine.Systems.RenderMessages
{
    internal class RenderMessageDisposeRT : RenderMessage
    {
        public string Name { get; private set; }
        public RenderMessageDisposeRT(string Name) : base(RenderMessageType.DisposeRT)
        {
            this.Name = Name;
        }
    }
}
