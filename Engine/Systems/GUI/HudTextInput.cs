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
        public int MaxCharCount;
        public string Text
        {
            get => _text.Text;
            set
            {
                _text.Text = value;
                if (value.Length == 0)
                    _text.Text = EmptyText;
            }
        }

        public string EmptyText
        {
            get => _emptyText;
            set {
                if (_text != null && _text.Text == _emptyText)
                    _text.Text = value;
                _emptyText = value;
            }
        }
        public Color EmptyTextColor;
        public Color BackgroundColor;
        public Color TextColor
        {
            get => _textColor; 
            set => _text.TextColor = _textColor;
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
                    _text.UpdateParentAlignment();
            }
        }

        private Color _textColor;
        private HudText _text;
        private bool _capturingText;
        private string _emptyText;

        public HudTextInput(HudNode parent) : base(parent)
        {
            _textColor = Color.Black;
            EmptyText = "Enter Text";
            BackgroundColor = Color.White;
            Bounds = new Vector2I(200, 25);
            EmptyTextColor = Color.SlateGray;
            MaxCharCount = 20;
            _text = new HudText(this)
            {
                Text = EmptyText,
                SizeAlignment = SizeAlignments.Height,
                ParentAlignment = ParentAlignments.Left | ParentAlignments.Top | ParentAlignments.Inner,
                TextAlignment = TextDrawOptions.Left | TextDrawOptions.CenteredH,
            };
        }

        public override void Draw(float deltaTime)
        {
            if (_text.Text == EmptyText)
                _text.TextColor = EmptyTextColor;
            else
                _text.TextColor = _textColor;

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
                if (_text.Text.Length == 0)
                    _text.Text = EmptyText;
            }

            if (_capturingText)
            {
                if (_text.Text == EmptyText)
                    _text.Text = "";

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

                    if (x >= Keys.NumPad0 && x <= Keys.NumPad9)
                        _text.Text += (char)((int)x - 48);

                    if (x == Keys.Back && _text.Text.Length != 0)
                        _text.Text = _text.Text.Remove(_text.Text.Length - 1);

                    if (_text.Text.Length > MaxCharCount)
                        _text.Text = _text.Text.Remove(MaxCharCount);
                }

            }

        }

        public override void Layout()
        {

        }
    }
}
