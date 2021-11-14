using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class SceneStateMachine
    {
        private Dictionary<int, Scene> SceneMap { get; set; } = new Dictionary<int, Scene>();

        private Scene CurrentScene { get; set; }

        public void AddScene(int sceneId, Scene scene)
        {
            SceneMap[sceneId] = scene;
            scene.OnCreate();
        }

        public void SwitchTo(int sceneId)
        {
            Scene scene = SceneMap[sceneId];

            if (CurrentScene != null)
            {
                CurrentScene.OnDeactivate();
            }

            CurrentScene = scene;
            CurrentScene.OnActivate();
        }

        public void RemoveScene(int sceneId)
        {
            Scene scene = SceneMap[sceneId];
            
            if (scene == CurrentScene)
            {
                CurrentScene.OnDeactivate();
                CurrentScene = null;
            }

            scene.OnDestroy();
            SceneMap.Remove(sceneId);
        }

        public void ProcessInput()
        {
            if (CurrentScene != null)
            {
                CurrentScene.ProcessInput();
            }
        }

        public void Update()
        {
            if (CurrentScene != null)
            {
                CurrentScene.Update();
            }
        }

        public void LateUpdate()
        {
            if (CurrentScene != null)
            {
                CurrentScene.LateUpdate();
            }
        }

        public void Render()
        {
            if (CurrentScene != null)
            {
                CurrentScene.Render();
            }
        }

    }
}
