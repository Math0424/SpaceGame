using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project1.Engine;
using Project1.Engine.Systems.GUI;
using Project1.Engine.Systems.RenderMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.Engine.Systems.GUI
{
    internal class HudTextInput : HudNode
    {

        public Color BackgroundColor;
        public Color TextColor
        {
            get => _text.TextColor; 
            set => _text.TextColor = value;
        }

        public override Vector2I Bounds
        {
            get => base.Bounds;
            set
            {
                base.Bounds = value;
                if (_text != null)
                    _text.Bounds = value;
            }
        }

        public override Vector2I Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                if (_text != null)
                    _text.ParentAlignment = ParentAlignments.Center;
            }
        }

        private HudText _text;
        private bool _capturingText;

        public HudTextInput(HudNode parent) : base(parent)
        {
            BackgroundColor = Color.White;
            Bounds = new Vector2I(200, 25);
            _text = new HudText(this)
            {
                SizeAlignment = SizeAlignments.Height,
                ParentAlignment = ParentAlignments.Left | ParentAlignments.Top | ParentAlignments.Inner,
                TextAlignment = TextDrawOptions.Left | TextDrawOptions.CenteredH,
            };
        }

        public override void Draw(float deltaTime)
        {
            DrawColoredSprite("Textures/GUI/ColorableSprite", Position, Bounds, zOffset + 1, BackgroundColor);
        }

        public override void HandleInput(ref HudInput input)
        {
            if (input.IsWithinBounds(Position, Bounds))
            {
                if (Input.IsNewMouseDown(Input.MouseButtons.LeftButton))
                    _capturingText = true;
            } 
            else if(Input.IsNewMouseDown(Input.MouseButtons.LeftButton))
            {
                _capturingText = false;
            }

            if (_capturingText)
            {
                Keys[] keys = Input.NewPressedKeys();

                Keys[] modifiers = Input.PressedKeys();
                bool caps = modifiers.Contains(Keys.LeftShift) || modifiers.Contains(Keys.RightShift) || Input.CapsLock;

                foreach(var x in keys)
                {
                    char c = (char)x;
                    if ((c >= '0' && c <= 'Z') || c == ' ')
                    {
                        if (!caps)
                            c = char.ToLower(c);
                        _text.Text += c;
                    }
                    
                    if (x == Keys.Back && _text.Text.Length != 0)
                        _text.Text = _text.Text.Remove(_text.Text.Length - 1);
                }

            }

        }

        public override void Layout()
        {

        }
    }
}
