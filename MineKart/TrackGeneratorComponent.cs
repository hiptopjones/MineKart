using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    class TrackGeneratorComponent : Component
    {
        public int GenerateDistance { get; set; }  // Distance ahead of the player to generate
        public int PruneDistance { get; set; } // Distance it must be from player before segment can be pruned

        private TrackCollection TrackCollection { get; set; }

        private GameObject Player { get; set; }
        private GameObjectCollection SceneObjects { get; set; }

        private Random Random { get; set; } = new Random();

        public override void Awake()
        {
            TrackCollection = ServiceLocator.Instance.GetService<TrackCollection>();
            if (TrackCollection == null)
            {
                throw new Exception($"Unable to retrieve track collection from service locator");
            }

            Player = ServiceLocator.Instance.GetService<GameObject>("Player");
            if (Player == null)
            {
                throw new Exception($"Unable to retrieve player from service locator");
            }

            SceneObjects = ServiceLocator.Instance.GetService<GameObjectCollection>();
            if (SceneObjects == null)
            {
                throw new Exception($"Unable to retrieve game object collection from service locator");
            }
        }

        public override void Start()
        {
            // Start with some predetermined segments
            TrackSegmentComponent previousSegmentComponent = GenerateStraight(null, 10);
            GenerateHill(previousSegmentComponent, 20, -0.2);

            TrackCollection.FirstActiveObjectIndex = 0;
        }

        public override void Update()
        {
            PruneTrack();

            //Debug.DrawText($"Total GameObjects: {SceneObjects.GameObjects.Count}");
            //Debug.DrawText($"Active: {TrackCollection.TrackSegments.Count}");
            //Debug.DrawText($"First: {TrackCollection.FirstActiveObjectIndex}");
            //Debug.DrawText($"Last:  {TrackCollection.LastActiveObjectIndex}");

            GrowTrack();
        }

        private void PruneTrack()
        {
            // If we break in the debugger too long, the player could be way ahead of track generation.
            //  - Do not assume the player Z has track
            //  - Do not prune the last object in the list (used for previous elsewhere)
            for (int segmentId = TrackCollection.FirstActiveObjectIndex; segmentId < TrackCollection.LastActiveObjectIndex; segmentId++)
            {
                int playerZ = (int)Player.Transform.Position.Z;
                if (segmentId > playerZ - PruneDistance)
                {
                    break;
                }

                GameObject gameObject = TrackCollection.TrackSegments[segmentId];

                // Causes the object to be removed from the scene object collection
                gameObject.Destroy();

                if (segmentId != TrackCollection.FirstActiveObjectIndex)
                {
                    throw new Exception($"Segment ID mismatch: {segmentId} != {TrackCollection.FirstActiveObjectIndex}");
                }

                TrackCollection.TrackSegments.Remove(segmentId);
                TrackCollection.FirstActiveObjectIndex++;
            }
        }

        public void GrowTrack()
        {
            TrackSegmentComponent previousSegmentComponent = TrackCollection.TrackSegments[TrackCollection.LastActiveObjectIndex].GetComponent<TrackSegmentComponent>();

            while (TrackCollection.LastActiveObjectIndex < Player.Transform.Position.Z + GenerateDistance)
            {
                // TODO: Add percent settings to the GameSettings

                int percent = Random.Next(100);
                if (percent < 25)
                {
                    previousSegmentComponent = GenerateHole(previousSegmentComponent);
                }
                else if (percent < 50)
                {
                    int numSegments = Random.Next(10, 15);
                    double incline = Utilities.Lerp(0.1, 0.15, Random.NextDouble());
                    int sign = Random.Next(2) == 0 ? -1 : 1;
                    previousSegmentComponent = GenerateHill(previousSegmentComponent, numSegments, incline * sign);
                }
                else if (percent < 90)
                {
                    int numSegments = Random.Next(10, 15);
                    double curvature = Utilities.Lerp(0.15, 0.3, Random.NextDouble());
                    int sign = Random.Next(2) == 0 ? -1 : 1;
                    previousSegmentComponent = GenerateCurve(previousSegmentComponent, numSegments, curvature * sign);
                }
                else
                {
                    int numSegments = Random.Next(10, 15);
                    previousSegmentComponent = GenerateStraight(previousSegmentComponent, numSegments);
                }
            }
        }

        private TrackSegmentComponent GenerateStraight(TrackSegmentComponent previousSegmentComponent, int numSegments)
        {
            for (int i = 0; i < numSegments; i++)
            {
                previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.Track);
            }

            return previousSegmentComponent;
        }

        private TrackSegmentComponent GenerateHill(TrackSegmentComponent previousSegmentComponent, int numSegments, double incline)
        {
            int halfNumSegments = numSegments / 2;

            for (int i = 0; i < numSegments; i++)
            {
                // Ensures the hill carves out an S-curve, rather than a C-curve like a corner
                int inclineSign = (i < halfNumSegments ? 1 : -1);

                previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.Track, new Vector3(0, incline * inclineSign));
            }

            return previousSegmentComponent;
        }

        private TrackSegmentComponent GenerateCurve(TrackSegmentComponent previousSegmentComponent, int numSegments, double curvature)
        {
            for (int i = 0; i < numSegments; i++)
            {
                previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.Track, new Vector3(curvature, 0, 0));
            }

            return previousSegmentComponent;
        }

        private TrackSegmentComponent GenerateHole(TrackSegmentComponent previousSegmentComponent)
        {
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.Track);
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.Track);
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.TrackBreaking);
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.HoleEntering);
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.Hole);
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.Hole);
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.Hole);
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.TrackFixing);
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.Track);
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.Track);

            return previousSegmentComponent;
        }

        private TrackSegmentComponent GenerateDeadCart(TrackSegmentComponent previousSegmentComponent)
        {
            return previousSegmentComponent;
        }

        private TrackSegmentComponent AddTrackSegment(TrackSegmentComponent previousSegmentComponent, TrackSegmentType segmentType)
        {
            return AddTrackSegment(previousSegmentComponent, segmentType, Vector3.Zero);
        }

        private TrackSegmentComponent AddTrackSegment(TrackSegmentComponent previousSegmentComponent, TrackSegmentType segmentType, Vector3 curvature)
        {
            int segmentId = previousSegmentComponent == null ? 0 : previousSegmentComponent.SegmentId + 1;

            GameObject segment = new GameObject
            {
                Name = $"TrackSegment{segmentId}"
            };
            segment.Transform.Position = new Vector3(0, 1, segmentId);

            TrackSegmentComponent segmentComponent = new TrackSegmentComponent
            {
                SegmentId = segmentId,
                SegmentType = segmentType,
                Curvature = curvature,
                PreviousSegment = previousSegmentComponent,
                NextSegment = null,
                CumulativeCurvature = Vector3.Zero,
            };
            segment.AddComponent(segmentComponent);

            TrackSegmentDrawableComponent drawableComponent = new TrackSegmentDrawableComponent
            {
                TextureFilePath = GetTrackSegmentTypeTextureFilePath(segmentType)
            };
            segment.AddComponent(drawableComponent);

            if (previousSegmentComponent != null)
            {
                segmentComponent.CumulativeCurvature = previousSegmentComponent.CumulativeCurvature + curvature;
                previousSegmentComponent.NextSegment = segmentComponent;
            }

            BoxColliderComponent boxColliderComponent = new BoxColliderComponent
            {
                // Collider bounding box is shifted 1 segment early to match up with rendering
                BoundingBox = new Rect3(-1, -0.1, -1, 2, 0.2, 1),
                Collider = new BoxCollider()
            };
            segment.AddComponent(boxColliderComponent);

            TrackCollection.TrackSegments[segmentId] = segment;
            TrackCollection.LastActiveObjectIndex++;

            if (segmentId != TrackCollection.LastActiveObjectIndex)
            {
                throw new Exception($"Segment ID mismatch: {segmentId} != {TrackCollection.LastActiveObjectIndex}");
            }

            // Add to scene object collection
            SceneObjects.Add(segment);

            return segmentComponent;
        }

        private string GetTrackSegmentTypeTextureFilePath(TrackSegmentType segmentType)
        {
            switch (segmentType)
            {
                case TrackSegmentType.Track:
                    return "Assets\\track.png";

                case TrackSegmentType.TrackBreaking:
                    return "Assets\\track-breaking.png";

                case TrackSegmentType.TrackFixing:
                    return "Assets\\track-fixing.png";

                case TrackSegmentType.HoleEntering:
                    return "Assets\\hole-entering.png";

                case TrackSegmentType.Hole:
                    return "Assets\\hole.png";

                default:
                    throw new Exception($"Unhandled segment type: {segmentType}");
            }
        }
    }
}
