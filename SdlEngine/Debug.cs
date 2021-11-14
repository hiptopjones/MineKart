using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public static class Debug
    {
        public static List<Action<IntPtr>> FrameActions { get; set; } = new List<Action<IntPtr>>();

        private static Vector3 StartingTextPosition { get; set; } = new Vector3(5, 5, 0);
        private static Vector3 CurrentTextPosition { get; set; } = StartingTextPosition;
        private static Vector3 TextLineOffset { get; set; } = new Vector3(0, 13, 0);
        private static Color DefaultColor { get; set; } = Color.Yellow;

        public static bool IsEnabled { get; set; }

        private static GraphicsManager GraphicsManager { get; set; }

        static Debug()
        {
            GraphicsManager = ServiceLocator.Instance.GetService<GraphicsManager>();
            if (GraphicsManager == null)
            {
                throw new Exception($"Unable to retrieve graphics manager from service locator");
            }
        }

        public static void DrawLine(Vector3 from, Vector3 to)
        {
            DrawLine(from, to, DefaultColor);
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            FrameActions.Add(
                new Action<IntPtr>(
                (rendererHandle) => {
                    SDL.SDL_SetRenderDrawColor(rendererHandle, color.R, color.G, color.B, color.A);
                    SDL.SDL_RenderDrawLine(rendererHandle, (int)from.X, (int)from.Y, (int)to.X, (int)to.Y);
                }));
        }

        public static void DrawRect(Rect3 rect)
        {
            DrawRect(rect, DefaultColor);
        }

        public static void DrawRect(Rect3 rect, Color color)
        {
            FrameActions.Add(
                new Action<IntPtr>(
                (rendererHandle) => {
                    SDL.SDL_SetRenderDrawColor(rendererHandle, color.R, color.G, color.B, color.A);
                    SDL.SDL_Rect sdlRect = rect.ToSdlRect();
                    SDL.SDL_RenderDrawRect(rendererHandle, ref sdlRect);
                }));
        }

        public static void DrawText(string text)
        {
            DrawText(text, DefaultColor);
        }

        public static void DrawText(string text, Color color)
        {
            Vector3 position = CurrentTextPosition;
            CurrentTextPosition += TextLineOffset;

            DrawText(position, text, color);
        }

        public static void DrawText(Vector3 position, string text)
        {
            DrawText(position, text, DefaultColor);
        }

        public static void DrawText(Vector3 position, string text, Color color)
        {
            FrameActions.Add(
                new Action<IntPtr>(
                (rendererHandle) => {
                    using (Texture texture = Texture.LoadFromRenderedText(text, color.ToSdlColor()))
                    {
                        texture.Render(rendererHandle, position.ToSdlPoint(), 1);
                    };
                }));
        }

        public static void Render()
        {
            if (IsEnabled)
            {
                foreach (Action<IntPtr> frameAction in FrameActions)
                {
                    frameAction(GraphicsManager.RendererHandle);
                }
            }

            PrepareForNextFrame();
        }

        private static void PrepareForNextFrame()
        {
            FrameActions.Clear();
            CurrentTextPosition = StartingTextPosition;
        }
    }
}
