using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class Collision
    {
        public GameObject FromObject { get; set; }
        public GameObject ToObject { get; set; }
        public Vector3 Overlap { get; set; }
    }

    public abstract class ColliderComponent : Component
    {
        public int Layer { get; set; } // Player, track, decoration
        public Collider Collider { get; set; }

        public abstract bool Intersects(ColliderComponent colliderComponent, out Collision collision);
    }
}
