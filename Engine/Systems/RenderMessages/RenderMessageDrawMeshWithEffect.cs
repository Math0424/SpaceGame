using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.RenderMessages
{

    internal class RenderMessageDrawMeshWithEffect : RenderMessage
    {
        public string Model { get; private set; }
        public string Effect { get; private set; }
        public Matrix Matrix { get; private set; }
        public RenderMessageDrawMeshWithEffect(string model, Matrix transformMatrix, string effect) : base(RenderMessageType.DrawMeshWithEffect)
        {
            this.Model = model;
            this.Matrix = transformMatrix;
            this.Effect = effect;
        }
    }
}
