using Microsoft.Xna.Framework;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.Engine.Systems.RenderMessages
{
    struct ParticleDef
    {
        public string Texture;
        public int Lifetime;
        public Vector3 Position;
        public Vector3 Velocity;
        public Color Color;
    }

    // TODO : GPU particles
    internal class RenderMessageDrawParticle : RenderMessage
    {
        public ParticleDef[] Particles { private set; get; }
        public RenderMessageDrawParticle(params ParticleDef[] def) : base(RenderMessageType.DrawParticle)
        {
            this.Particles = def;
        }
    }
}
