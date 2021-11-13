using SDL2;
using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    public class RailsMovementComponent : Component
    {
        public double NormalForwardSpeed { get; set; }
        public double BrakeForwardSpeed { get; set; }
        public double JumpSpeed { get; set; }
        public double GravityAcceleration { get; set; }

        public bool IsBraking { get; set; }
        public bool IsJumping { get; set; }

        public double VerticalSpeed { get; set; }

        private EventManager EventManager { get; set; }

        public override void Awake()
        {
            EventManager = ServiceLocator.Instance.GetService<EventManager>();
            if (EventManager == null)
            {
                throw new Exception($"Unable to retrieve event manager from service locator");
            }
        }

        public override void Update()
        {
            TransformComponent transform = Owner.Transform;

            Vector3 velocity = Vector3.Zero;

            if (IsJumping)
            {
                // TODO: Need a way to get the current segment (for detecting ground, and for checking collisions)
                if (transform.Position.Y < 1)
                {
                    velocity.Y = VerticalSpeed;
                }
                else
                {
                    IsJumping = false;

                    // TODO: Set position back onto track
                    transform.Position = new Vector3(transform.Position.X, 1, transform.Position.Z);

                    // TODO: Throw sparks, make noise, animate rumble
                }
            }
            else
            {
                IsBraking = EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_DOWN);

                if (EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_SPACE))
                {
                    IsJumping = true;
                    velocity.Y = JumpSpeed;
                }
            }

            if (IsBraking)
            {
                // TODO: Throw sparks, make noise
                velocity.Z = BrakeForwardSpeed;
            }
            else
            {
                velocity.Z = NormalForwardSpeed;
            }

            transform.Position += velocity * Time.DeltaTime;

            if (IsJumping)
            {
                VerticalSpeed = velocity.Y + GravityAcceleration * Time.DeltaTime;
            }
        }
    }
}
