using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.Engine.Components
{

    internal class MeshAnimationComponent : EntityUpdateComponent
    {
        private Dictionary<string, AnimationSet[]> _animationSets;

        private List<AnimationSet> _animation;

        struct AnimationSet
        {
            public string MeshName;
            public int Frame;
            public Matrix Rotation;
            public Vector3 Position;
            public Vector3 Scale;
        }

        public MeshAnimationComponent()
        {
            _animationSets = new Dictionary<string, AnimationSet[]>();
            _animation = new List<AnimationSet>();
            IsActive = true;
        }

        public void LoadAnimation(string name, string path)
        {
            List<AnimationSet> animation = new List<AnimationSet>();
            string content = File.ReadAllText(Path.Combine(_entity.World.Game.Content.RootDirectory, path));
            string[] lines = content.Split("\n");
            foreach (var x in lines)
            {
                if (x.Trim().Length != 0)
                {
                    string[] args = x.Split(':');
                    Vector3 rotation = -new Vector3(float.Parse(args[8]), float.Parse(args[10]), float.Parse(args[9]));
                    animation.Add(new AnimationSet()
                    {
                        MeshName = args[0],
                        Frame = int.Parse(args[1]),
                        Position = new Vector3(float.Parse(args[2]), float.Parse(args[4]), float.Parse(args[3])),
                        Rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z)) *
                                    Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) *
                                    Matrix.CreateRotationX(MathHelper.ToRadians(-rotation.X)),
                        Scale = new Vector3(float.Parse(args[5]), float.Parse(args[7]), float.Parse(args[6])) * 2f,
                    });
                }
            }
            _animationSets[name] = animation.ToArray();
        }

        public void PlayAnimation(string name)
        {
            _animation.AddRange(_animationSets[name]);
        }

        public void StopAllAnimations()
        {

        }

        public override void Update(GameTime deltaTime)
        {

        }
    }
}
