using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    class SubdividingExplosionComponent : Component
    {
        public string TextureFilePath { get; set; }
        public int NumSplits { get; set; }
        public double MinSpeed { get; set; } // Velocity will be random between min and max
        public double MaxSpeed { get; set; } // Velocity will be random between min and max
        public double GravityAcceleration { get; set; }

        private bool IsDividing { get; set; }
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
            if (IsDividing)
            {
                IsDividing = false;

                ResourceManager resourceManager = ServiceLocator.Instance.GetService<ResourceManager>();
                if (resourceManager == null)
                {
                    throw new Exception($"Unable to retrieve resource manager from service locator");
                }

                Texture texture = resourceManager.GetTexture(TextureFilePath);
                if (texture == null)
                {
                    throw new Exception($"Unable to load texture from file");
                }

                int numAxisFragments = (int)Math.Pow(NumSplits, 2);
                int fragmentWidth = texture.Width / numAxisFragments;
                int fragmentHeight = texture.Height / numAxisFragments;
                for (int fragmentIndexX = 0; fragmentIndexX < numAxisFragments; fragmentIndexX++)
                {
                    for (int fragmentIndexY = 0; fragmentIndexY < numAxisFragments; fragmentIndexY++)
                    {
                        int fragmentIndex = fragmentIndexY * numAxisFragments + fragmentIndexX;
                        GameObject fragment = SpawnFragment(fragmentIndex, fragmentIndexX, fragmentIndexY, fragmentWidth, fragmentHeight);
                        SceneObjects.Add(fragment);
                    }
                }
            }
        }

        public GameObject SpawnFragment(int fragmentIndex, int fragmentIndexX, int fragmentIndexY, int fragmentWidth, int fragmentHeight)
        {
            GameObject fragment = new GameObject
            {
                Name = $"Fragment{fragmentIndex++}"
            };
            fragment.Transform.Position = SpawnPosition;

            // TODO: Could calculate radians using position relative to the center of the texture?
            double velocityRadians = Random.NextDouble() * 0.25 * Math.PI - 0.675 * Math.PI;
            double velocitySpeed = MinSpeed + Random.NextDouble() * (MaxSpeed - MinSpeed);
            AcceleratedMovementComponent movementComponent = new AcceleratedMovementComponent
            {
                Velocity = Vector3.FromPolar(velocitySpeed, velocityRadians),
                Acceleration = new Vector3(0, GravityAcceleration, 0),
            };
            fragment.AddComponent(movementComponent);

            SpriteComponent spriteComponent = new SpriteComponent
            {
                TextureFilePath = TextureFilePath,
                NormalizedOrigin = new Vector3(0.5, 0.5),
                ClippingRect = new Rect3(fragmentIndexX * fragmentWidth, fragmentIndexY * fragmentHeight, fragmentWidth, fragmentHeight)
            };
            fragment.AddComponent(spriteComponent);

            return fragment;
        }

        public void StartSpawning()
        {
            SpawnPosition = Player.Transform.Position;
            IsDividing = true;
        }
    }
}
