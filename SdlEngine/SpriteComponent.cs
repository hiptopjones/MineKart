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
        public bool IsFlipped { get; set; }

        public Vector3 DrawPosition { get; set; } // Position to render at
        public bool UseTransformPosition { get; set; } = true; // If true, DrawPosition will follow Transform.Position

        // Private variables below here
        private Texture SpriteTexture { get; set; } // TODO: Use a weak reference?

        private Camera Camera { get; set; }
        private GraphicsManager GraphicsManager { get; set; }
        private ResourceManager ResourceManager { get; set; }

        public override void Awake()
        {
            Camera = ServiceLocator.Instance.GetService<Camera>();
            if (Camera == null)
            {
                throw new Exception($"Unable to retrieve camera from service locator");
            }

            GraphicsManager = ServiceLocator.Instance.GetService<GraphicsManager>();
            if (GraphicsManager == null)
            {
                throw new Exception($"Unable to retrieve graphics manager from service locator");
            }

            ResourceManager = ServiceLocator.Instance.GetService<ResourceManager>();
            if (ResourceManager == null)
            {
                throw new Exception($"Unable to retrieve resource manager from service locator");
            }
        }

        public override void Start()
        {
            RefreshTexture();
        }

        public override void Render()
        {
            if (UseTransformPosition)
            {
                DrawPosition = Owner.Transform.Position;
            }

            Rect3 clippingRect = ClippingRect ?? new Rect3
            {
                X = 0,
                Y = 0,
                Width = SpriteTexture.Width,
                Height = SpriteTexture.Height
            };

            Rect3 projectedTargetRect = Camera.ProjectSpriteToScreen(DrawPosition, clippingRect);

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

            SpriteTexture.Render(GraphicsManager.RendererHandle, sourceRect, targetRect, IsFlipped);
        }

        public void RefreshTexture()
        {
            SpriteTexture = ResourceManager.GetTexture(TextureFilePath);
        }
    }
}
