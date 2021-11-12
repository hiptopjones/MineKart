using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class SplashScreenScene : Scene
    {
        private string TextureFilePath { get; set; }
        private SceneStateMachine SceneStateMachine { get; set; }
        private double TransitionDelayTime { get; set; }
        private int TransitionSceneId { get; set; }

        private double ElapsedTime { get; set; }
        private Texture SplashScreenTexture { get; set; }

        public SplashScreenScene(string textureFilePath, SceneStateMachine sceneStateMachine, int transitionSceneId, double transitionDelayTime)
        {
            TextureFilePath = textureFilePath;
            SceneStateMachine = sceneStateMachine;
            TransitionDelayTime = transitionDelayTime;
            TransitionSceneId = transitionSceneId;

            Initialize();
        }

        private void Initialize()
        {
            ResourceManager resourceManager = ServiceLocator.Instance.GetService<ResourceManager>();
            SplashScreenTexture = resourceManager.GetTexture(TextureFilePath);
        }

        public override void OnActivate()
        {
            ElapsedTime = 0;
        }

        public override void Update()
        {
            ElapsedTime += Time.DeltaTime;
            if (ElapsedTime >= TransitionDelayTime)
            {
                SceneStateMachine.SwitchTo(TransitionSceneId);
            }
        }

        public override void Render()
        {
            GraphicsManager graphicsManager = ServiceLocator.Instance.GetService<GraphicsManager>();
            SplashScreenTexture.Render(graphicsManager.RendererHandle);
        }
    }
}
