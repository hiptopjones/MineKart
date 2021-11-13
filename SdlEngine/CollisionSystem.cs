using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    class CollisionSystem
    {
        private Quadtree CollisionTree { get; set; }
        private Dictionary<GameObject, QuadtreeEntry> EntryMap { get; set; } = new Dictionary<GameObject, QuadtreeEntry>();

        public CollisionSystem()
        {
            CollisionTree = new Quadtree
            {
                // TODO: What's the right way to handle the bounding box?
                // TODO: Should be a sliding window around the player?
                BoundingBox = new Rect3(0, 0, 100, 100),
                MaxEntries = 10
            };
        }

        public void Update(List<GameObject> collidableObjects)
        {
            CollisionTree.Clear();

            foreach (GameObject collidableObject in collidableObjects)
            {
                BoxColliderComponent boxColliderComponent = collidableObject.GetComponent<BoxColliderComponent>();

                Rect3 boundingBox = new Rect3
                {
                    // Converting from Z/Y to X/Y here (minekart specific code)
                    X = collidableObject.Transform.Position.Z + boxColliderComponent.BoundingBox.Z,
                    Y = collidableObject.Transform.Position.Y + boxColliderComponent.BoundingBox.Y,
                    Width = boxColliderComponent.BoundingBox.Depth,
                    Height = boxColliderComponent.BoundingBox.Height
                };

                QuadtreeEntry quadtreeEntry = new QuadtreeEntry
                {
                    BoundingBox = boundingBox,
                    Context = boxColliderComponent
                };

                EntryMap[collidableObject] = quadtreeEntry;
                CollisionTree.Add(quadtreeEntry);
            }

            CheckCollisions(collidableObjects);
        }

        private void CheckCollisions(List<GameObject> collidableObjects)
        {
            foreach (GameObject collidableObject in collidableObjects)
            {
                BoxColliderComponent boxColliderComponent1 = collidableObject.GetComponent<BoxColliderComponent>();

                // Find nearby objects to check collisions with
                List<QuadtreeEntry> candidateEntries = CollisionTree.Search(EntryMap[collidableObject].BoundingBox);

                foreach (QuadtreeEntry quadtreeEntry in candidateEntries)
                {
                    BoxColliderComponent boxColliderComponent2 = (BoxColliderComponent)quadtreeEntry.Context;

                    // Don't collide with ourselves
                    if (boxColliderComponent1 == boxColliderComponent2)
                    {
                        continue;
                    }

                    Collision collision;
                    if (boxColliderComponent1.Intersects(boxColliderComponent2, out collision))
                    {
                        // TODO: Raise some event so the collision can be handled meaningfully
                        Debug.DrawText($"{boxColliderComponent1.Owner.Name} -> {boxColliderComponent2.Owner.Name}");
                    }
                }
            }
        }
    }
}
