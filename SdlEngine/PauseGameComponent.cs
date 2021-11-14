using SDL2;
using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    class PauseGameComponent : DrawableComponent
    {
        public string TextureFilePath { get; set; }

        private Texture Texture { get; set; }
        private EventManager EventManager { get; set; }
        private bool IsPaused { get; set; }

        public override void Awake()
        {
            EventManager = ServiceLocator.Instance.GetService<EventManager>();

            ResourceManager resourceManager = ServiceLocator.Instance.GetService<ResourceManager>();
            Texture = resourceManager.GetTexture(TextureFilePath);
        }

        public override void Update()
        {
            if (EventManager.IsKeyDown(SDL.SDL_Keycode.SDLK_ESCAPE))
            {
                IsPaused = !IsPaused;
            }

            Time.TimeScale = IsPaused ? 0 : 1;
        }

        public override void Render()
        {
            if (IsPaused)
            {
                GraphicsManager graphicsManager = ServiceLocator.Instance.GetService<GraphicsManager>();
                Texture.Render(graphicsManager.RendererHandle);
            }
        }
    }
}
