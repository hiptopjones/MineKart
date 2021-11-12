using SDL2;
using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    public class GameScene : Scene
    {
        private GameObjectCollection GameObjects { get; set; }

        public GameScene()
        {
            Initialize();
        }

        // TODO: Should this be OnCreate()?
        private void Initialize()
        {
            GameObjects = new GameObjectCollection();

            GameObject player = InitializePlayer();
            GameObjects.Add(player);

            // Make the player available to the locator
            ServiceLocator.Instance.ProvideService("Player", player);

            //GameObjects.AddRange(
            //    new[] {
            //        InitializeEnemy(new Vector3(1, 1, 5)),
            //        InitializeEnemy(new Vector3(1, 1, 4)),
            //        InitializeEnemy(new Vector3(-1, 1, 5)),
            //        InitializeEnemy(new Vector3(-1, 1, 4)),
            //        InitializeEnemy(new Vector3(0, 1, 5)),
            //        InitializeEnemy(new Vector3(0, 1, 4))
            //    });

            GameObject follow = InitializeFollowCamera();
            GameObjects.Add(follow);

            List<GameObject> trackSegments = InitializeTrack();
            GameObjects.AddRange(trackSegments);

            GameObject debug = InitializeDebug();
            GameObjects.Add(debug);
        }

        public override void Update()
        {
            GameObjects.ProcessRemovals();
            GameObjects.ProcessAdditions();
            GameObjects.Update();
        }

        public override void LateUpdate()
        {
            GameObjects.LateUpdate();
        }

        public override void Render()
        {
            GameObjects.Render();
            Debug.Render();
        }

        private GameObject InitializeDebug()
        {
            GameObject debug = new GameObject();

            DebugComponent debugComponent = new DebugComponent();
            debug.AddComponent(debugComponent);

            return debug;
        }

        private List<GameObject> InitializeTrack()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            
            List<TrackSegmentComponent> segmentComponents = TrackGenerator.GenerateTrack();
            for (int z = 0; z < segmentComponents.Count; z++)
            {
                TrackSegmentComponent segmentComponent = segmentComponents[z];

                GameObject segment = new GameObject();
                segment.Transform.Position = new Vector3(0, 1, z);

                segment.AddComponent(segmentComponent);

                TrackSegmentDrawableComponent drawableComponent = new TrackSegmentDrawableComponent
                {
                    TextureFilePath = "Assets\\segment.png"
                };

                segment.AddComponent(drawableComponent);

                gameObjects.Add(segment);
            }

            return gameObjects;
        }

        private GameObject InitializePlayer()
        {
            GameObject player = new GameObject();
            player.Transform.Position = new Vector3(0, 1, 3);

            BasicMovementComponent movementComponent = new BasicMovementComponent
            {
                BoundingBox = new Rect3(-10, -10, 0, 20, 20, 100),
                //LeftRightVelocity = new Vector3(2, 0, 0),
                UpDownVelocity = new Vector3(0, 0, -2),
                //InOutVelocity = new Vector3(0, -2, 0),
                ShiftSpeedMultiplier = 0.05,
                CtrlSpeedMultiplier = 5
            };
            player.AddComponent(movementComponent);

            SpriteComponent spriteComponent = new SpriteComponent
            {
                TextureFilePath = "Assets\\player.png",
                NormalizedOrigin = new Vector3(0.5, 1.0),
                //ClippingRect = new Rect3(53, 113)
            };
            player.AddComponent(spriteComponent);

            BoxColliderComponent boxColliderComponent = new BoxColliderComponent
            {
                BoundingBox = new Rect3(-0.1, -0.1, -0.1, 0.2, 0.2, 0.2),
                Collider = new BoxCollider()
            };
            player.AddComponent(boxColliderComponent);

            DebugActionComponent debugComponent = new DebugActionComponent
            {
                DebugAction = (gameObject) =>
                {
                    Debug.DrawText($"Player: {gameObject.Transform.Position}");
                }
            };
            player.AddComponent(debugComponent);

            return player;
        }

        private GameObject InitializeEnemy(Vector3 position)
        {
            GameObject enemy = new GameObject();
            enemy.Transform.Position = position;

            SpriteComponent spriteComponent = new SpriteComponent
            {
                TextureFilePath = "Assets\\player.png",
                NormalizedOrigin = new Vector3(0.5, 1.0),
                ClippingRect = new Rect3(53, 113)
            };
            enemy.AddComponent(spriteComponent);

            BoxColliderComponent boxColliderComponent = new BoxColliderComponent
            {
                BoundingBox = new Rect3(-0.1, -0.1, -0.1, 0.2, 0.2, 0.2),
                Collider = new BoxCollider()
            };
            enemy.AddComponent(boxColliderComponent);

            return enemy;
        }

        private GameObject InitializeFollowCamera()
        {
            GameObject follow = new GameObject();
            follow.Transform.Position = new Vector3(0, 0, 0);

            FollowCameraComponent followComponent = new FollowCameraComponent
            {
                Camera = ServiceLocator.Instance.GetService<Camera>(),
                FollowObject = ServiceLocator.Instance.GetService<GameObject>("Player"),
                FollowOffset = new Vector3(0, -1, -2)
            };

            follow.AddComponent(followComponent);

            return follow;
        }
    }
}
