using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class Sound : IDisposable
    {
        private IntPtr SoundHandle { get; set; }

        // Use factory method
        private Sound()
        {

        }

        ~Sound()
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
            if (SoundHandle != IntPtr.Zero)
            {
                SDL_mixer.Mix_FreeChunk(SoundHandle);
                SoundHandle = IntPtr.Zero;
            }
        }
        #endregion

        public static Sound LoadFromFile(string filePath)
        {
            IntPtr soundHandle = SDL_mixer.Mix_LoadWAV(filePath);
            if (soundHandle == IntPtr.Zero)
            {
                throw new Exception($"Unable to load sound: {SDL_mixer.Mix_GetError()}");
            }

            Sound sound = new Sound
            {
                SoundHandle = soundHandle
            };

            return sound;
        }

        public void Play()
        {
            Play(-1);
        }

        public void Play(int channel)
        {
            SDL_mixer.Mix_PlayChannel(channel, SoundHandle, 0);
        }
    }
}
