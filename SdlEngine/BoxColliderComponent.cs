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

                DrawBoundingBoxes(boxCollider1, boxCollider2);
                return true;
            }

            return false;
        }

        private void DrawBoundingBoxes(BoxCollider boxCollider1, BoxCollider boxCollider2)
        {
            Camera camera = ServiceLocator.Instance.GetService<Camera>();

            Rect3 projectedBoundingBox1 = camera.ProjectRectToScreen(boxCollider1.BoundingBox);
            Debug.DrawRect(projectedBoundingBox1);

            Rect3 projectedBoundingBox2 = camera.ProjectRectToScreen(boxCollider2.BoundingBox);
            Debug.DrawRect(projectedBoundingBox2);
        }
    }
}
