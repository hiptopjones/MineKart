using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    public class EnemySpawnerComponent : Component
    {
        public double SpawnTime { get; set; }
        public int SpawnAheadDistance { get; set; }

        private int SpawnIndex { get; set; }

        private GameObject Player { get; set; }
        private GameObjectCollection SceneObjects { get; set; }
        private EventManager EventManager { get; set; }

        public override void Awake()
        {
            Player = ServiceLocator.Instance.GetService<GameObject>("Player");
            if (Player == null)
            {
                throw new Exception($"Unable to retrieve player from service locator");
            }

            SceneObjects = ServiceLocator.Instance.GetService<GameObjectCollection>();
            if (SceneObjects == null)
            {
                throw new Exception($"Unable to retrieve game object collection from service locator");
            }

            EventManager = ServiceLocator.Instance.GetService<EventManager>();
            if (EventManager == null)
            {
                throw new Exception($"Unable to retrieve event manager from service locator");
            }
        }

        public override void Start()
        {
            EventManager.RequestCallback(SpawnTime, SpawnEnemy);
        }

        private void SpawnEnemy()
        {
            GameObject enemy = GenerateEnemy();
            SceneObjects.Add(enemy);

            // Repeat the callback
            EventManager.RequestCallback(SpawnTime, SpawnEnemy);
        }

        private GameObject GenerateEnemy()
        {
            GameObject enemy = new GameObject
            {
                Name = $"Enemy{SpawnIndex++}"
            };
            enemy.Transform.Position = Player.Transform.Position + new Vector3(0, 0, SpawnAheadDistance);

            AutomaticMovementComponent movementComponent = new AutomaticMovementComponent
            {
                Velocity = new Vector3(0, 0, -10)
            };
            enemy.AddComponent(movementComponent);

            SpriteComponent spriteComponent = new SpriteComponent
            {
                TextureFilePath = GameSettings.EnemyTextureFilePath,
                NormalizedOrigin = new Vector3(0.5, 1.0),
                UseTransformPosition = false
            };
            enemy.AddComponent(spriteComponent);

            SpriteComponent warningSpriteComponent = new SpriteComponent
            {
                TextureFilePath = GameSettings.WarningTextureFilePath,
                NormalizedOrigin = new Vector3(0.5, 7), // TODO: This is a hacky way to shift it above the enemy's head
                UseTransformPosition = false
            };
            enemy.AddComponent(warningSpriteComponent);

            TrackAlignmentComponent alignmentComponent = new TrackAlignmentComponent();
            enemy.AddComponent(alignmentComponent);

            RangedDestroyComponent destroyComponent = new RangedDestroyComponent
            {
                PruneDistance = 5
            };
            enemy.AddComponent(destroyComponent);

            BoxColliderComponent boxColliderComponent = new BoxColliderComponent
            {
                BoundingBox = new Rect3(-0.1, -0.1, -0.25, 0.2, 0.2, 0.5),
                Collider = new BoxCollider()
            };
            enemy.AddComponent(boxColliderComponent);

            DebugActionComponent debugComponent = new DebugActionComponent
            {
                DebugAction = (gameObject) =>
                {
                    Debug.DrawText($"Enemy: {gameObject.Transform.Position}");
                }
            };
            enemy.AddComponent(debugComponent);

            return enemy;
        }
    }
}
