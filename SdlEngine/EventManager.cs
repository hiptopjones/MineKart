using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using SDL2;

namespace SdlEngine
{
    public class EventManager : IDisposable
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private HashSet<SDL.SDL_Keycode> KeyDown { get; set; } = new HashSet<SDL.SDL_Keycode>();
        private HashSet<SDL.SDL_Keycode> KeyPressed { get; set; } = new HashSet<SDL.SDL_Keycode>();
        private HashSet<SDL.SDL_Keycode> KeyUp { get; set; } = new HashSet<SDL.SDL_Keycode>();

        private bool IsQuitEventRaised { get; set; }

        private List<KeyValuePair<double, Action>> Callbacks { get; set; } = new List<KeyValuePair<double, Action>>();

        public EventManager()
        {
            Initialize();
        }

        ~EventManager()
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
            // N/A
        }

        #endregion

        private void Initialize()
        {
            Logger.Info("Setting up event manager");

            Logger.Info("Event manager setup complete");
        }

        public void HandleEvents()
        {
            ProcessKeyFlags();
            ProcessCallbacks();
 
            // Now handle events
            SDL.SDL_Event e;

            while (SDL.SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    IsQuitEventRaised = true;
                }

                if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    if (e.key.repeat != 0)
                    {
                        // Ignore repeated key downs
                        continue;
                    }

                    SDL.SDL_Keycode keycode = e.key.keysym.sym;
                    KeyDown.Add(keycode);
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    SDL.SDL_Keycode keycode = e.key.keysym.sym;
                    KeyUp.Add(keycode);

                    KeyPressed.Remove(keycode);
                }
            }
        }

        public bool IsQuitRequested()
        {
            return IsQuitEventRaised;
        }

        public bool IsKeyDown(SDL.SDL_Keycode keycode)
        {
            return KeyDown.Contains(keycode);
        }

        public bool IsKeyPressed(SDL.SDL_Keycode keycode)
        {
            return KeyPressed.Contains(keycode);
        }

        public bool IsKeyUp(SDL.SDL_Keycode keycode)
        {
            return KeyUp.Contains(keycode);
        }

        private void ProcessKeyFlags()
        {
            // Shuffle key flags on new frame
            KeyUp.Clear();

            foreach (SDL.SDL_Keycode keycode in KeyDown)
            {
                KeyPressed.Add(keycode);
            }

            KeyDown.Clear();
        }

        public void RequestCallback(double callbackDeltaTime, Action callbackAction)
        {
            Callbacks.Add(new KeyValuePair<double, Action>(Time.TotalTime + callbackDeltaTime, callbackAction));
        }

        private void ProcessCallbacks()
        {
            int i = 0;
            while (i < Callbacks.Count)
            {
                KeyValuePair<double, Action> callback = Callbacks[i];

                if (Time.TotalTime > callback.Key)
                {
                    try
                    {
                        callback.Value();
                    }
                    finally
                    {
                        Callbacks[i] = Callbacks[Callbacks.Count - 1];
                        Callbacks.RemoveAt(Callbacks.Count - 1);
                    }
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
