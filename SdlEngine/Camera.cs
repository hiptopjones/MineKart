using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class Camera
    {
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);

        public int Width { get; private set; }
        public int Height { get; private set; }

        public double AspectRatio { get; private set; }

        public double FieldOfViewDegrees { get; private set; }
        public double FocalDistance { get; private set; }

        public int DrawDistance { get; private set; }

        public Camera(int width, int height, double fieldOfViewDegrees, int drawDistance)
        {
            Width = width;
            Height = height;
            FieldOfViewDegrees = fieldOfViewDegrees;
            DrawDistance = drawDistance;

            Initialize();
        }

        private void Initialize()
        {
            AspectRatio = Width / (double)Height;

            // NOTE: FOV is the same in both X and Y
            double fieldOfViewRadians = Utilities.Deg2Rad(FieldOfViewDegrees);
            FocalDistance = 1 / Math.Tan(fieldOfViewRadians / 2);
        }

        public Vector3 ProjectPointToScreen(Vector3 subjectWorldPosition)
        {
            // TODO: Ignore points outside of the draw distance

            Vector3 cameraWorldPosition = Position;
            Vector3 subjectCameraPosition = subjectWorldPosition - cameraWorldPosition;

            double scale = FocalDistance / subjectCameraPosition.Z;

            Vector3 subjectScreenPosition = new Vector3
            {
                X = (Width / 2) * subjectCameraPosition.X * scale + Width / 2,
                Y = (Height / 2) * subjectCameraPosition.Y * AspectRatio * scale + Height / 2,
                Z = 0 // unused
            };

            return subjectScreenPosition;
        }

        // Texture rect is neither world nor screen values.  It's the unscaled pixel bounds of a texture.
        public Rect3 ProjectSpriteToScreen(Vector3 subjectWorldPosition, Rect3 subjectTextureRect)
        {
            // TODO: Ignore points outside of the draw distance

            Vector3 cameraWorldPosition = Position;
            Vector3 subjectCameraPosition = subjectWorldPosition - cameraWorldPosition;

            double scale = FocalDistance / subjectCameraPosition.Z;

            Rect3 subjectScreenRect = new Rect3
            {
                X = (Width / 2) * subjectCameraPosition.X * scale + Width / 2,
                Y = (Height / 2) * subjectCameraPosition.Y * AspectRatio * scale + Height / 2,
                Width = subjectTextureRect.Width * scale,
                Height = subjectTextureRect.Height * scale
            };

            return subjectScreenRect;
        }

        public Rect3 ProjectRectToScreen(Rect3 subjectWorldRect)
        {
            // TODO: Ignore points outside of the draw distance

            Vector3 cameraWorldPosition = Position;
            Vector3 subjectCameraPosition = subjectWorldRect.GetPosition() - cameraWorldPosition;

            double scale = FocalDistance / subjectCameraPosition.Z;

            Rect3 subjectScreenRect = new Rect3
            {
                X = (Width / 2) * subjectCameraPosition.X * scale + Width / 2,
                Y = (Height / 2) * subjectCameraPosition.Y * AspectRatio * scale + Height / 2,
                Width = (Width / 2) * subjectWorldRect.Width * scale,
                Height = (Height / 2) * subjectWorldRect.Height * AspectRatio * scale
            };

            return subjectScreenRect;
        }
    }
}
