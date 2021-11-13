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
        public GameObject Player { get; set; }

        private GameObjectCollection SceneObjects { get; set; }

        private Dictionary<int, GameObject> TrackSegments { get; set; } = new Dictionary<int, GameObject>();

        private int FirstActiveObjectIndex { get; set; } = -1;
        private int LastActiveObjectIndex { get; set; } = -1;

        private Random Random { get; set; } = new Random();

        public override void Awake()
        {
            SceneObjects = ServiceLocator.Instance.GetService<GameObjectCollection>();
            if (SceneObjects == null)
            {
                throw new Exception($"Unable to retrieve game object collection from service locator");
            }
        }

        public override void Start()
        {
            GrowTrack();
        }

        public override void Update()
        {
            PruneTrack();

            Debug.DrawText($"Active: {TrackSegments.Count}");
            Debug.DrawText($"First: {FirstActiveObjectIndex}");
            Debug.DrawText($"Last:  {LastActiveObjectIndex}");

            GrowTrack();
        }

        private void PruneTrack()
        {
            int playerZ = (int)Player.Transform.Position.Z;
            for (int segmentId = FirstActiveObjectIndex; segmentId < playerZ - PruneDistance; segmentId++)
            {
                GameObject gameObject = TrackSegments[segmentId];

                // Causes the object to be removed from the scene object collection
                gameObject.IsAlive = false;

                if (segmentId != FirstActiveObjectIndex)
                {
                    throw new Exception($"Segment ID mismatch: {segmentId} != {FirstActiveObjectIndex}");
                }

                TrackSegments.Remove(segmentId);
                FirstActiveObjectIndex++;
            }
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

                case TrackSegmentType.HoleExiting:
                    return "Assets\\hole-exiting.png";

                default:
                    throw new Exception($"Unhandled segment type: {segmentType}");
            }
        }

        public void GrowTrack()
        {
            if (FirstActiveObjectIndex < 0)
            {
                GenerateStraight(null, 10);
                FirstActiveObjectIndex = 0;
            }

            TrackSegmentComponent previousSegmentComponent = TrackSegments[LastActiveObjectIndex].GetComponent<TrackSegmentComponent>();

            while (LastActiveObjectIndex < Player.Transform.Position.Z + GenerateDistance)
            {
                // TODO: Add percent settings to the GameSettings
                // TODO: Use random for number of segments and hill/curve values

                int percent = Random.Next(100);
                if (percent < 25)
                {
                    previousSegmentComponent = GenerateHole(previousSegmentComponent);
                }
                else if (percent < 50)
                {
                    int numSegments = Random.Next(5, 15);
                    double incline = Utilities.Lerp(0.02, 0.15, Random.NextDouble());
                    int sign = Random.Next(2) == 0 ? -1 : 1;
                    previousSegmentComponent = GenerateHill(previousSegmentComponent, numSegments, incline * sign);
                }
                else if (percent < 90)
                {
                    int numSegments = Random.Next(5, 15);
                    double curvature = Utilities.Lerp(0.05, 0.3, Random.NextDouble());
                    int sign = Random.Next(2) == 0 ? -1 : 1;
                    previousSegmentComponent = GenerateCurve(previousSegmentComponent, numSegments, curvature * sign);
                }
                else
                {
                    int numSegments = Random.Next(5, 15);
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
            previousSegmentComponent = AddTrackSegment(previousSegmentComponent, TrackSegmentType.HoleExiting);
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

            GameObject gameObject = new GameObject();
            gameObject.Transform.Position = new Vector3(0, 1, segmentId);

            TrackSegmentComponent segmentComponent = new TrackSegmentComponent
            {
                SegmentId = segmentId,
                SegmentType = segmentType,
                Curvature = curvature,
                PreviousSegment = previousSegmentComponent,
                NextSegment = null,
                CumulativeCurvature = Vector3.Zero,
            };
            gameObject.AddComponent(segmentComponent);

            TrackSegmentDrawableComponent drawableComponent = new TrackSegmentDrawableComponent
            {
                TextureFilePath = GetTrackSegmentTypeTextureFilePath(segmentType)
            };
            gameObject.AddComponent(drawableComponent);

            if (previousSegmentComponent != null)
            {
                segmentComponent.CumulativeCurvature = previousSegmentComponent.CumulativeCurvature + curvature;
                previousSegmentComponent.NextSegment = segmentComponent;
            }

            TrackSegments[segmentId] = gameObject;
            LastActiveObjectIndex++;

            if (segmentId != LastActiveObjectIndex)
            {
                throw new Exception($"Segment ID mismatch: {segmentId} != {LastActiveObjectIndex}");
            }

            // Add to scene object collection
            SceneObjects.Add(gameObject);

            return segmentComponent;
        }
    }
}
