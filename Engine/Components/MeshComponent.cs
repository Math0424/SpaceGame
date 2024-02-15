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
        public string Texture_CM;
        public string Texture_ADD;

        public string Name;
        public int Verticies;
        public BoundingBox BB;
        public Vector3 Center;
        public float BoundingSphereRadius;
    }

    internal class MeshComponent : EntityComponent
    {
        // TODO : possible memory leak if lots unique models are created and then never used again
        private static Dictionary<string, ModelInfo> cache = new Dictionary<string, ModelInfo>();

        public ref ModelInfo Model => ref _info;
        public BoundingBox AABB => _AABB;

        private ModelInfo _info;
        private BoundingBox _AABB;

        public ref float BoundingSphere => ref _info.BoundingSphereRadius;

        public MeshComponent(string modelname, string texture_cm = null, string texture_add = null)
        {
            _info = new ModelInfo()
            {
                Name = modelname,
                Texture_CM = texture_cm,
                Texture_ADD = texture_add,
            };
        }

        private void UpdateAABB()
        {
            var m = _entity.Position.TransformMatrix;
            BoundingBox bb = Model.BB;
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
            if (_info.Texture_ADD != null)
                _entity.World.Render.EnqueueMessage(new RenderMessageLoadTexture(_info.Texture_ADD));
            if (_info.Texture_CM != null)
                _entity.World.Render.EnqueueMessage(new RenderMessageLoadTexture(_info.Texture_CM));

            _entity.World.Render.EnqueueMessage(new RenderMessageLoadMesh(_info.Name));
            _entity.Position.UpdatedTransforms += UpdateAABB;
            SetModel(_info.Name);
        }

        public override void Close()
        {
            _entity.Position.UpdatedTransforms -= UpdateAABB;
        }

        private void SetModel(string name)
        {
            if (cache.ContainsKey(name))
            {
                _info = cache[name];
                return;
            }
            ModelInfo modelInfo;
            Model model = _entity.World.Game.Content.Load<Model>(name);
            CalculateModelInfo(model, out modelInfo);

            modelInfo.Name = name;
            cache[name] = modelInfo;
            _info = modelInfo;
        }

        private void CalculateModelInfo(Model model, out ModelInfo info)
        {
            Vector3 min = Vector3.Zero, max = Vector3.Zero;
            int verticies = 0;
            foreach(var mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    int vertexDataSize = vertexBufferSize / sizeof(float);
                    float[] vertexData = new float[vertexDataSize];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    for (int i = 0; i < vertexDataSize; i += vertexStride / sizeof(float))
                    {
                        Vector3 vertex = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
                        min = Vector3.Min(min, vertex);
                        max = Vector3.Max(max, vertex);
                    }
                }
            }

            info = new ModelInfo()
            {
                Name = _info.Name,
                Texture_ADD = _info.Texture_ADD,
                Texture_CM = _info.Texture_CM,
                Model = model,
                Verticies = verticies,
                Center = (min + max) / 2,
                BB = new BoundingBox(min, max),
                BoundingSphereRadius = Vector3.Distance(min, max),
            };
        }

    }
}
