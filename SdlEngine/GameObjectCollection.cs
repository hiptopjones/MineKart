using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class GameObjectCollection
    {
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();
        public List<GameObject> NewGameObjects { get; set; } = new List<GameObject>();
        public List<GameObject> DrawableGameObjects { get; set; } = new List<GameObject>();
        public List<GameObject> CollidableGameObjects { get; set; } = new List<GameObject>();

        private DrawableSystem DrawableSystem { get; set; }
        private CollisionSystem CollisionSystem { get; set; }

        public GameObjectCollection()
        {
            Initialize();
        }

        private void Initialize()
        {
            DrawableSystem = new DrawableSystem();
            CollisionSystem = new CollisionSystem();
        }

        public void Add(GameObject gameObject)
        {
            NewGameObjects.Add(gameObject);
        }

        public void AddRange(IEnumerable<GameObject> gameObjects)
        {
            NewGameObjects.AddRange(gameObjects);
        }

        public void Update()
        {
            foreach (GameObject gameObject in GameObjects)
            {
                if (gameObject.IsAlive)
                {
                    gameObject.Update();
                }
            }

            CollisionSystem.Update(CollidableGameObjects);
        }

        public void LateUpdate()
        {
            foreach (GameObject gameObject in GameObjects)
            {
                if (gameObject.IsAlive)
                {
                    gameObject.LateUpdate();
                }
            }
        }

        public void Render()
        {
            DrawableSystem.Render(DrawableGameObjects);
        }

        public void ProcessAdditions()
        {
            if (NewGameObjects.Count > 0)
            {
                // Swap out the collection to avoid races with new objects being added during the below processing

                List<GameObject> newGameObjects = NewGameObjects;
                NewGameObjects = new List<GameObject>();

                foreach (GameObject gameObject in newGameObjects)
                {
                    gameObject.Awake();
                }

                foreach (GameObject gameObject in newGameObjects)
                {
                    gameObject.Start();
                }

                foreach (GameObject gameObject in newGameObjects)
                {
                    GameObjects.Add(gameObject);

                    DrawableComponent drawableComponent = gameObject.GetComponent<DrawableComponent>();
                    if (drawableComponent != null)
                    {
                        DrawableGameObjects.Add(gameObject);
                    }

                    ColliderComponent colliderComponent = gameObject.GetComponent<ColliderComponent>();
                    if (colliderComponent != null)
                    {
                        CollidableGameObjects.Add(gameObject);
                    }
                }
            }
        }

        public void ProcessRemovals()
        {
            ProcessRemovals(GameObjects);

            // TODO: Consider using a (sorted?) set so that removing known items is O(1)
            ProcessRemovals(DrawableGameObjects);
            ProcessRemovals(CollidableGameObjects);
        }

        private void ProcessRemovals(List<GameObject> gameObjects)
        {
            int i = 0;
            while (i < gameObjects.Count)
            {
                GameObject gameObject = gameObjects[i];

                if (gameObject.IsAlive)
                {
                    // Only increment if we didn't swap
                    i++;
                    continue;
                }

                // Swap and pop
                int lastIndex = gameObjects.Count - 1;
                gameObjects[i] = gameObjects[lastIndex];
                gameObjects.RemoveAt(lastIndex);
            }
        }
    }
}

