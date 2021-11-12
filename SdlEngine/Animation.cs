using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class AnimationFrame
    {
        // TODO: Could use an ID or something instead?
        public string TextureFilePath { get; set; }
        public Rect3 ClippingRect { get; set; }
        public double FrameTime { get; set; }
    }

    public class Animation
    {
        private List<AnimationFrame> Frames { get; set; } = new List<AnimationFrame>();

        private int CurrentFrameIndex { get; set; }
        private double CurrentFrameTime { get; set; }

        public void AddFrame(string textureFilePath, Rect3 clippingRect, double frameTime)
        {
            AnimationFrame frame = new AnimationFrame
            {
                TextureFilePath = textureFilePath,
                ClippingRect = clippingRect,
                FrameTime = frameTime
            };

            Frames.Add(frame);
        }

        public AnimationFrame GetCurrentFrame()
        {
            if (Frames.Count > 0)
            {
                return Frames[CurrentFrameIndex];
            }

            return null;
        }

        public bool UpdateFrame()
        {
            AnimationFrame frame = GetCurrentFrame();

            CurrentFrameTime += Time.DeltaTime;
            if (CurrentFrameTime >= frame.FrameTime)
            {
                CurrentFrameTime -= frame.FrameTime;
                CurrentFrameIndex = (CurrentFrameIndex + 1) % Frames.Count;

                return true;
            }

            return false;
        }

        public void Reset()
        {
            CurrentFrameIndex = 0;
            CurrentFrameTime = 0;
        }
    }
}
