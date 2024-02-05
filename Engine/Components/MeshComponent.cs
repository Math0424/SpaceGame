using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project1.Engine.Systems;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Components
{
    struct ModelInfo
    {
        public Model Model;
        public string Name;
        public int Verticies;
        public BoundingBox BoundingBox;
        public Vector3 ModelCenter;
        public float BoundingSphereRadius;
    }

    internal class MeshComponent : RenderableComponent
    {
        // TODO : possible memory leak if lots unique models are created and then never used again
        private static Dictionary<string, ModelInfo> cache = new Dictionary<string, ModelInfo>();

        public ref ModelInfo Model => ref _info;
        public BoundingBox AABB => _AABB;
        public string ModelName
        {
            get => _info.Name;
            set => SetModel(value);
        }

        private ModelInfo _info;
        private string _modelName;
        private string _effect;
        private BoundingBox _AABB;

        public MeshComponent(string modelname, string effect = null)
        {
            _effect = effect;
            _modelName = modelname;
        }

        private void UpdateAABB()
        {
            var m = _entity.Position.WorldMatrix;
            BoundingBox bb = Model.BoundingBox;
            for (int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    float e = m[i, j] * bb.Min.GetIndex(j);
                    float f = m[i, j] * bb.Max.GetIndex(j);
                    if (e < f)
                    {
                        bb.Min.SetIndex(i, bb.Min.GetIndex(i) + e);
                        bb.Max.SetIndex(i, bb.Max.GetIndex(i) + f);
                    } 
                    else
                    {
                        bb.Min.SetIndex(i, bb.Min.GetIndex(i) + f);
                        bb.Max.SetIndex(i, bb.Max.GetIndex(i) + e);
                    }
                }
            }
            bb.Min += m.Translation;
            bb.Max += m.Translation;
            _AABB = bb;
        }

        public override void Initalize()
        {
            if (_effect != null)
                _entity.World.Render.EnqueueMessage(new RenderMessageLoadEffect(_effect));
            _entity.World.Render.EnqueueMessage(new RenderMessageLoadMesh(_modelName));
            _entity.Position.UpdatedTransforms += UpdateAABB;
            SetModel(_modelName);
        }

        public override void Close()
        {
            _entity.Position.UpdatedTransforms -= UpdateAABB;
        }

        public override bool IsVisible(ref Camera cam)
        {
            Matrix lm = _entity.Position.TransformMatrix;
            Vector3 boundingSphere = Vector3.Transform(new Vector3(_info.BoundingSphereRadius), lm);
            float scale = boundingSphere.Length();//Math.Max(lm.Forward.Length(), Math.Max(lm.Up.Length(), lm.Right.Length()));
            return cam.Frustum.Intersects(new BoundingSphere(_entity.Position.Position, _info.BoundingSphereRadius * scale));
        }

        public override void Draw(RenderingSystem rendering, ref Camera cam)
        {
            if (_effect != null)
                rendering.EnqueueMessage(new RenderMessageDrawMeshWithEffect(_modelName, _entity.Position.TransformMatrix, _effect));
            else
                rendering.EnqueueMessage(new RenderMessageDrawMesh(_modelName, _entity.Position.TransformMatrix));
        }

        private void SetModel(string name)
        {
            _info.Name = name;
            if (cache.ContainsKey(name))
            {
                _info = cache[name];
                return;
            }
            Model model = _entity.World.Game.Content.Load<Model>(name);
            CalculateModelInfo(model, out _info);

            cache[name] = _info;
        }

        private void CalculateModelInfo(Model model, out ModelInfo info)
        {
            Vector3 min = Vector3.Zero, max = Vector3.Zero;
            int verticies = 0;
            foreach(var mesh in model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                    verticies += part.NumVertices;

                int vertexStride = mesh.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
                float[] vertexData = new float[verticies * vertexStride / sizeof(float)];
                mesh.MeshParts[0].VertexBuffer.GetData(vertexData);

                for (int i = 0; i < vertexData.Length; i += vertexStride / sizeof(float))
                {
                    Vector3 pos = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
                    min = Vector3.Min(min, pos);
                    max = Vector3.Max(max, pos);
                }
            }

            info = new ModelInfo()
            {
                Model = model,
                Verticies = verticies,
                ModelCenter = (min + max) / 2,
                BoundingBox = new BoundingBox(min, max),
                BoundingSphereRadius = Vector3.Distance(min, max),
            };
        }

    }
}
