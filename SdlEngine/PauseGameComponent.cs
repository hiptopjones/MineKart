using SDL2;
using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class PauseGameComponent : DrawableComponent
    {
        public string TextureFilePath { get; set; }

        private Texture Texture { get; set; }
        private GraphicsManager GraphicsManager { get; set; }
        private EventManager EventManager { get; set; }
        private bool IsPaused { get; set; }

        public override void Awake()
        {
            GraphicsManager = ServiceLocator.Instance.GetService<GraphicsManager>();
            if (GraphicsManager == null)
            {
                throw new Exception($"Unable to retrieve graphics manager from service locator");
            }

            EventManager = ServiceLocator.Instance.GetService<EventManager>();
            if (EventManager == null)
            {
                throw new Exception($"Unable to retrieve event manager from service locator");
            }

            ResourceManager resourceManager = ServiceLocator.Instance.GetService<ResourceManager>();
            if (resourceManager == null)
            {
                throw new Exception($"Unable to retrieve resource manager from service locator");
            }

            Texture = resourceManager.GetTexture(TextureFilePath);
        }

        public override void Update()
        {
            if (EventManager.IsKeyDown(SDL.SDL_Keycode.SDLK_ESCAPE))
            {
                IsPaused = !IsPaused;
                Time.TimeScale = IsPaused ? 0 : 1;
            }
        }

        public override void Render()
        {
            if (IsPaused)
            {
                Texture.Render(GraphicsManager.RendererHandle);
            }
        }
    }
}
