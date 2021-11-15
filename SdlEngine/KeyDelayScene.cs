using SDL2;
using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class KeyDelayScene : Scene
    {
        public string TextureFilePath { get; set; }
        public List<KeyValuePair<SDL.SDL_Keycode, int>> TransitionMap { get; set; }
        public double TransitionDelayTime { get; set; } = 0.25;

        private Texture Texture { get; set; }

        private SceneManager SceneManager { get; set; }
        private EventManager EventManager { get; set; }
        private GraphicsManager GraphicsManager { get; set; }

        public override void OnCreate()
        {
            SceneManager = ServiceLocator.Instance.GetService<SceneManager>();
            if (SceneManager == null)
            {
                throw new Exception($"Unable to retrieve scene manager from service locator");
            }

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

        public override void OnActivate()
        {
            if (TransitionMap.Count == 0)
            {
                throw new Exception("No transition key-mappings provided");
            }
        }

        public override void Update()
        {
            foreach (KeyValuePair<SDL.SDL_Keycode, int> pair in TransitionMap)
            {
                if (EventManager.IsKeyDown(pair.Key))
                {
                    int transitionSceneId = pair.Value;
                    EventManager.RequestCallback(TransitionDelayTime, () => { SceneManager.SwitchTo(transitionSceneId); });
                }
            }
        }

        public override void Render()
        {
            Texture.Render(GraphicsManager.RendererHandle);
        }
    }
}
