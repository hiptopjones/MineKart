using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class DebugComponent : Component
    {
        private EventManager EventManager { get; set; }

        public override void Awake()
        {
            EventManager = ServiceLocator.Instance.GetService<EventManager>();
            if (EventManager == null)
            {
                throw new Exception($"Unable to retrieve event manager from service locator");
            }
        }

        public override void Update()
        {
            if (EventManager.IsKeyDown(SDL.SDL_Keycode.SDLK_d))
            {
                Debug.IsEnabled = !Debug.IsEnabled;
            }

            if (EventManager.IsKeyDown(SDL.SDL_Keycode.SDLK_t))
            {
                if (Time.TimeScale != 0)
                {
                    Time.TimeScale = 0;
                }
                else
                {
                    Time.TimeScale = 1;
                }
            }

            if (EventManager.IsKeyDown(SDL.SDL_Keycode.SDLK_BACKSLASH))
            {
                Time.StepSingleFrame = true;
            }
        }
    }
}
