using SDL2;
using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    class GameSettings
    {
        public const string WindowTitle = "MineKart";

        // Size of the rendering surface (what the code will draw to)
        public const int RenderWidth = 640;
        public const int RenderHeight = 480;

        // Size of the window (render surface will scale to this)
        public const int WindowWidth = 1280;
        public const int WindowHeight = 960;

        // Color used to clear the render window
        public static readonly Color RenderClearColor = new Color(0x00, 0x00, 0x00, 0xff);

        public const int FieldOfViewDegrees = 90;

        public const int DrawDistance = 20;

        // Debug text output
        public const string DebugFontFilePath = "Assets\\unispace rg.ttf";
        public const int DebugFontSize = 10;

        public const int SplashScreenTransitionDelay = 1;
        public const string SplashScreenTextureFilePath = "Assets\\splashscreen.png";
        public const string StartScreenTextureFilePath = "Assets\\startscreen.png";
        public const string EndScreenTextureFilePath = "Assets\\endscreen.png";
        public const string PauseScreenTextureFilePath = "Assets\\pausescreen.png";
        public const string RocksTextureFilePath = "Assets\\rocks.png";

        public const double EndScreenDelay = 1.3;
    }
}
