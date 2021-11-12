using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    class CollisionSystem
    {
        public void ProcessCollisions(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject1 in gameObjects)
            {
                ColliderComponent colliderComponent1 = gameObject1.GetComponent<ColliderComponent>();

                foreach (GameObject gameObject2 in gameObjects)
                {
                    ColliderComponent colliderComponent2 = gameObject2.GetComponent<ColliderComponent>();
                    if (colliderComponent1 == colliderComponent2)
                    {
                        continue;
                    }

                    Collision collision;
                    if (colliderComponent1.Intersects(colliderComponent2, out collision))
                    {
                        // TODO: Raise some event so the collision can be handled meaningfully
                    }
                }
            }
        }
    }
}
