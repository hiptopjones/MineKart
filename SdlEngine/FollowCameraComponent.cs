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

        private bool IsFollowing { get; set; }

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
                Camera.Position = FollowObject.Transform.Position + FollowOffset;
            }
            else
            {
                Camera.Position = new Vector3(0, 0, 0);
            }
        }
    }
}
