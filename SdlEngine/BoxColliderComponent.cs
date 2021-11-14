using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class BoxColliderComponent : ColliderComponent
    {
        public Rect3 BoundingBox { get; set; }

        private Camera Camera { get; set; }

        public override void Awake()
        {
            Camera = ServiceLocator.Instance.GetService<Camera>();
            if (Camera == null)
            {
                throw new Exception($"Unable to retrieve camera from service locator");
            }
        }

        public override bool Intersects(ColliderComponent colliderComponent, out Collision collision)
        {
            BoxColliderComponent boxColliderComponent = colliderComponent as BoxColliderComponent;
            if (boxColliderComponent != null)
            {
                return Intersects(boxColliderComponent, out collision);
            }

            throw new Exception($"Unhandled collider component type: {colliderComponent.GetType()}");
        }

        public bool Intersects(BoxColliderComponent boxColliderComponent, out Collision collision)
        {
            collision = null;

            BoxCollider boxCollider1 = (BoxCollider)Collider;
            boxCollider1.BoundingBox = BoundingBox + Owner.Transform.Position;

            BoxCollider boxCollider2 = (BoxCollider)boxColliderComponent.Collider;
            boxCollider2.BoundingBox = boxColliderComponent.BoundingBox + boxColliderComponent.Owner.Transform.Position;

            Vector3 overlap;
            if (boxCollider1.Intersects(boxCollider2, out overlap))
            {
                collision = new Collision
                {
                    Overlap = overlap,
                    FromObject = Owner,
                    ToObject = boxColliderComponent.Owner
                };

                // This is a bunch of math, so don't bother if Debug isn't enabled
                if (Debug.IsEnabled)
                {
                    DrawBoundingBoxes(boxCollider1, boxCollider2);
                }

                return true;
            }

            return false;
        }

        private void DrawBoundingBoxes(BoxCollider boxCollider1, BoxCollider boxCollider2)
        {
            DrawBoundingBox(boxCollider1.BoundingBox);
            DrawBoundingBox(boxCollider2.BoundingBox);
        }

        // TODO: Move this to the Debug class?
        private void DrawBoundingBox(Rect3 boundingBox)
        {
            Rect3 frontRect = Camera.ProjectRectToScreen(boundingBox);
            Debug.DrawRect(frontRect, Color.Green);

            Rect3 backRect = Camera.ProjectRectToScreen(boundingBox + new Vector3(0, 0, boundingBox.Depth));
            Debug.DrawRect(backRect, Color.Red);

            Debug.DrawLine(frontRect.GetPosition(), backRect.GetPosition(), Color.Green);
            Debug.DrawLine(frontRect.GetPosition() + new Vector3(frontRect.Width, 0, 0), backRect.GetPosition() + new Vector3(backRect.Width, 0, 0), Color.Green);
            Debug.DrawLine(frontRect.GetPosition() + new Vector3(frontRect.Width, frontRect.Height, 0), backRect.GetPosition() + new Vector3(backRect.Width, backRect.Height, 0), Color.Green);
            Debug.DrawLine(frontRect.GetPosition() + new Vector3(0, frontRect.Height, 0), backRect.GetPosition() + new Vector3(0, backRect.Height, 0), Color.Green);
        }
    }
}
