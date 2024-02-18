using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal class HudButton : HudNode
    {
        public Color HoverColor;
        public Color PressedColor;
        public Color Color;

        public bool IsPressed { get; protected set; }
        public Action<object> OnHovered;
        public Action<object> OnRightClicked;
        public Action<object> OnLeftClicked;

        protected bool _isHovered;
        private bool _wasWithinBounds;

        public HudButton(HudNode parent) : base(parent) 
        {
            Color = Color.White;
            HoverColor = Color.Gray;
            PressedColor = Color.DarkSlateGray;
        }

        public override void HandleInput(ref HudInput input)
        {
            if (input.IsWithinBounds(Position, Bounds))
            {
                if (!_wasWithinBounds)
                {
                    _wasWithinBounds = true;
                    OnHovered?.Invoke(this);
                }

                _isHovered = true;
                if (Input.IsNewMouseDown(Input.MouseButtons.LeftButton) || Input.IsNewMouseDown(Input.MouseButtons.RightButton))
                {
                    IsPressed = true;
                }

                if (IsPressed)
                {
                    if (Input.IsNewMouseUp(Input.MouseButtons.LeftButton))
                    {
                        OnLeftClicked?.Invoke(this);
                        IsPressed = false;
                    }
                    else if(Input.IsNewMouseUp(Input.MouseButtons.RightButton))
                    {
                        OnRightClicked?.Invoke(this);
                        IsPressed = false;
                    }
                }
            }
            else
            {
                _isHovered = false;
                IsPressed = false;
                _wasWithinBounds = false;
            }
        }

        public override void Draw(float deltaTime)
        {
            if (IsPressed)
                DrawColoredSprite("Textures/GUI/ColorableSprite", Position, Bounds, zOffset, PressedColor);
            else if(_isHovered)
                DrawColoredSprite("Textures/GUI/ColorableSprite", Position, Bounds, zOffset, HoverColor);
            else
                DrawColoredSprite("Textures/GUI/ColorableSprite", Position, Bounds, zOffset, Color);
        }

        public override void Layout()
        {
            
        }
    }
}
