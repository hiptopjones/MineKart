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
        public GameObjectCollection GameObjects { get; set; }

        public override void OnActivate()
        {
            GameObjects = new GameObjectCollection();

            // Make the game object collection available to the locator
            ServiceLocator.Instance.ProvideService<GameObjectCollection>(GameObjects);

            GameObject player = InitializePlayer();
            GameObjects.Add(player);

            // Make the player available to the locator
            ServiceLocator.Instance.ProvideService("Player", player);

            GameObject follow = InitializeFollowCamera();
            GameObjects.Add(follow);

            GameObject trackGenerator = InitializeTrackGenerator();
            GameObjects.Add(trackGenerator);

            GameObject debug = InitializeDebug();
            GameObjects.Add(debug);

            GameObject pause = InitializePause();
            GameObjects.Add(pause);
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
            GameObject debug = new GameObject
            {
                Name = "Debug"
            };

            DebugComponent debugComponent = new DebugComponent();
            debug.AddComponent(debugComponent);

            return debug;
        }

        private GameObject InitializeTrackGenerator()
        {
            GameObject trackGenerator = new GameObject
            {
                Name = "TrackGenerator"
            };

            TrackGeneratorComponent trackGeneratorComponent = new TrackGeneratorComponent
            {
                GenerateDistance = 100,
                PruneDistance = 5,
                Player = ServiceLocator.Instance.GetService<GameObject>("Player")
            };
            trackGenerator.AddComponent(trackGeneratorComponent);

            return trackGenerator;
        }

        private GameObject InitializePlayer()
        {
            GameObject player = new GameObject
            {
                Name = "Player"
            };
            player.Transform.Position = new Vector3(0, 1, 3);

            RailsMovementComponent movementComponent = new RailsMovementComponent
            {
                NormalForwardSpeed = 10,
                BrakeForwardSpeed = 2,
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

            PlayerCollisionHandlerComponent collisionHandlerComponent = new PlayerCollisionHandlerComponent();
            player.AddComponent(collisionHandlerComponent);

            return player;
        }

        private GameObject InitializeFollowCamera()
        {
            GameObject follow = new GameObject
            {
                Name = "Follow"
            };

            FollowCameraComponent followComponent = new FollowCameraComponent
            {
                DefaultPosition = Vector3.Zero,
                Camera = ServiceLocator.Instance.GetService<Camera>(),
                FollowObject = ServiceLocator.Instance.GetService<GameObject>("Player"),
                FollowOffset = new Vector3(0, -1.5, -2),
                IsFollowingZ = true
            };

            follow.AddComponent(followComponent);

            return follow;
        }

        private GameObject InitializePause()
        {
            GameObject pause = new GameObject
            {
                Name = "Pause"
            };

            // Position on the Z so it gets drawn in front of everything
            pause.Transform.Position = new Vector3(0, 0, -100);

            PauseGameComponent pauseComponent = new PauseGameComponent
            {
                TextureFilePath = GameSettings.PauseScreenTextureFilePath
            };
            pause.AddComponent(pauseComponent);

            return pause;
        }
    }
}
