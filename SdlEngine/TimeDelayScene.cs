using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class TimeDelayScene : Scene
    {
        public string TextureFilePath { get; set; }
        public double TransitionDelayTime { get; set; }
        public int TransitionSceneId { get; set; }

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
            EventManager.RequestCallback(TransitionDelayTime, () => { SceneStateMachine.SwitchTo(TransitionSceneId); });
        }

        public override void Render()
        {
            GraphicsManager graphicsManager = ServiceLocator.Instance.GetService<GraphicsManager>();
            Texture.Render(graphicsManager.RendererHandle);
        }
    }
}
