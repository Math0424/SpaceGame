﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Engine;
using Project1.Engine.Systems;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project2.Engine
{
    struct FrameStatus
    {
        public long TriangleCount;
        public long FrameTickTime;
        public int RenderMessages;
        public long DrawCalls;
        public long TextureCount;
        public int LoadedAssets;
        public int RenderTargets;

        public override string ToString()
        {
            return $"{FrameTickTime / 10000}ms : {TriangleCount} tris : {RenderMessages} messages";
        }
    }

    /// <summary>
    /// Orignally I wanted it to be a component, 
    /// due to the fact that you can only have one graphics device it just
    /// made more sense to make this one of the very few singletons
    /// There is only ever one camera and one rendering at a time... oh well...
    /// </summary>
    internal class Render
    {
        public static Render Instance;
        public static FrameStatus FrameStats { get; private set; }
        public static bool IsReady { get; private set; }
        public static Vector2I ScreenBounds { get; private set; }
        public static Action GraphicsReady;
        public static Action PreDraw;
        public static Action PostDraw;

        public static void Initalize(Game game)
        {
            if (Instance != null)
                throw new Exception("Cannot make more than one render");
            Instance = new Render(game);
        }

        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _graphicsDevice;
        private BasicEffect _basicEffect;
        private SpriteBatch _spriteBatch;
        private SpriteEffect _spriteEffect;
        private Game _game;

        private List<RenderMessage> _renderMessages;
        private Dictionary<string, RenderTarget2D> _renderTargets;

        protected Render(Game game)
        {
            _renderMessages = new List<RenderMessage>();
            _renderTargets = new Dictionary<string, RenderTarget2D>();
            _game = game;

            _graphics = new GraphicsDeviceManager(game);
            _graphics.DeviceCreated += GraphicInit;

            var monitor = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            _graphics.PreferredBackBufferHeight = monitor.Height;
            _graphics.PreferredBackBufferWidth = monitor.Width;
            _graphics.HardwareModeSwitch = false;
            _graphics.IsFullScreen = true;
        }

        private void GraphicInit(object sender, EventArgs e)
        {
            Console.WriteLine($"Graphics Init");

            _graphics = (GraphicsDeviceManager)sender;
            _graphicsDevice = _graphics.GraphicsDevice;

            _basicEffect = new BasicEffect(_graphicsDevice);
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _spriteEffect = new SpriteEffect(_graphicsDevice);

            EnqueueMessage(new RenderMessageLoadFont("Fonts/Debug"));
            EnqueueMessage(new RenderMessageLoadMesh("Models/DebugSphere"));
            EnqueueMessage(new RenderMessageLoadEffect("Shaders/WorldShader"));
            EnqueueMessage(new RenderMessageLoadEffect("Shaders/Skybox"));

            ScreenBounds = new Vector2I(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            EnqueueMessage(new RenderMessageCreateRT("default", ScreenBounds));

            IsReady = true;
            GraphicsReady?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnqueueMessage(RenderMessage message)
        {
            Instance._renderMessages.Add(message);
        }
        
        public void DrawScene(Camera camera, RenderTarget2D target)
        {
            if (!IsReady)
                return;

            PreDraw?.Invoke();

            long timeNow = DateTime.Now.Ticks;
            ConsumeMessages();

            ProcessLoadMessages();

            // draw sprites to the render targets
            ProcessSpriteMessages();

            _graphicsDevice.SetRenderTarget(target);
            _graphicsDevice.Clear(Color.CornflowerBlue);

            // draw our 3d stuff
            ProcessDrawMessages(camera);
            ProcessOtherMessages(camera);

            // overlay our default sprite render target to the 3d scene
            OverlaySpriteRenderTarget("default");
            
            FrameStats = new FrameStatus()
            {
                RenderMessages = _renderMessages.Count,
                DrawCalls = _graphicsDevice.Metrics.DrawCount,
                LoadedAssets = _assets.Count,
                TextureCount = _graphicsDevice.Metrics.TextureCount,
                TriangleCount = _graphicsDevice.Metrics.PrimitiveCount,
                FrameTickTime = DateTime.Now.Ticks - timeNow,
                RenderTargets = _renderTargets.Count,
            };

            ClearMessages();
            PostDraw?.Invoke();
        }

        private void ClearMessages()
        {
            _loadMessages.Clear();
            _drawMessages.Clear();
            _spriteMessages.Clear();
            _otherMessages.Clear();
            _renderMessages.Clear();
        }

        List<RenderMessage> _loadMessages = new List<RenderMessage>();
        List<RenderMessage> _drawMessages = new List<RenderMessage>();
        List<RenderMessage> _spriteMessages = new List<RenderMessage>();
        List<RenderMessage> _otherMessages = new List<RenderMessage>();
        private void ConsumeMessages()
        {
            RenderMessage[] arr = _renderMessages.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                RenderMessageType elm = arr[i].Type;
                if ((elm & RenderMessageType.Sorting) != 0)
                    _drawMessages.Add(arr[i]);
                else if ((elm & RenderMessageType.Load) != 0)
                    _loadMessages.Add(arr[i]);
                else if ((elm & RenderMessageType.Depth) != 0)
                    _spriteMessages.Add(arr[i]);
                else
                    _otherMessages.Add(arr[i]);
            }
        }

        private Dictionary<string, object> _assets = new Dictionary<string, object>();
        private void LoadAsset<T>(string path)
        {
            if (!_assets.ContainsKey(path))
                _assets.Add(path, _game.Content.Load<T>(path));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T Asset<T>(string path)
        {
            return (T)_assets[path];
        }

        private void ProcessLoadMessages()
        {
            foreach (var message in _loadMessages)
            {
                switch (message.Type)
                {
                    case RenderMessageType.LoadMesh:
                        LoadAsset<Model>(((RenderMessageLoadMesh)message).Model);
                        break;
                    case RenderMessageType.LoadFont:
                        LoadAsset<SpriteFont>(((RenderMessageLoadFont)message).Font);
                        break;
                    case RenderMessageType.LoadTexture:
                        LoadAsset<Texture2D>(((RenderMessageLoadTexture)message).Texture);
                        break;
                    case RenderMessageType.LoadEffect:
                        LoadAsset<Effect>(((RenderMessageLoadEffect)message).Effect);
                        break;
                    case RenderMessageType.CreateRT:
                        var createRt = ((RenderMessageCreateRT)message);
                        if (!_renderTargets.ContainsKey(createRt.Name))
                            _renderTargets[createRt.Name] = new RenderTarget2D(_graphicsDevice, createRt.Bounds.X, createRt.Bounds.Y);
                        break;
                    case RenderMessageType.DisposeRT:
                        var disposeRt = ((RenderMessageDisposeRT)message);
                        if (_renderTargets.ContainsKey(disposeRt.Name))
                        {
                            _renderTargets[disposeRt.Name].Dispose();
                            _renderTargets.Remove(disposeRt.Name);
                        }
                        break;
                }
            }
        }

        private void OverlaySpriteRenderTarget(string texture)
        {
            _graphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_renderTargets[texture], Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        private void ProcessDrawMessages(Camera camera)
        {
            _graphicsDevice.SetRenderTarget(null);

            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            _graphicsDevice.BlendState = BlendState.AlphaBlend;

            Matrix cameraViewMatrix = camera.ViewMatrix;
            cameraViewMatrix.Translation = Vector3.Zero;

            RenderMessage[] arr = _drawMessages.OrderBy(e => -Vector3.DistanceSquared(((RenderMessageSorting)e).Matrix.Translation, camera.Translation)).ToArray();
            foreach (var message in arr)
            {
                switch (message.Type)
                {
                    case RenderMessageType.DrawMesh:
                        var drawEffectMesh = (RenderMessageDrawMesh)message;
                        var effect = Asset<Effect>("Shaders/WorldShader");
                        effect.Parameters["ViewDir"].SetValue(camera.WorldMatrix.Forward);
                        effect.Parameters["ViewProjection"].SetValue(cameraViewMatrix * camera.ProjectionMatrix);

                        effect.Parameters["DiffuseDirection"].SetValue(Vector3.Down);
                        effect.Parameters["DiffuseColor"].SetValue(new Vector3(255 / 255f, 247 / 255f, 250 / 255f));
                        effect.Parameters["DiffuseIntensity"].SetValue(0.8f);

                        effect.Parameters["AmbientColor"].SetValue(new Vector3(1, 1, 1));
                        effect.Parameters["AmbientIntensity"].SetValue(0.1f);

                        effect.Parameters["Transparency"].SetValue(drawEffectMesh.Transparency);
                        effect.Parameters["Color"].SetValue(drawEffectMesh.Color.ToVector3());

                        if (drawEffectMesh.Model.Texture_CM != null)
                            effect.Parameters["Texture_CM"].SetValue(Asset<Texture2D>(drawEffectMesh.Model.Texture_CM));

                        if (drawEffectMesh.Model.Texture_ADD != null)
                            effect.Parameters["Texture_ADD"].SetValue(Asset<Texture2D>(drawEffectMesh.Model.Texture_ADD));

                        effect.CurrentTechnique = effect.Techniques[1];
                        var model = drawEffectMesh.Model.Model;
                        foreach (ModelMesh mesh in model.Meshes)
                        {
                            for (int i = 0; i < mesh.MeshParts.Count; i++)
                            {
                                ModelMeshPart modelMeshPart = mesh.MeshParts[i];
                                if (modelMeshPart.PrimitiveCount > 0)
                                {
                                    _graphicsDevice.SetVertexBuffer(modelMeshPart.VertexBuffer);
                                    _graphicsDevice.Indices = modelMeshPart.IndexBuffer;

                                    Matrix modelMat = mesh.ParentBone.Transform * drawEffectMesh.Matrix;
                                    modelMat.Translation -= camera.WorldMatrix.Translation;

                                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(modelMat)));
                                    effect.Parameters["World"].SetValue(modelMat);

                                    for (int j = 0; j < effect.CurrentTechnique.Passes.Count; j++)
                                    {
                                        effect.CurrentTechnique.Passes[j].Apply();
                                        _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, modelMeshPart.VertexOffset, modelMeshPart.StartIndex, modelMeshPart.PrimitiveCount);
                                    }
                                }
                            }
                        }
                        break;
                    case RenderMessageType.DrawQuad:
                        var drawQuad = (RenderMessageDrawQuad)message;
                        _basicEffect.VertexColorEnabled = false;
                        _basicEffect.TextureEnabled = true;
                        _basicEffect.Texture = Asset<Texture2D>(drawQuad.Texture);
                        Matrix quadMat = drawQuad.Matrix;
                        quadMat.Translation -= camera.WorldMatrix.Translation;
                        _basicEffect.World = quadMat;
                        _basicEffect.CurrentTechnique.Passes[0].Apply();
                        if (drawQuad.DrawBack)
                            _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _quadVertexPositionTexture, 0, 4, _quadVertexIndices, 0, 4);
                        else
                            _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _quadVertexPositionTexture, 0, 4, _quadVertexIndicesNoBack, 0, 2);
                        _basicEffect.View = cameraViewMatrix;
                        break;
                }
            }
        }

        private void ProcessOtherMessages(Camera camera)
        {
            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = camera.ViewMatrix;
            _basicEffect.Projection = camera.ProjectionMatrix;
            _basicEffect.TextureEnabled = true;
            _basicEffect.VertexColorEnabled = false;
            _basicEffect.CurrentTechnique.Passes[0].Apply();

            foreach (var message in _otherMessages)
            {
                switch (message.Type)
                {
                    case RenderMessageType.DrawLine:
                        var drawLine = (RenderMessageDrawLine)message;
                        _basicEffect.VertexColorEnabled = true;
                        _basicEffect.TextureEnabled = false;
                        _basicEffect.World = Matrix.Identity;
                        _basicEffect.CurrentTechnique.Passes[0].Apply();
                        var coloredLineVertices = new[] {
                            new VertexPositionColor(drawLine.Pos1, drawLine.Color), new VertexPositionColor(drawLine.Pos2, drawLine.Color),
                        };
                        _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, coloredLineVertices, 0, 1);
                        break;
                    case RenderMessageType.DrawBox:
                        var drawBox = (RenderMessageDrawBox)message;
                        _basicEffect.VertexColorEnabled = true;
                        _basicEffect.TextureEnabled = false;
                        _basicEffect.World = drawBox.Matrix;
                        _basicEffect.CurrentTechnique.Passes[0].Apply();
                        _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, _boxVertexPosition, 0, 8, _boxVertexIndices, 0, 12);
                        break;
                    case RenderMessageType.DrawSphere:
                        var drawSphere = (RenderMessageDrawSphere)message;
                        _basicEffect.VertexColorEnabled = true;
                        _basicEffect.TextureEnabled = false;
                        _basicEffect.CurrentTechnique.Passes[0].Apply();
                        Matrix translate = Matrix.CreateScale(drawSphere.Radius * 2) * Matrix.CreateTranslation(drawSphere.Pos);
                        var prevState = _graphicsDevice.RasterizerState;
                        _graphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };
                        Asset<Model>("Models/DebugSphere").Draw(translate, camera.ViewMatrix, camera.ProjectionMatrix);
                        _graphicsDevice.RasterizerState = prevState;
                        break;
                }
            }
        }

        public void ProcessSpriteMessages()
        {
            var depthSpriteBatch = _spriteMessages
                    .OrderBy(e => -((RenderMessageSprite)e).Depth)
                    .GroupBy(e => ((RenderMessageSprite)e).RenderTarget)
                    .ToDictionary(g => g.Key, g => g.ToArray());

            _spriteEffect.CurrentTechnique.Passes[0].Apply();
            _graphicsDevice.SetRenderTarget(_renderTargets["default"]);
            _graphicsDevice.Clear(Color.Transparent);

            foreach (var texture in depthSpriteBatch.Keys)
            {
                RenderMessage[] messages = depthSpriteBatch[texture];
                _graphicsDevice.SetRenderTarget(_renderTargets[texture]);
                _graphicsDevice.Clear(Color.Transparent);
                _spriteBatch.Begin();
                foreach (var sprite in messages)
                {
                    switch (sprite.Type)
                    {
                        case RenderMessageType.DrawSprite:
                            var drawSprite = (RenderMessageDrawSprite)sprite;
                            _spriteBatch.Draw(Asset<Texture2D>(drawSprite.Texture), drawSprite.Rectangle, Color.White);
                            break;
                        case RenderMessageType.DrawColoredSprite:
                            var drawColoredSprite = (RenderMessageDrawColoredSprite)sprite;
                            _spriteBatch.Draw(Asset<Texture2D>(drawColoredSprite.Texture), drawColoredSprite.Rectangle, drawColoredSprite.Color);
                            break;
                        case RenderMessageType.DrawText:
                            var drawText = (RenderMessageDrawText)sprite;
                            SpriteFont font = Asset<SpriteFont>(drawText.Font);
                            if (drawText.DrawOptions == TextDrawOptions.Centered)
                            {
                                Vector2 width = font.MeasureString(drawText.Text);
                                _spriteBatch.DrawString(font, drawText.Text, drawText.Pos, drawText.Color, 0, width / 2, drawText.Scale, SpriteEffects.None, 0);
                            }
                            else
                                _spriteBatch.DrawString(font, drawText.Text, drawText.Pos, drawText.Color, 0, Vector2.Zero, drawText.Scale, SpriteEffects.None, 0);
                            break;
                    }
                }
                _spriteBatch.End();
            }
        }

        private static VertexPositionTexture[] _quadVertexPositionTexture = new[]
        {
            new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, -1)),
            new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, -1)),
            new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 0)),
        };
        private static short[] _quadVertexIndicesNoBack = new short[] { 0, 1, 2, 2, 3, 0 };
        private static short[] _quadVertexIndices = new short[] { 0, 1, 2, 2, 3, 0, 0, 3, 2, 2, 1, 0 };

        private static VertexPosition[] _boxVertexPosition = new[]
        {
            new VertexPosition(new Vector3(-1, 1, 1)),
            new VertexPosition(new Vector3(1, 1, 1)),
            new VertexPosition(new Vector3(1, -1, 1)),
            new VertexPosition(new Vector3(-1, -1, 1)),

            new VertexPosition(new Vector3(-1, 1, -1)),
            new VertexPosition(new Vector3(1, 1, -1)),
            new VertexPosition(new Vector3(1, -1, -1)),
            new VertexPosition(new Vector3(-1, -1, -1)),
        };
        private static short[] _boxVertexIndices = new short[] { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7 };
    }
}