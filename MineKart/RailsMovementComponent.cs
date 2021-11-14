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
        public double JumpInitialSpeed { get; set; }
        public double DeathInitialSpeed { get; set; }
        public double GravityAcceleration { get; set; }

        public bool IsBraking { get; set; }
        public bool IsJumping { get; set; }
        public bool IsFalling { get; set; }

        public double CurrentVerticalSpeed { get; set; }

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

            if (IsJumping || IsFalling)
            {
                velocity.Y = CurrentVerticalSpeed;
            }
            else
            {
                IsBraking = EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_x) || EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_DOWN);

                if (EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_c) || EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_UP))
                {
                    IsJumping = true;
                    velocity.Y = JumpInitialSpeed;
                }
            }

            if (IsFalling)
            {
                velocity.Z = 0;
            }
            else if (IsJumping)
            {
                velocity.Z = NormalForwardSpeed;
            }
            else if (IsBraking)
            {
                // TODO: Throw sparks, make noise
                velocity.Z = BrakeForwardSpeed;
            }
            else
            {
                velocity.Z = NormalForwardSpeed;
            }

            transform.Position += velocity * Time.DeltaTime;

            if (IsJumping || IsFalling)
            {
                CurrentVerticalSpeed = velocity.Y + GravityAcceleration * Time.DeltaTime;
            }
        }

        public void StartFalling()
        {
            IsJumping = false;
            IsBraking = false;

            IsFalling = true;
            CurrentVerticalSpeed = DeathInitialSpeed;
        }

        public void StopJumping()
        {
            if (IsFalling || false == IsJumping)
            {
                return;
            }

            IsJumping = false;

            TransformComponent transform = Owner.Transform;
            transform.Position = new Vector3(transform.Position.X, 1, transform.Position.Z);

            // TODO: Throw sparks, make noise, animate rumble
        }
    }
}
