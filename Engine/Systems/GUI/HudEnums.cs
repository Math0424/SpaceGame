using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine.Systems.GUI
{
    internal enum ParentAlignments
    {
        None = 0,
        InnerV = 1 << 0,
        InnerH = 1 << 1,

        Top = 1 << 2,
        Bottom = 1 << 3,
        Left = 1 << 4,
        Right = 1 << 5,

        Center = Top | Bottom | Left | Right,
        Inner = InnerV | InnerH,

        Outer = 1 << 7,
        Padding = 1 << 6,
    }

    internal enum SizeAlignments
    {
        None = 0,
        Width = 1 << 0,
        Height = 1 << 1,
        Both = Width | Height,
    }

}
