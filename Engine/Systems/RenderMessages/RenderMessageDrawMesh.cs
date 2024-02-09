using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{

    internal class RenderMessageDrawMesh : RenderMessage
    {
        public string Model { get; private set; }
        public Matrix Matrix { get; private set; }
        public RenderMessageDrawMesh(string model, Matrix transformMatrix) : base(RenderMessageType.DrawBasicMesh)
        {
            this.Model = model;
            this.Matrix = transformMatrix;
        }
    }
}
