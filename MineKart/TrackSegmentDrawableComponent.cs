using SDL2;
using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    public class TrackSegmentDrawableComponent : DrawableComponent
    {
        // Maintaining a "shadow" world position for rendering use
        public Vector3 DrawPosition { get; set; }
        public Vector3 DrawVelocity { get; set; }
        public string TextureFilePath { get; set; }

        private TrackSegmentComponent TrackSegment { get; set; }
        private GameObject Player { get; set; }
        private Camera Camera { get; set; }
        private GraphicsManager GraphicsManager { get; set; }

        private Texture SegmentTexture { get; set; }

        public override void Awake()
        {
            TrackSegment = Owner.GetComponent<TrackSegmentComponent>();
            if (TrackSegment == null)
            {
                throw new Exception($"Unable to retrieve component: {nameof(TrackSegmentComponent)}");
            }

            Player = ServiceLocator.Instance.GetService<GameObject>("Player");
            if (Player == null)
            {
                throw new Exception($"Unable to retrieve player from service locator");
            }

            ResourceManager resourceManager = ServiceLocator.Instance.GetService<ResourceManager>();
            if (resourceManager == null)
            {
                throw new Exception($"Unable to retrieve resource manager from service locator");
            }

            SegmentTexture = resourceManager.GetTexture(TextureFilePath);
            if (SegmentTexture == null)
            {
                throw new Exception($"Unable to load segment texture from file");
            }

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
        }

        // This code assumes the segment components are ordered front-to-back
        public override void Update()
        {
            // Goal is to have these segments to render themselves independently (no management system)
            //  - This makes it easy to place other objects in the world and have them rendered correctly on top of the track
            //  - However, the discrete nature of this architecture can be hard to reason about
            //  - The manipulation is split between Update() and Render()
            //  - The draw positions needs to be calculated front-to-back, but the drawing needs to be done back-to-front
            //  - The lack of a management system makes looping the track harder

            // Update algorithm
            //  - Any segment behind the player is set to draw at the segment's default position (no curves or hills are applied)
            //  - Accumulation of curves and hills starts from the segment currently under the player
            //  - The logical segment under the player is directly behind them
            //  - If the player is at Z = 0.124, then the current segment is #0, which will be behind them
            //  - However that surface will be drawn by segment #1, eliminating Z-ordering problems

            TransformComponent transform = Owner.Transform;

            DrawPosition = transform.Position;
            DrawVelocity = Vector3.Zero;

            // Runs for only the segments under and ahead of the player
            if (Player.Transform.Position.Z < transform.Position.Z)
            {
                double playerFractionalZ = Player.Transform.Position.Z - (transform.Position.Z - 1);
                if (playerFractionalZ > 0)
                {
                    // Runs for only the segment under the player
                    DrawVelocity -= TrackSegment.Curvature * playerFractionalZ;
                    DrawPosition -= DrawVelocity * playerFractionalZ + TrackSegment.Curvature * 0.5 * playerFractionalZ * playerFractionalZ;
                }
                else
                {
                    TrackSegmentDrawableComponent previousDrawableComponent = TrackSegment.PreviousSegment.Owner.GetComponent<TrackSegmentDrawableComponent>();
                    DrawVelocity = previousDrawableComponent.DrawVelocity;
                    DrawPosition = new Vector3(previousDrawableComponent.DrawPosition.X, previousDrawableComponent.DrawPosition.Y, transform.Position.Z);
                }

                DrawPosition += DrawVelocity * 0.5;
                DrawVelocity += TrackSegment.Curvature;
            }
        }

        // This assumes all segment components have up-to-date DrawPositions and are linked together
        public override void Render()
        {
            TransformComponent transform = Owner.Transform;

            if (transform.Position.Z < Camera.Position.Z ||
                transform.Position.Z > Camera.Position.Z + Camera.DrawDistance)
            {
                // Ignore objects behind the camera or forward of the draw distance
                // TODO: Consider looping tracks
                return;
            }

            RenderRoad();
            RenderDebug();
        }

        private void RenderRoad()
        {
            // In this context the "previous" segment is the one with a lower Z (higher projected Y)
            TrackSegmentDrawableComponent previousDrawableComponent = TrackSegment.PreviousSegment.Owner.GetComponent<TrackSegmentDrawableComponent>();
            Vector3 previousSegmentDrawPosition = previousDrawableComponent.DrawPosition;
            Vector3 currentSegmentDrawPosition = DrawPosition;

            Rect3 sourceRect = SegmentTexture.GetRect();
            sourceRect.Height = 1; // Limit to one pixel high

            // Project the start and end of the segment, then we'll lerp between them
            Rect3 previousSegmentScreenRect = Camera.ProjectSpriteToScreen(previousSegmentDrawPosition, sourceRect);
            Rect3 currentSegmentScreenRect = Camera.ProjectSpriteToScreen(currentSegmentDrawPosition, sourceRect);

            if (false == Utilities.InRange(currentSegmentScreenRect.Y, 0, Camera.Height))
            {
                // Exit early if the current segment isn't on screen
                return;
            }

            Vector3 screenDeltaPosition = previousSegmentScreenRect.GetPosition() - currentSegmentScreenRect.GetPosition();
            if (Math.Round(screenDeltaPosition.Y) <= 0 || double.IsInfinity(screenDeltaPosition.Y))
            {
                // Handle degenerate cases
                return;
            }

            //Debug.DrawText($"Segment {TrackSegment.SegmentId}: {currentSegmentScreenRect}");

            Vector3 screenDeltaPositionStep = screenDeltaPosition / screenDeltaPosition.Y;
            Vector3 screenDeltaPositionAccumulated = Vector3.Zero;

            double screenDeltaWidth = previousSegmentScreenRect.Width - currentSegmentScreenRect.Width;
            double screenDeltaWidthStep = screenDeltaWidth / screenDeltaPosition.Y;
            double screenDeltaWidthAccumulated = 0;

            for (int i = 0; i < screenDeltaPosition.Y; i++)
            {
                Rect3 steppedScreenRect = currentSegmentScreenRect + screenDeltaPositionAccumulated;
                steppedScreenRect.Width += screenDeltaWidthAccumulated;
                steppedScreenRect.Height = 1; // Leave source rect height unscaled

                if (false == Utilities.InRange(steppedScreenRect.Y, 0, Camera.Height))
                {
                    // Exit early if we're starting to draw outside of the camera
                    break;
                }

                screenDeltaPositionAccumulated += screenDeltaPositionStep;
                screenDeltaWidthAccumulated += screenDeltaWidthStep;

                Vector3 textureOrigin = new Vector3
                {
                    X = steppedScreenRect.Width / 2,
                    Y = 0
                };

                sourceRect.Y = i / screenDeltaPosition.Y * SegmentTexture.Height;
                
                SDL.SDL_Rect sourceSdlRect = sourceRect.ToSdlRect();
                SDL.SDL_Rect targetSdlRect = (steppedScreenRect - textureOrigin).ToSdlRect();

                SegmentTexture.Render(GraphicsManager.RendererHandle, sourceSdlRect, targetSdlRect, isFlipped: false);
            }
        }

        public void RenderDebug()
        {
            Vector3 worldLeft = DrawPosition - new Vector3(1, 0, 0);
            Vector3 worldRight = DrawPosition + new Vector3(1, 0, 0);

            Vector3 screenLeft = Camera.ProjectPointToScreen(worldLeft);
            Vector3 screenRight = Camera.ProjectPointToScreen(worldRight);

            Color debugColor = TrackSegment.SegmentId == 0 ? Color.Cyan : Color.Yellow;
            Debug.DrawLine(screenLeft, screenRight, debugColor);
        }
    }
}
