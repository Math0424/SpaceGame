using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Systems.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.Engine.Systems.GUI
{
    internal class HudSlider : HudNode
    {
        public Color ScrollbarColor
        {
            get => _slider.Color;
            set => _slider.Color = value;
        }
        public Color ScrollbarHoverColor
        {
            get => _slider.HoverColor;
            set
            {
                _slider.HoverColor = value;
                _slider.PressedColor = value;
            }
        }

        public Color BackgroundColor;

        public int ScrollbarWidth
        {
            get => _slider.Bounds.X;
            set
            {
                var bounds = _slider.Bounds;
                bounds.X = Math.Min(value, Bounds.X);
                bounds.Y = Bounds.Y;
                _slider.Bounds = bounds;
            }
        }
        public float ScrollbarPosition;

        private HudButton _slider;
        private bool _selected;

        public override Vector2I Bounds { 
            get => base.Bounds;
            set { 
                base.Bounds = value;
                if (_slider != null)
                    ScrollbarWidth = ScrollbarWidth;
            } 
        }

        public HudSlider(HudNode parent) : base(parent)
        {
            BackgroundColor = Color.White;
            ScrollbarPosition = 0.5f;
            _slider = new HudButton(this)
            {
                SizeAlignment = SizeAlignments.Height,
            };
            ScrollbarWidth = 20;
            ScrollbarHoverColor = Color.Gray;
            ScrollbarColor = Color.DimGray;
        }

        public override void Draw(float deltaTime)
        {
            DrawColoredSprite("Textures/GUI/ColorableSprite", Position, Bounds, zOffset + 0.01f, BackgroundColor);
        }

        public override void HandleInput(ref HudInput input)
        {
            if (_slider.IsPressed)
                _selected = true;

            if (_selected)
            {
                int left = Position.X - (Bounds.X / 2) + (ScrollbarWidth / 2);
                float x = Math.Clamp(input.Location.X, left, Position.X + (Bounds.X / 2) - (ScrollbarWidth / 2));
                ScrollbarPosition = (x - left) / (Bounds.X - ScrollbarWidth);
            }

            if (Input.IsNewMouseUp(Input.MouseButtons.LeftButton) || Input.IsNewMouseUp(Input.MouseButtons.RightButton))
                _selected = false;
        }

        public override void Layout()
        {
            int left = Position.X - (Bounds.X / 2) + (ScrollbarWidth / 2);
            var sliderPos = _slider.Position;
            sliderPos.Y = Position.Y;
            sliderPos.X = left + (int)((Bounds.X - ScrollbarWidth) * ScrollbarPosition);
            _slider.Position = sliderPos;
        }
    }
}
