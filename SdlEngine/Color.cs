using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public struct Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
        
        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public SDL.SDL_Color ToSdlColor()
        {
            return new SDL.SDL_Color
            {
                r = R,
                g = G,
                b = B,
                a = A
            };
        }

        public static readonly Color Red = new Color(0xff, 0x00, 0x00, 0xff);
        public static readonly Color Green = new Color(0x00, 0xff, 0x00, 0xff);
        public static readonly Color Blue = new Color(0x00, 0x00, 0xff, 0xff);
        public static readonly Color Yellow = new Color(0xff, 0xff, 0x00, 0xff);
        public static readonly Color Magenta = new Color(0xff, 0x00, 0xff, 0xff);
        public static readonly Color Cyan = new Color(0x00, 0xff, 0xff, 0xff);
        public static readonly Color Gray = new Color(0x80, 0x80, 0x80, 0xff);
        public static readonly Color White = new Color(0xff, 0xff, 0xff, 0xff);
        public static readonly Color Black = new Color(0x00, 0x00, 0x00, 0xff);
        public static readonly Color Transparent = new Color(0x00, 0x00, 0x00, 0x00);
    }
}
