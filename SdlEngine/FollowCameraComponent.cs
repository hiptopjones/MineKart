using SDL2;
using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class FollowCameraComponent : Component
    {
        public Camera Camera { get; set; }
        public GameObject FollowObject { get; set; }
        public Vector3 FollowOffset { get; set; }
        public Vector3 DefaultPosition { get; set; }

        public bool IsFollowingX { get; set; }
        public bool IsFollowingY { get; set; }
        public bool IsFollowingZ { get; set; }

        private bool IsFollowing { get; set; } = true;

        public override void Awake()
        {
            if (Camera == null)
            {
                throw new Exception("No camera configured");
            }

            if (FollowObject == null)
            {
                throw new Exception("No follow object configured");
            }
        }

        public override void Update()
        {
            EventManager eventManager = ServiceLocator.Instance.GetService<EventManager>();

            if (eventManager.IsKeyDown(SDL.SDL_Keycode.SDLK_f))
            {
                IsFollowing = !IsFollowing;
            }
        }
        
        public override void LateUpdate()
        {
            if (IsFollowing)
            {
                Vector3 cameraPosition = Camera.Position;

                if (IsFollowingX)
                {
                    cameraPosition.X = FollowObject.Transform.Position.X + FollowOffset.X;
                }
                if (IsFollowingY)
                {
                    cameraPosition.Y = FollowObject.Transform.Position.Y + FollowOffset.Y;
                }
                if (IsFollowingZ)
                {
                    cameraPosition.Z = FollowObject.Transform.Position.Z + FollowOffset.Z;
                }

                Camera.Position = cameraPosition;
            }
            else
            {
                Camera.Position = DefaultPosition;
            }
        }
    }
}
