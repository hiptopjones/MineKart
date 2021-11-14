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

        // Active collisions
        private Dictionary<string, KeyValuePair<GameObject, GameObject>> CollisionMap { get; set; } = new Dictionary<string, KeyValuePair<GameObject, GameObject>>();

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
            HashSet<string> checkedCollisions = new HashSet<string>();

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

                    GameObject gameObject1 = boxColliderComponent1.Owner;
                    GameObject gameObject2 = boxColliderComponent2.Owner;
                    string key = MakeKey(gameObject1, gameObject2);

                    // Enforce only checking a pair of objects once per frame
                    if (checkedCollisions.Contains(key))
                    {
                        continue;
                    }

                    checkedCollisions.Add(key);

                    Collision collision;
                    bool isColliding = boxColliderComponent1.Intersects(boxColliderComponent2, out collision);
                    bool wasColliding = CollisionMap.ContainsKey(key);

                    if (isColliding || wasColliding)
                    {
                        if (isColliding)
                        {
                            Debug.DrawText($"Collision Enter/Stay: {gameObject1.Name} -> {gameObject2.Name}");
                            CollisionMap[key] = new KeyValuePair<GameObject, GameObject>(gameObject1, gameObject2);
                        }
                        else
                        {
                            Debug.DrawText($"Collision Exit: {gameObject1.Name} -> {gameObject2.Name}");
                            CollisionMap.Remove(key);
                        }

                        // Raise events in both directions so the collision can be handled meaningfully
                        NotifyCollisionEvent(isColliding, wasColliding, gameObject1, gameObject2);
                        NotifyCollisionEvent(isColliding, wasColliding, gameObject2, gameObject1);
                    }
                }
            }
        }

        private void NotifyCollisionEvent(bool isColliding, bool wasColliding, GameObject sourceGameObject, GameObject targetGameObject)
        {
            CollisionHandlerComponent collisionHandlerComponent = targetGameObject.GetComponent<CollisionHandlerComponent>();
            if (collisionHandlerComponent != null)
            {
                if (isColliding)
                {
                    if (wasColliding)
                    {
                        Debug.DrawText($"Collision Stay: {sourceGameObject.Name} -> {targetGameObject.Name}");
                        collisionHandlerComponent.OnCollisionStay(sourceGameObject);
                    }
                    else
                    {
                        Debug.DrawText($"Collision Enter: {sourceGameObject.Name} -> {targetGameObject.Name}");
                        collisionHandlerComponent.OnCollisionEnter(sourceGameObject);
                    }
                }
                else
                {
                    if (wasColliding)
                    {
                        // TODO: Objects that get removed from the scene may not receive an exit call
                        Debug.DrawText($"Collision Exit: {sourceGameObject.Name} -> {targetGameObject.Name}");
                        collisionHandlerComponent.OnCollisionExit(sourceGameObject);
                    }
                }
            }
        }

        // Creates a key that uniquely represents a pair of objects, regardless of the order they are provided
        // TODO: Consider Szudsik's elegant pairing algorithm for this (more efficient)
        private string MakeKey(GameObject gameObject1, GameObject gameObject2)
        {
            if (gameObject1.Id < gameObject2.Id)
            {
                return $"{gameObject1.Id}-{gameObject2.Id}";
            }
            else
            {
                return $"{gameObject2.Id}-{gameObject1.Id}";
            }
        }
    }
}
