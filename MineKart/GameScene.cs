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
                    TextureFilePath = GetTrackSegmentTypeTextureFilePath(segmentComponent.SegmentType)
            };

                segment.AddComponent(drawableComponent);

                gameObjects.Add(segment);
            }

            return gameObjects;
        }

        private string GetTrackSegmentTypeTextureFilePath(TrackSegmentType segmentType)
        {
            switch (segmentType)
            {
                case TrackSegmentType.Track:
                    return "Assets\\track.png";

                case TrackSegmentType.TrackBreaking:
                    return "Assets\\track-breaking.png";

                case TrackSegmentType.TrackFixing:
                    return "Assets\\track-fixing.png";

                case TrackSegmentType.HoleEntering:
                    return "Assets\\hole-entering.png";

                case TrackSegmentType.Hole:
                    return "Assets\\hole.png";

                case TrackSegmentType.HoleExiting:
                    return "Assets\\hole-exiting.png";

                default:
                    throw new Exception($"Unhandled segment type: {segmentType}");
            }
        }

        private GameObject InitializePlayer()
        {
            GameObject player = new GameObject();
            player.Transform.Position = new Vector3(0, 1, 3);

            RailsMovementComponent movementComponent = new RailsMovementComponent
            {
                NormalForwardSpeed = 10,
                BrakeForwardSpeed = 5,
                JumpSpeed = -20,
                GravityAcceleration = 100
            };
            player.AddComponent(movementComponent);

            SpriteComponent spriteComponent = new SpriteComponent
            {
                TextureFilePath = "Assets\\player.png",
                NormalizedOrigin = new Vector3(0.5, 1.0),
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

        private GameObject InitializeFollowCamera()
        {
            GameObject follow = new GameObject();

            FollowCameraComponent followComponent = new FollowCameraComponent
            {
                DefaultPosition = Vector3.Zero,
                Camera = ServiceLocator.Instance.GetService<Camera>(),
                FollowObject = ServiceLocator.Instance.GetService<GameObject>("Player"),
                FollowOffset = new Vector3(0, -1, -2),
                IsFollowingZ = true
            };

            follow.AddComponent(followComponent);

            return follow;
        }
    }
}
