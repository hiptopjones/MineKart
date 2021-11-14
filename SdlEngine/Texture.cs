using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    // TODO: Take a look at SFML texture / sprite for structural / functional ideas
    public class Texture : IDisposable
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private IntPtr TextureHandle { get; set; }

        private static GraphicsManager GraphicsManager { get; set; }
        private static FontManager FontManager { get; set; }

        static Texture()
        {
            GraphicsManager = ServiceLocator.Instance.GetService<GraphicsManager>();
            if (GraphicsManager == null)
            {
                throw new Exception($"Unable to retrieve graphics manager from service locator");
            }

            FontManager = ServiceLocator.Instance.GetService<FontManager>();
            if (FontManager == null)
            {
                throw new Exception($"Unable to retrieve font manager from service locator");
            }
        }

        // Use factory method
        private Texture()
        {

        }

        ~Texture()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool isDisposing)' method
            Dispose(isDisposing: false);
        }

        #region IDisposable
        private bool isAlreadyDisposed;

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isAlreadyDisposed)
            {
                if (isDisposing)
                {
                    TeardownManaged();
                }

                TeardownUnmanaged();
                isAlreadyDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool isDisposing)' method
            Dispose(isDisposing: true);
            GC.SuppressFinalize(this);
        }

        private void TeardownManaged()
        {
            // N/A
        }

        private void TeardownUnmanaged()
        {
            if (TextureHandle != IntPtr.Zero)
            {
                SDL.SDL_DestroyTexture(TextureHandle);
                TextureHandle = IntPtr.Zero;
            }
        }
        #endregion

        public static Texture LoadFromFile(string filePath)
        {
            IntPtr surfaceHandle = IntPtr.Zero;

            try
            {
                surfaceHandle = SDL_image.IMG_Load(filePath);
                if (surfaceHandle == IntPtr.Zero)
                {
                    throw new Exception($"Unable to load image: {SDL_image.IMG_GetError()}");
                }

                return CreateFromSurface(surfaceHandle);
            }
            finally
            {
                SDL.SDL_FreeSurface(surfaceHandle);
            }
        }

        // TODO: This needs to work with different fonts / sizes, not just colors
        public static Texture LoadFromRenderedText(string text, SDL.SDL_Color color)
        {
            IntPtr fontHandle = FontManager.GetFontHandle("Debug");

            IntPtr surfaceHandle = IntPtr.Zero;

            try
            {
                surfaceHandle = SDL_ttf.TTF_RenderText_Solid(fontHandle, text, color);
                if (surfaceHandle == IntPtr.Zero)
                {
                    throw new Exception($"Unable to render text surface: {SDL_ttf.TTF_GetError()}");
                }

                return CreateFromSurface(surfaceHandle);
            }
            finally
            {
                SDL.SDL_FreeSurface(surfaceHandle);
            }
        }

        private static Texture CreateFromSurface(IntPtr surfaceHandle)
        {
            IntPtr rendererHandle = GraphicsManager.RendererHandle;

            IntPtr textureHandle = SDL.SDL_CreateTextureFromSurface(rendererHandle, surfaceHandle);
            if (textureHandle == IntPtr.Zero)
            {
                throw new Exception($"Unable to create texture from image: {SDL.SDL_GetError()}");
            }

            SDL.SDL_Surface surface = Marshal.PtrToStructure<SDL.SDL_Surface>(surfaceHandle);

            Texture texture = new Texture
            {
                TextureHandle = textureHandle,
                Width = surface.w,
                Height = surface.h
            };

            return texture;
        }

        // Draws the whole texture to the origin of the target renderer, stretching to fit
        public void Render(IntPtr rendererHandle)
        {
            SDL.SDL_RenderCopy(rendererHandle, TextureHandle, IntPtr.Zero, IntPtr.Zero);
        }

        // Draws the whole texture to the target position, no stretching
        public void Render(IntPtr rendererHandle, SDL.SDL_Point targetPosition)
        {
            Render(rendererHandle, targetPosition, 1);
        }

        // Draws the whole texture to the target position, stretching by scale
        public void Render(IntPtr rendererHandle, SDL.SDL_Point targetPosition, double scale)
        {
            SDL.SDL_Rect targetRect = new SDL.SDL_Rect
            {
                x = targetPosition.x,
                y = targetPosition.y,
                w = (int)(Width * scale),
                h = (int)(Height * scale)
            };

            SDL.SDL_RenderCopy(rendererHandle, TextureHandle, IntPtr.Zero, ref targetRect);
        }

        // Draws the part of the texture specified by sourceRect to targetRect, flipping horizontally if specified, stretching to fit
        public void Render(IntPtr rendererHandle, SDL.SDL_Rect sourceRect, SDL.SDL_Rect targetRect, bool isFlipped)
        {
            SDL.SDL_RendererFlip flip = isFlipped ? SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL : SDL.SDL_RendererFlip.SDL_FLIP_NONE;
            SDL.SDL_RenderCopyEx(rendererHandle, TextureHandle, ref sourceRect, ref targetRect, 0, IntPtr.Zero, flip);
        }

        // Draws the part of the texture specified by sourceRect to targetRect, rotating by angleDegrees, stretching to fit
        public void Render(IntPtr rendererHandle, SDL.SDL_Rect sourceRect, SDL.SDL_Rect targetRect, double angleDegrees)
        {
            SDL.SDL_RenderCopyEx(rendererHandle, TextureHandle, ref sourceRect, ref targetRect, angleDegrees, IntPtr.Zero, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
        }

        public Rect3 GetRect()
        {
            return new Rect3(Width, Height);
        }
    }
}
