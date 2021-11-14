using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    class ExplosionSpawnerComponent : Component
    {
        public string TextureFilePath { get; set; }
        public int NumSpritesX { get; set; }
        public int NumSpritesY { get; set; }
        public int SpriteWidth { get; set; } // Width of an individual sprite (assumes tiled)
        public int SpriteHeight { get; set; } // Height of an individual sprite (assumes tiled)
        public int NumRocks { get; set; } // Number of rocks to spawn
        public int SpawnSpeed { get; set; } // How quickly to spawn the rocks
        public double MinSpeed { get; set; } // Velocity will be random between min and max
        public double MaxSpeed { get; set; } // Velocity will be random between min and max
        public double GravityAcceleration { get; set; }

        private int SpawnIndex { get; set; }
        private int SpawnCount { get; set; }

        private Vector3 SpawnPosition { get; set; }

        private GameObject Player { get; set; }
        private GameObjectCollection SceneObjects { get; set; }

        private Random Random { get; set; } = new Random();

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
        }

        public override void Update()
        {
            if (SpawnCount <= 0)
            {
                return;
            }

            int numToSpawn = (int)(SpawnSpeed * Time.DeltaTime);
            numToSpawn = Math.Min(numToSpawn, SpawnCount);

            Debug.DrawText($"NumRocks: {NumRocks}");
            Debug.DrawText($"SpawnCount: {SpawnCount}");
            Debug.DrawText($"NumToSpawn: {numToSpawn}");
            for (int i = 0; i < numToSpawn; i++)
            {
                GameObject rock = SpawnRock();
                SceneObjects.Add(rock);

                SpawnCount--;
            }
        }

        public GameObject SpawnRock()
        {
            GameObject rock = new GameObject
            {
                Name = $"Rock{SpawnIndex++}"
            };
            rock.Transform.Position = SpawnPosition;

            AcceleratedMovementComponent movementComponent = new AcceleratedMovementComponent
            {
                Velocity = new Vector3
                {
                    X = MinSpeed * Random.NextDouble() - MinSpeed / 2,
                    Y = -(MinSpeed + Random.NextDouble() * (MaxSpeed - MinSpeed)),
                    Z = 0
                },
                Acceleration = new Vector3(0, GravityAcceleration, 0),
            };
            rock.AddComponent(movementComponent);

            int spriteIndex = Random.Next(0, NumSpritesX * NumSpritesY);
            int spriteY = spriteIndex / NumSpritesX;
            int spriteX = spriteIndex % NumSpritesX;
            SpriteComponent spriteComponent = new SpriteComponent
            {
                TextureFilePath = TextureFilePath,
                NormalizedOrigin = new Vector3(0.5, 0.5),
                ClippingRect = new Rect3(spriteX * SpriteWidth, spriteY * SpriteHeight, SpriteWidth, SpriteHeight)
            };
            rock.AddComponent(spriteComponent);

            return rock;
        }

        public void StartSpawning()
        {
            SpawnPosition = Player.Transform.Position;
            SpawnCount = NumRocks;
        }
    }
}
