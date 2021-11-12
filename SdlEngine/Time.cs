using NLog;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class Time
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static double TotalTime { get; private set; }
        public static double DeltaTime { get; private set; }
        public static long TotalFrames { get; private set; }

        private static ulong PerformanceFrequency { get; set; }
        private static ulong PreviousPerformanceCounter { get; set; }

        private static List<double> RecentFrameTimes { get; set; } = new List<double>();

        static Time()
        {
            PerformanceFrequency = SDL.SDL_GetPerformanceFrequency();
            PreviousPerformanceCounter = SDL.SDL_GetPerformanceCounter();
        }

        public static void NextFrame()
        {
            ulong currentPerformanceCounter = SDL.SDL_GetPerformanceCounter();

            DeltaTime = (currentPerformanceCounter - PreviousPerformanceCounter) / (double)PerformanceFrequency;
            TotalTime += DeltaTime;

            TotalFrames++;

            PreviousPerformanceCounter = currentPerformanceCounter;

            RecentFrameTimes.Add(DeltaTime);
            if (RecentFrameTimes.Count > 100)
            {
                double fps = RecentFrameTimes.Count / RecentFrameTimes.Sum();
                Logger.Info($"FPS: {fps:0.000}");
                RecentFrameTimes.Clear();
            }
        }
    }
}
