﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Engine.Components;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1.Engine.Systems
{
    internal class RenderingSystem : SystemComponent, IDrawUpdate
    {
        public static GraphicsDeviceManager _graphics;

        public Camera Camera { get; private set; }
        public Vector2I ScreenBounds { get; private set; }
        public Action DoDraw;
        public Action OnGraphicsReady;
        public bool GraphicsReady;

        private GraphicsDevice _graphicsDevice;
        private BasicEffect _basicEffect;
        private SpriteFont _font;
        private SpriteBatch _debugSpriteBatch;
        private SpriteBatch _spriteBatch;
        private SpriteEffect _spriteEffect;

        private GameTime tickTime;
        private bool _debugMode;
        private World _world;
        private Game _game;

        private List<RenderMessage> _renderMessages;

        public RenderingSystem(World world, Game game, Camera camera)
        {
            _game = game;
            Camera = camera;
            _world = world;

            GraphicsReady = false;
            _renderMessages = new List<RenderMessage>();
            _meshes = new Dictionary<string, Model>();
            _fonts = new Dictionary<string, SpriteFont>();
            _textures = new Dictionary<string, Texture2D>();
            _effects = new Dictionary<string, Effect>();

            if (_graphics == null)
            {
                _graphics = new GraphicsDeviceManager(game);
                _graphics.DeviceCreated += GraphicInit;
                _graphics.PreferredBackBufferHeight = 800;
                _graphics.PreferredBackBufferWidth = 800;
            } 
            else
                GraphicInit(_graphics, null);

            world.AddInjectedType(_graphics);
        }

        private void GraphicInit(object sender, EventArgs e)
        {
            Console.WriteLine($"Graphics Init");

            _graphics = (GraphicsDeviceManager)sender;
            _graphicsDevice = _graphics.GraphicsDevice;

            _basicEffect = new BasicEffect(_graphicsDevice);

            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _debugSpriteBatch = new SpriteBatch(_graphicsDevice);

            //_basicEffect.EnableDefaultLighting();
            _basicEffect.LightingEnabled = true;
            _basicEffect.AmbientLightColor = Vector3.One;

            _spriteEffect = new SpriteEffect(_graphicsDevice);

            _font = _world.Game.Content.Load<SpriteFont>("Fonts/Debug");
            EnqueueMessage(new RenderMessageLoadFont("Fonts/Debug"));
            EnqueueMessage(new RenderMessageLoadMesh("Models/DebugSphere"));

            ScreenBounds = new Vector2I(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            Camera.SetupProjection(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 90);
            //Camera.SetupOrthographic(_graphicsDevice.Viewport.Width / 20, _graphicsDevice.Viewport.Height / 20, -50, 50);

            GraphicsReady = true;
            OnGraphicsReady?.Invoke();
        }

        // TODO move draw to a different thread away from the main gameloop
        public void Draw(GameTime delta)
        {
            if (!GraphicsReady)
                return;

            long timeNow = DateTime.Now.Ticks;
            _graphicsDevice.Clear(Color.CornflowerBlue);

            DoDraw?.Invoke();

            int rendering = 0;
            var drawables = _world.GetEntityComponents<RenderableComponent>();
            if (drawables != null)
            {
                // TODO : some sort of spatial partitioning
                // oct-tree or dynamic sectors
                var camera = Camera;
                foreach (var x in drawables)
                {
                    if (x.Visible && x.IsVisible(ref camera))
                    {
                        rendering++;
                        x.Draw(this, ref camera);
                    }
                }
            }

            int renderMessageCount = _renderMessages.Count;
            ProcessRenderMessages();
            _renderMessages.Clear();

            if (_debugMode)
            {
                _debugSpriteBatch.Begin();

                long ticksTaken = (DateTime.Now.Ticks - timeNow) / 10000;
                
                _debugSpriteBatch.DrawString(_font, $"Rendering Debug:\n" +
                    $"Time: {Math.Round(delta.TotalGameTime.TotalMilliseconds / 1000, 2)}s\n" +
                    $"FPS: {Math.Round(delta.ElapsedGameTime.TotalSeconds * 1000, 2)}ms {Math.Round((ticksTaken / delta.ElapsedGameTime.TotalMilliseconds) * 100)}%\n" +
                    $"TPS: {Math.Round(tickTime.ElapsedGameTime.TotalSeconds * 1000, 2)}ms\n" +
                    $"Entities: {_world.EntityCount}\n" +
                    $"Drawn: {rendering}/{drawables?.Count() ?? -1}\n" +
                    $"DrawCalls: {renderMessageCount} / {_graphicsDevice.Metrics.DrawCount}\n" +
                    $"Triangles: {_graphicsDevice.Metrics.PrimitiveCount}\n" +
                    $"Textures: {_graphicsDevice.Metrics.TextureCount}\n" +
                    $"Pos: [{Math.Round(Camera.Translation.X, 2)}, {Math.Round(Camera.Translation.Y, 2)}, {Math.Round(Camera.Translation.Z, 2)}]",
                    new Vector2(0, 0), Color.Yellow);

                _debugSpriteBatch.End();
            }
        }

        private Dictionary<string, Model> _meshes;
        private Dictionary<string, Texture2D> _textures;
        private Dictionary<string, SpriteFont> _fonts;
        private Dictionary<string, Effect> _effects;

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
        private static short[] _boxVertexIndices = new short[] { 0,1, 1,2, 2,3, 3,0,  4,5, 5,6, 6,7, 7,4,  0,4, 1,5, 2,6, 3,7 };

        private void ProcessRenderMessages()
        {
            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = Camera.ViewMatrix;
            _basicEffect.Projection = Camera.ProjectionMatrix;
            _basicEffect.TextureEnabled = true;
            _basicEffect.VertexColorEnabled = false;

            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            _basicEffect.CurrentTechnique.Passes[0].Apply();

            RenderMessage[] arr = _renderMessages.ToArray();
            foreach (var message in arr)
            {
                switch (message.Type)
                {
                    case RenderMessageType.LoadMesh:
                        var loadMesh = (RenderMessageLoadMesh)message;
                        _meshes[loadMesh.Model] = _game.Content.Load<Model>(loadMesh.Model);
                        break;
                    case RenderMessageType.LoadFont:
                        var loadFont = (RenderMessageLoadFont)message;
                        _fonts[loadFont.Font] = _game.Content.Load<SpriteFont>(loadFont.Font);
                        break;
                    case RenderMessageType.LoadTexture:
                        var loadTexture = (RenderMessageLoadTexture)message;
                        _textures[loadTexture.Texture] = _game.Content.Load<Texture2D>(loadTexture.Texture);
                        break;
                    case RenderMessageType.LoadEffect:
                        var loadEffect = (RenderMessageLoadEffect)message;
                        _effects[loadEffect.Effect] = _game.Content.Load<Effect>(loadEffect.Effect);
                        break;
                    case RenderMessageType.DrawMesh:
                        var drawMesh = (RenderMessageDrawMesh)message;
                        _basicEffect.VertexColorEnabled = false;
                        _basicEffect.TextureEnabled = true;
                        _meshes[drawMesh.Model].Draw(drawMesh.Matrix, Camera.ViewMatrix, Camera.ProjectionMatrix);
                        break;
                    case RenderMessageType.DrawMeshWithEffect:
                        var drawEffectMesh = (RenderMessageDrawMeshWithEffect)message;
                        var effect = _effects[drawEffectMesh.Effect];
                        foreach (ModelMesh mesh in _meshes[drawEffectMesh.Model].Meshes)
                        {
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                part.Effect = effect;
                                effect.Parameters["World"].SetValue(drawEffectMesh.Matrix * mesh.ParentBone.Transform);
                                effect.Parameters["CameraPos"].SetValue(Camera.WorldMatrix.Translation);
                                effect.Parameters["View"].SetValue(Camera.ViewMatrix);
                                effect.Parameters["Projection"].SetValue(Camera.ProjectionMatrix);
                            }
                            mesh.Draw();
                        }
                        break;
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
                    case RenderMessageType.DrawQuad:
                        var drawQuad = (RenderMessageDrawQuad)message;
                        _basicEffect.VertexColorEnabled = false;
                        _basicEffect.TextureEnabled = true;
                        _basicEffect.Texture = _textures[drawQuad.Texture];
                        _basicEffect.World = drawQuad.Matrix;
                        _basicEffect.CurrentTechnique.Passes[0].Apply();
                        if (drawQuad.DrawBack)
                            _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _quadVertexPositionTexture, 0, 4, _quadVertexIndices, 0, 4);
                        else
                            _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _quadVertexPositionTexture, 0, 4, _quadVertexIndicesNoBack, 0, 2);
                        _basicEffect.View = Camera.ViewMatrix;
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
                        _meshes["Models/DebugSphere"].Draw(translate, Camera.ViewMatrix, Camera.ProjectionMatrix);
                        _graphicsDevice.RasterizerState = prevState;
                        break;
                }
            }

            RenderMessage[] depthSpriteBatch = _renderMessages.Where(e => 
                                                        e.Type == RenderMessageType.DrawSprite || 
                                                        e.Type == RenderMessageType.DrawText || 
                                                        e.Type == RenderMessageType.DrawColoredSprite)
                                                .OrderBy(e => -((RenderMessageDepth)e).Depth).ToArray();
            _spriteEffect.CurrentTechnique.Passes[0].Apply();
            _spriteBatch.Begin();
            foreach (var sprite in depthSpriteBatch)
            {
                switch(sprite.Type)
                {
                    case RenderMessageType.DrawSprite:
                        var drawSprite = (RenderMessageDrawSprite)sprite;
                        _spriteBatch.Draw(_textures[drawSprite.Texture], drawSprite.Rectangle, Color.White);
                        break;
                    case RenderMessageType.DrawColoredSprite:
                        var drawColoredSprite = (RenderMessageDrawColoredSprite)sprite;
                        _spriteBatch.Draw(_textures[drawColoredSprite.Texture], drawColoredSprite.Rectangle, drawColoredSprite.Color);
                        break;
                    case RenderMessageType.DrawText:
                        var drawText = (RenderMessageDrawText)sprite;
                        if (drawText.DrawOptions == TextDrawOptions.Centered)
                        {
                            Vector2 width = _fonts[drawText.Font].MeasureString(drawText.Text);
                            _spriteBatch.DrawString(_fonts[drawText.Font], drawText.Text, drawText.Pos, drawText.Color, 0, width / 2, drawText.Scale, SpriteEffects.None, 0);
                        }
                        else
                            _spriteBatch.DrawString(_fonts[drawText.Font], drawText.Text, drawText.Pos, drawText.Color, 0, Vector2.Zero, drawText.Scale, SpriteEffects.None, 0);
                        break;
                }
            }
            _spriteBatch.End();

        }

        public void EnqueueMessage(RenderMessage message)
        {
            _renderMessages.Add(message);
        }

        public override void Update(GameTime delta)
        {
            tickTime = delta;
            if (Input.IsNewKeyDown(Keys.F11))
            {
                _debugMode = !_debugMode;
            }
        }

        public override void Close()
        {
            _basicEffect.Dispose();
            _debugSpriteBatch.Dispose();
            _spriteBatch.Dispose();
            _spriteEffect.Dispose();
        }
    }
}
