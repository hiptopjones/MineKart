using NLog;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class SpriteComponent : DrawableComponent
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        // Configuration
        public string TextureFilePath { get; set; }
        public Rect3? ClippingRect { get; set; }

        public Vector3 NormalizedOrigin { get; set; } = new Vector3(0, 0);

        // Private variables below here
        private Texture SpriteTexture { get; set; } // TODO: Use a weak reference?

        public override void Start()
        {
            RefreshTexture();
        }

        public override void Render()
        {
            Camera camera = ServiceLocator.Instance.GetService<Camera>();

            GraphicsManager graphicsManager = ServiceLocator.Instance.GetService<GraphicsManager>();
            TransformComponent transform = Owner.Transform;

            Rect3 clippingRect = ClippingRect ?? new Rect3
            {
                X = 0,
                Y = 0,
                Width = SpriteTexture.Width,
                Height = SpriteTexture.Height
            };

            Rect3 projectedTargetRect = camera.ProjectSpriteToScreen(transform.Position, clippingRect);

            Vector3 origin = new Vector3
            {
                X = NormalizedOrigin.X * projectedTargetRect.Width,
                Y = NormalizedOrigin.Y * projectedTargetRect.Height
            };

            SDL.SDL_Rect sourceRect = new SDL.SDL_Rect
            {
                x = (int)clippingRect.X,
                y = (int)clippingRect.Y,
                w = (int)clippingRect.Width,
                h = (int)clippingRect.Height
            };

            SDL.SDL_Rect targetRect = new SDL.SDL_Rect
            {
                x = (int)(projectedTargetRect.X - origin.X),
                y = (int)(projectedTargetRect.Y - origin.Y),
                w = (int)projectedTargetRect.Width,
                h = (int)projectedTargetRect.Height
            };

            SpriteTexture.Render(graphicsManager.RendererHandle, sourceRect, targetRect, isFlipped: false);
        }

        public void RefreshTexture()
        {
            ResourceManager resourceManager = ServiceLocator.Instance.GetService<ResourceManager>();
            SpriteTexture = resourceManager.GetTexture(TextureFilePath);
        }
    }
}
