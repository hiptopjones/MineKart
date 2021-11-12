using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class AnimationComponent : Component
    {
        private const int ANIMATION_STATE_NONE = -1;

        private SpriteComponent SpriteComponent { get; set; }

        private int CurrentAnimationState { get; set; } = ANIMATION_STATE_NONE;
        private Dictionary<int, Animation> AnimationMap { get; set; } = new Dictionary<int, Animation>();

        public void AddAnimation(int animationState, Animation animation)
        {
            AnimationMap[animationState] = animation;
        }

        public void SetAnimationState(int animationState)
        {
            CurrentAnimationState = animationState;

            Animation animation = GetCurrentAnimation();
            if (animation != null)
            {
                animation.Reset();

                AnimationFrame frame = animation.GetCurrentFrame();
                UpdateSpriteComponent(frame);
            }
        }

        public Animation GetCurrentAnimation()
        {
            if (CurrentAnimationState == ANIMATION_STATE_NONE)
            {
                return null;
            }

            return AnimationMap[CurrentAnimationState];
        }

        public override void Awake()
        {
            SpriteComponent = Owner.GetComponent<SpriteComponent>();
            if (SpriteComponent == null)
            {
                throw new Exception("Unable to locate a SpriteComponent");
            }
        }

        public override void Start()
        {
            Animation animation = GetCurrentAnimation();
            if (animation == null)
            {
                return;
            }

            AnimationFrame frame = animation.GetCurrentFrame();
            UpdateSpriteComponent(frame);
        }

        public override void Update()
        {
            Animation animation = GetCurrentAnimation();
            if (animation == null)
            {
                return;
            }

            bool newFrame = animation.UpdateFrame();
            if (newFrame)
            {
                AnimationFrame frame = animation.GetCurrentFrame();
                UpdateSpriteComponent(frame);
            }
        }

        private void UpdateSpriteComponent(AnimationFrame frame)
        {
            SpriteComponent.TextureFilePath = frame.TextureFilePath;
            SpriteComponent.ClippingRect = frame.ClippingRect;

            // Tell the sprite component that we might have updated the texture file
            SpriteComponent.RefreshTexture();
        }
    }
}
