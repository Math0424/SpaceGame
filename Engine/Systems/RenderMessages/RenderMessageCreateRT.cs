using Project1.Engine;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.Engine.Systems.RenderMessages
{
    internal class RenderMessageCreateRT : RenderMessage
    {
        public string Name { get; private set; }
        public Vector2I Bounds { get; private set; }
        public RenderMessageCreateRT(string Name, Vector2I Bounds) : base(RenderMessageType.CreateRT)
        {
            this.Name = Name;
            this.Bounds = Bounds;
        }
    }
}
