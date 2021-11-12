using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NLog;
using SDL2;

namespace SdlEngine
{
	public class FontManager : IDisposable
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private static FontManager _instance;
		public static FontManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new FontManager();
				}

				return _instance;
			}
		}

		private Dictionary<string, IntPtr> FontsMap { get; set; } = new Dictionary<string, IntPtr>();

		public FontManager()
		{
			Initialize();
		}

		~FontManager()
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
			List<string> handleNames = FontsMap.Keys.ToList();

			foreach (string handleName in handleNames)
			{
				IntPtr fontHandle = FontsMap[handleName];

				if (fontHandle != IntPtr.Zero)
				{
					SDL_ttf.TTF_CloseFont(fontHandle);
					fontHandle = IntPtr.Zero;
				}

				FontsMap.Remove(handleName);
			}

			SDL_ttf.TTF_Quit();
		}

		private void TeardownUnmanaged()
		{
			// N/A
		}
		#endregion

		private void Initialize()
		{
			Logger.Info("Setting up font manager");

			int sdlResult;

			sdlResult = SDL_ttf.TTF_Init();
			if (sdlResult < 0)
			{
				throw new Exception($"Unable to initialize SDL_ttf: {SDL_ttf.TTF_GetError()}");
			}
            
			Logger.Info("Font manager setup complete");
		}

		public void LoadFont(string handleName, string fontFilePath, int fontSize)
        {
			IntPtr fontHandle = SDL_ttf.TTF_OpenFont(fontFilePath, fontSize);
			if (fontHandle == IntPtr.Zero)
			{
				throw new Exception($"Failed to load font: {SDL_ttf.TTF_GetError()}");
			}

			FontsMap[handleName] = fontHandle;
        }

		public IntPtr GetFontHandle(string handleName)
		{
			IntPtr fontHandle;
			if (false == FontsMap.TryGetValue(handleName, out fontHandle))
			{
				throw new Exception($"Unable to locate font handle for '{handleName}'");
			}

			return fontHandle;
		}
	}
}
