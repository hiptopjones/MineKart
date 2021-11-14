using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class BasicMovementComponent : Component
    {
        // Clamp position to these bounds
        public Rect3 BoundingBox { get; set; }

        public Vector3 UpDownVelocity { get; set; }
        public Vector3 LeftRightVelocity { get; set; }
        public Vector3 InOutVelocity { get; set; }

        // Used when Ctrl pressed
        public double CtrlSpeedMultiplier { get; set; }

        // Used when Shift pressed
        public double ShiftSpeedMultiplier { get; set; }

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
            Vector3 velocity = new Vector3();

            double speedMultiplier = 1;
            if (EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_LCTRL))
            {
                speedMultiplier = CtrlSpeedMultiplier;
            }
            else if (EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_LSHIFT))
            {
                speedMultiplier = ShiftSpeedMultiplier;
            }

            if (EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_LEFT))
            {
                velocity -= LeftRightVelocity;
            }
            else if (EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_RIGHT))
            {
                velocity += LeftRightVelocity;
            }

            if (EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_UP))
            {
                velocity -= UpDownVelocity;
            }
            else if (EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_DOWN))
            {
                velocity += UpDownVelocity;
            }

            if (EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_a))
            {
                velocity += InOutVelocity;
            }
            else if (EventManager.IsKeyPressed(SDL.SDL_Keycode.SDLK_z))
            {
                velocity -= InOutVelocity;
            }

            TransformComponent transform = Owner.Transform;
            transform.Position += velocity * Time.DeltaTime * speedMultiplier;

            transform.Position = BoundingBox.Clamp(transform.Position);
        }
    }
}
