using NLog;
using SDL2;
using System;

namespace SdlEngine
{
	public class GraphicsManager : IDisposable
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public IntPtr WindowHandle { get; private set; }
		public IntPtr RendererHandle { get; private set; }

		private string WindowTitle { get; set; }

		private int WindowWidth { get; set; }
		private int WindowHeight { get; set; }

		private int RenderWidth { get; set; }
		private int RenderHeight { get; set; }

		public GraphicsManager(string windowTitle, int renderWidth, int renderHeight, int windowWidth, int windowHeight)
		{
			WindowTitle = windowTitle;

			RenderWidth = renderWidth;
			RenderHeight = renderHeight;
			
			WindowWidth = windowWidth;
			WindowHeight = windowHeight;

			Initialize();
		}

		~GraphicsManager()
		{
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
			SDL_image.IMG_Quit();

			if (RendererHandle != IntPtr.Zero)
			{
				SDL.SDL_DestroyRenderer(RendererHandle);
				RendererHandle = IntPtr.Zero;
			}

			if (WindowHandle != IntPtr.Zero)
			{
				SDL.SDL_DestroyWindow(WindowHandle);
				WindowHandle = IntPtr.Zero;
			}

			SDL.SDL_Quit();
		}
		#endregion

		private void Initialize()
		{
			Logger.Info("Setting up graphics manager");

			int sdlResult;
			SDL.SDL_bool sdlSuccess;

			sdlResult = SDL.SDL_InitSubSystem(SDL.SDL_INIT_VIDEO);
			if (sdlResult < 0)
			{
				throw new Exception($"Unable to initialize video subsystem: {SDL.SDL_GetError()}");
			}

			sdlSuccess = SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "1");
			if (sdlSuccess == SDL.SDL_bool.SDL_FALSE)
			{
				Logger.Warn("Linear texture filtering not enabled");
			}

			WindowHandle = SDL.SDL_CreateWindow(WindowTitle, SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, WindowWidth, WindowHeight, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
			if (WindowHandle == IntPtr.Zero)
			{
				throw new Exception($"Unable to create window: {SDL.SDL_GetError()}");
			}

			RendererHandle = SDL.SDL_CreateRenderer(WindowHandle, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
			if (RendererHandle == IntPtr.Zero)
			{
				throw new Exception($"Unable to create renderer: {SDL.SDL_GetError()}");
			}

			sdlResult = SDL.SDL_RenderSetLogicalSize(RendererHandle, RenderWidth, RenderHeight);
			if (sdlResult < 0)
			{
				throw new Exception($"Logical size could not be set: {SDL.SDL_GetError()}");
			}

			sdlResult = SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG);
			if (sdlResult == 0)
			{
				throw new Exception($"Unable to initialize SDL_image: {SDL_image.IMG_GetError()}");
			}

			Logger.Info("Graphics manager setup complete");
		}
	}
}
