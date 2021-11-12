﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class GameObjectCollection
    {
        private List<GameObject> GameObjects { get; set; } = new List<GameObject>();
        private List<GameObject> NewGameObjects { get; set; } = new List<GameObject>();
        private List<GameObject> DrawableGameObjects { get; set; } = new List<GameObject>();
        private List<GameObject> CollidableGameObjects { get; set; } = new List<GameObject>();

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

            CollisionSystem.ProcessCollisions(CollidableGameObjects);
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
                foreach (GameObject gameObject in NewGameObjects)
                {
                    gameObject.Awake();
                }

                foreach (GameObject gameObject in NewGameObjects)
                {
                    gameObject.Start();
                }

                foreach (GameObject gameObject in NewGameObjects)
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

                NewGameObjects.Clear();
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

