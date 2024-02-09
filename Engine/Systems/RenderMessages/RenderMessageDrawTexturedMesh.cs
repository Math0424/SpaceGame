using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{
    internal class RenderMessageDrawTexturedMesh : RenderMessage
    {
        public string Model { get; private set; }
        public string Texture_CM { get; private set; }
        public string Texture_ADD { get; private set; }
        public Matrix Matrix { get; private set; }
        public RenderMessageDrawTexturedMesh(string model, string cm, string add, Matrix transformMatrix) : base(RenderMessageType.DrawMesh)
        {
            this.Model = model;
            this.Matrix = transformMatrix;
            this.Texture_CM = cm;
            this.Texture_ADD = add;
        }
    }
}
