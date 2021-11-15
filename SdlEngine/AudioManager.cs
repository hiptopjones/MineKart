using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NLog;
using SDL2;

namespace SdlEngine
{
	public class AudioManager : IDisposable
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private static AudioManager _instance;
		public static AudioManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AudioManager();
				}

				return _instance;
			}
		}

		public AudioManager()
		{
			Initialize();
		}

		~AudioManager()
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
			SDL_mixer.Mix_CloseAudio();

			SDL.SDL_QuitSubSystem(SDL.SDL_INIT_AUDIO);
		}

		private void TeardownUnmanaged()
		{
			// N/A
		}
		#endregion

		private void Initialize()
		{
			Logger.Info("Setting up audio manager");

			int sdlResult;

			sdlResult = SDL.SDL_InitSubSystem(SDL.SDL_INIT_AUDIO);
			if (sdlResult < 0)
			{
				throw new Exception($"Unable to initialize audio subsystem: {SDL.SDL_GetError()}");
			}

			sdlResult = SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);
			if (sdlResult < 0)
			{
				throw new Exception($"Unable to initialize SDL_mixer: {SDL_mixer.Mix_GetError()}");
			}

			Logger.Info("Audio manager setup complete");
		}
	}
}
