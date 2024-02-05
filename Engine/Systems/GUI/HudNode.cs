using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal abstract partial class HudNode
    {
        public virtual bool Visible { get; set; }
        public virtual HudNode Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public ref Vector2I PositionRef { get => ref _position; }
        public Vector2I Position { 
            get => _position; 
            set => _position = value; 
        }

        public bool InputEnabled { get; set; }
        public IReadOnlyList<HudNode> Children => _children;
        public float zOffset { get; set; }

        private Vector2I _position;
        private List<HudNode> _children;
        private bool _registered;
        protected HudNode _parent;

        public HudNode()
        {
            _children = new List<HudNode>();
        }

        public HudNode(HudNode parent)
        {
            Parent = parent;
            _children = new List<HudNode>();
            Parent?.AddChild(this);
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

        public abstract void HandleInput(ref HudInput input);
        public abstract void Layout();
        public abstract void Draw(float deltaTime);
    }
}
