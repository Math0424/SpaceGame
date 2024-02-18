using Microsoft.Xna.Framework;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal abstract partial class HudNode
    {
        public bool Visible { get; set; }
        public HudNode Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public ref Vector2I PositionRef { get => ref _position; }
        public virtual Vector2I Position { 
            get => _position; 
            set => _position = value; 
        }

        public bool UseCursor { get; set; }
        public bool ShareCursor { get; set; }

        public int Padding;
        public virtual Vector2I Bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                UpdateParentAlignment();
            }
        }
        public ParentAlignments ParentAlignment
        {
            get { return _parentAlignments; }
            set
            {
                _parentAlignments = value;
                UpdateParentAlignment();
            }
        }
        public SizeAlignments SizeAlignment
        {
            get => _sizeAlignments;
            set
            {
                _sizeAlignments = value;
                UpdateSizeAlignment();
            }
        }

        private ParentAlignments _parentAlignments;
        private SizeAlignments _sizeAlignments;
        private Vector2I _bounds;

        public bool InputEnabled { get; set; }
        public IReadOnlyList<HudNode> Children => _children;
        public float zOffset { get; set; }

        private Vector2I _position;
        private List<HudNode> _children;
        private bool _registered;
        protected HudNode _parent;
        protected string _renderTarget;

        public HudNode(Vector2I bounds, string renderTarget)
        {
            Visible = true;
            _children = new List<HudNode>();
            Bounds = bounds;
            Position = bounds / 2;
            _renderTarget = renderTarget;
        }

        public HudNode(HudNode parent)
        {
            Visible = true;
            _children = new List<HudNode>();
            Parent = parent;
            
            ShareCursor = false;
            Visible = true;
            Bounds = new Vector2I(10, 10);

            Position = Parent.Position;

            Parent.AddChild(this);
            _renderTarget = parent._renderTarget;
        }

        public void AddChild(HudNode element)
        {
            if (element._registered)
                throw new InvalidOperationException("Element already added to another HudNode");
            element._registered = true;
            _children.Add(element);
        }

        public void RemoveChild(HudNode element)
        {
            if (element._registered && !_children.Contains(element))
                throw new InvalidOperationException("Element cannot be removed, different HudNode is owner");
            element._registered = false;
            _children.Remove(element);
        }

        public virtual void PreLayout(bool force)
        {
            if (Visible || force)
            {
                Layout();
                foreach (var x in Children)
                    x.PreLayout(force);
            }
        }

        public virtual void PreDraw(float deltaTime)
        {
            if (Visible)
            {
                Draw(deltaTime);
                foreach (var x in Children)
                    x.PreDraw(deltaTime);
            }
        }

        public virtual void PreHandleInput(ref HudInput input)
        {
            if (Visible)
            {
                foreach (var x in Children)
                    x.PreHandleInput(ref input);
                if (!input.Captured)
                    HandleInput(ref input);
            }
        }

        protected void UpdateSizeAlignment()
        {
            if (Parent == null)
                return;

            Vector2I size = Bounds;
            if ((_sizeAlignments & SizeAlignments.Width) == SizeAlignments.Width)
                size.X = Parent.Bounds.X;
            if ((_sizeAlignments & SizeAlignments.Height) == SizeAlignments.Height)
                size.Y = Parent.Bounds.Y;
            Bounds = size;
        }

        protected void UpdateParentAlignment()
        {
            if (Parent == null)
                return;

            Vector2I newPos = Position;
            if ((_parentAlignments & ParentAlignments.Center) == ParentAlignments.Center)
            {
                Position = Parent.PositionRef;
                return;
            }

            int innerV = (_parentAlignments & ParentAlignments.InnerV) == ParentAlignments.InnerV ? -1 : 1;
            int innerH = (_parentAlignments & ParentAlignments.InnerH) == ParentAlignments.InnerH ? -1 : 1;

            if (_parentAlignments.HasFlag(ParentAlignments.Top))
            {
                newPos.Y = Parent.PositionRef.Y - (Parent.Bounds.Y / 2) - Padding * innerH;
                newPos.Y -= (Bounds.Y / 2) * innerH;
            }
            if (_parentAlignments.HasFlag(ParentAlignments.Bottom))
            {
                newPos.Y = Parent.PositionRef.Y + (Parent.Bounds.Y / 2) + Padding * innerH;
                newPos.Y += (Bounds.Y / 2) * innerH;
            }
            if (_parentAlignments.HasFlag(ParentAlignments.Left))
            {
                newPos.X = Parent.PositionRef.X - (Parent.Bounds.X / 2) - Padding * innerV;
                newPos.X -= (Bounds.X / 2) * innerV;
            }
            if (_parentAlignments.HasFlag(ParentAlignments.Right))
            {
                newPos.X = Parent.PositionRef.X + (Parent.Bounds.X / 2) + Padding * innerV;
                newPos.X += (Bounds.X / 2) * innerV;
            }
            Position = newPos;
        }

        public abstract void HandleInput(ref HudInput input);
        public abstract void Layout();
        public abstract void Draw(float deltaTime);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void DrawSprite(string sprite, Vector2I pos, Vector2I bounds, float depth)
        {
            Rectangle rec = new Rectangle(pos.X - bounds.X / 2, pos.Y - bounds.Y / 2, bounds.X, bounds.Y);
            Render.EnqueueMessage(new RenderMessageDrawSprite(sprite, rec, depth, RenderTarget: _renderTarget));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void DrawColoredSprite(string sprite, Vector2I pos, Vector2I bounds, float depth, Color color)
        {
            Rectangle rec = new Rectangle(pos.X - bounds.X / 2, pos.Y - bounds.Y / 2, bounds.X, bounds.Y);
            Render.EnqueueMessage(new RenderMessageDrawColoredSprite(sprite, rec, depth, color, RenderTarget: _renderTarget));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void DrawText(string font, string text, float scale, float depth, Vector2I pos, Color color, TextDrawOptions options = TextDrawOptions.Left)
        {
            Render.EnqueueMessage(new RenderMessageDrawText(font, text, scale, depth, pos, color, options, RenderTarget: _renderTarget));
        }

    }
}
