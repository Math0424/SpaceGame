using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageLoadMesh : RenderMessage
    {
        public string Model { get; private set; }
        public RenderMessageLoadMesh(string model) : base(RenderMessageType.LoadMesh)
        {
            this.Model = model;
        }
    }
}
