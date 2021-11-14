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
        public Dictionary<SDL.SDL_Keycode, int> TransitionMap { get; set; }
        public double TransitionDelayTime { get; set; } = 0.25;

        private Texture Texture { get; set; }

        private SceneStateMachine SceneStateMachine { get; set; }
        private EventManager EventManager { get; set; }

        public override void OnCreate()
        {
            SceneStateMachine = ServiceLocator.Instance.GetService<SceneStateMachine>();
            EventManager = ServiceLocator.Instance.GetService<EventManager>();

            ResourceManager resourceManager = ServiceLocator.Instance.GetService<ResourceManager>();
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
            foreach (SDL.SDL_Keycode keycode in TransitionMap.Keys)
            {
                if (EventManager.IsKeyDown(keycode))
                {
                    int transitionSceneId = TransitionMap[keycode];
                    EventManager.RequestCallback(TransitionDelayTime, () => { SceneStateMachine.SwitchTo(transitionSceneId); });
                }
            }
        }

        public override void Render()
        {
            GraphicsManager graphicsManager = ServiceLocator.Instance.GetService<GraphicsManager>();
            Texture.Render(graphicsManager.RendererHandle);
        }
    }
}
