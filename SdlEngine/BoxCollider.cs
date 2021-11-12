using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class BoxCollider : Collider
    {
        // Axis-aligned bounding box (AABB)
        public Rect3 BoundingBox { get; set; }

        public override bool Intersects(Collider collider, out Vector3 overlap)
        {
            BoxCollider boxCollider = collider as BoxCollider;
            if (boxCollider != null)
            {
                return Intersects(boxCollider, out overlap);
            }

            throw new Exception($"Unhandled collider type: {collider.GetType()}");
        }

        public bool Intersects(BoxCollider boxCollider, out Vector3 overlap)
        {
            overlap = default;

            if (BoundingBox.Intersects(boxCollider.BoundingBox))
            {
                overlap = new Vector3((BoundingBox.Center() - boxCollider.BoundingBox.Center()) / 2);
                return true;
            }

            return false;
        }
    }
}
