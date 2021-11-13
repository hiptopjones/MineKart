using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class QuadtreeEntry
    {
        public Rect3 BoundingBox { get; set; }
        public object Context { get; set; }
    }

    // Recursive data structure
    public class Quadtree
    {
        // Couple of things about this code...
        // - Quadtrees are 2D, and for minekart the critical axes are Y/Z
        // - To maximize understanding, we'll keep this quadtree code working in X/Y
        // - New terrain is constantly being generated, so the bounds of the quadtree need to continually change
        // - Small movements along the third dimension is possible (leaning, tilting, etc.) but can be flattened for purposes of the quadtree

        private const int CurrentTree = -1;
        private const int NorthwestChild = 0;
        private const int NortheastChild = 1;
        private const int SouthwestChild = 2;
        private const int SoutheastChild = 3;

        public Rect3 BoundingBox { get; set; }
        public int MaxEntries { get; set; }

        private Quadtree Parent { get; set; }
        private Quadtree[] Children { get; set; } = new Quadtree[4];
        private List<QuadtreeEntry> Entries { get; set; } = new List<QuadtreeEntry>();
        private int CurrentLevel { get; set; }

        public void Add(QuadtreeEntry entry)
        {
            if (Children[0] != null) // Any children defined?
            {
                int childIndex = GetChildIndex(entry.BoundingBox);
                if (childIndex != CurrentTree)
                {
                    Children[childIndex].Add(entry);
                    return;
                }
            }

            Entries.Add(entry);

            if (Entries.Count > MaxEntries)
            {
                Split();

                // Avoid forearch as we're mutating the list
                // Use swap-and-pop to avoid shifting elements
                int i = 0;
                while (i < Entries.Count)
                {
                    int childIndex = GetChildIndex(Entries[i].BoundingBox);
                    if (childIndex != CurrentTree)
                    {
                        Children[childIndex].Add(Entries[i]);

                        int lastIndex = Entries.Count - 1;
                        Entries[i] = Entries[lastIndex];
                        Entries.RemoveAt(lastIndex);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        public void Remove(QuadtreeEntry entry)
        {
            int childIndex = GetChildIndex(entry.BoundingBox);
            if (childIndex == CurrentTree)
            {
                // Avoid forearch as we're mutating the list
                // Use swap-and-pop to avoid shifting elements
                int i = 0;
                while (i < Entries.Count)
                {
                    if (Entries[i] == entry)
                    {
                        int lastIndex = Entries.Count - 1;
                        Entries[i] = Entries[lastIndex];
                        Entries.RemoveAt(lastIndex);

                        // Exit early, once found
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            else
            {
                Children[childIndex].Remove(entry);
            }
        }

        public void Clear()
        {
            Entries.Clear();

            if (Children[0] != null) // Any children defined?
            {
                for (int i = 0; i < Children.Length; i++)
                {
                    Children[i].Clear();
                    Children[i] = null;
                }
            }
        }

        // This code assumes the bounding box passed in is an object's collider,
        // and leaves checking individual candidate objects to the collision system
        // However, if the bounding box passed in is just a region of interest, we might
        // do an additional pass here and prune the candidate list further to just those
        // objects intersecting the bounding box
        public List<QuadtreeEntry> Search(Rect3 boundingBox)
        {
            List<QuadtreeEntry> candidateEntries = new List<QuadtreeEntry>();
            candidateEntries.AddRange(Entries);

            if (Children[0] != null) // Any children defined?
            {
                int childIndex = GetChildIndex(boundingBox);
                if (childIndex == CurrentTree)
                {
                    for (int i = 0; i < Children.Length; i++)
                    {
                        if (Children[i].BoundingBox.IntersectsXY(boundingBox))
                        {
                            candidateEntries.AddRange(Children[i].Search(boundingBox));
                        }
                    }
                }
                else
                {
                    candidateEntries.AddRange(Children[childIndex].Search(boundingBox));
                }
            }

            return candidateEntries;
        }

        private int GetChildIndex(Rect3 boundingBox)
        {
            // If the provided bounding box doesn't fit completely within a region, stay at this level
            int childIndex = CurrentTree;

            double verticalDivider = BoundingBox.X + BoundingBox.Width / 2;
            double horizontalDivider = BoundingBox.Y + BoundingBox.Height / 2;

            bool isNorth = boundingBox.Y < horizontalDivider && boundingBox.Y + boundingBox.Height < horizontalDivider;
            bool isSouth = boundingBox.Y > horizontalDivider;
            bool isWest = boundingBox.X < verticalDivider && boundingBox.X + boundingBox.Width < verticalDivider;
            bool isEast = boundingBox.X > verticalDivider;

            if (isWest)
            {
                if (isNorth)
                {
                    childIndex = NorthwestChild;
                }
                else if (isSouth)
                {
                    childIndex = SouthwestChild;
                }
            }
            else if (isEast)
            {
                if (isNorth)
                {
                    childIndex = NortheastChild;
                }
                else if (isSouth)
                {
                    childIndex = SoutheastChild;
                }
            }

            return childIndex;
        }

        private void Split()
        {
            int childWidth = (int)(BoundingBox.Width / 2);
            int childHeight = (int)(BoundingBox.Height / 2);

            Children[NorthwestChild] = new Quadtree
            {
                CurrentLevel = CurrentLevel + 1,
                Parent = this,
                MaxEntries = MaxEntries,
                BoundingBox = new Rect3(BoundingBox.X, BoundingBox.Y, childWidth, childHeight)
            };

            Children[NortheastChild] = new Quadtree
            {
                CurrentLevel = CurrentLevel + 1,
                Parent = this,
                MaxEntries = MaxEntries,
                BoundingBox = new Rect3(BoundingBox.X + childWidth, BoundingBox.Y, childWidth, childHeight)
            };

            Children[SoutheastChild] = new Quadtree
            {
                CurrentLevel = CurrentLevel + 1,
                Parent = this,
                MaxEntries = MaxEntries,
                BoundingBox = new Rect3(BoundingBox.X + childWidth, BoundingBox.Y + childHeight, childWidth, childHeight)
            };

            Children[SouthwestChild] = new Quadtree
            {
                CurrentLevel = CurrentLevel + 1,
                Parent = this,
                MaxEntries = MaxEntries,
                BoundingBox = new Rect3(BoundingBox.X, BoundingBox.Y + childHeight, childWidth, childHeight)
            };
        }
    }
}
