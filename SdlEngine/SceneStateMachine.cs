using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class SceneStateMachine
    {
        private int NextSceneId { get; set; }
        private Dictionary<int, Scene> SceneMap { get; set; } = new Dictionary<int, Scene>();

        private Scene CurrentScene { get; set; }

        public int AddScene(Scene scene)
        {
            int sceneId = NextSceneId++;

            SceneMap[sceneId] = scene;
            scene.OnCreate();

            return sceneId;
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
