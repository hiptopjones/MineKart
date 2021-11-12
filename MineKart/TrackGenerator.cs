using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    class TrackGenerator
    {
        private static List<TrackSegmentComponent> Segments { get; set; } = new List<TrackSegmentComponent>();

        public static List<TrackSegmentComponent> GenerateTrack()
        {
            AddSection(TrackSegmentType.Normal, 10, 0, 0);
            AddSection(TrackSegmentType.Start, 1, 0, 0);
            AddSection(TrackSegmentType.Normal, 10, 0.2, -0.1);
            AddSection(TrackSegmentType.Checkpoint, 1, 0, 0);
            AddSection(TrackSegmentType.Normal, 10, -0.2, 0.1);
            AddSection(TrackSegmentType.Finish, 1, 0, 0);
            AddSection(TrackSegmentType.Normal, 10, 0, 0);

            PostProcessTrack();
            return Segments;
        }

        private static void AddSection(TrackSegmentType segmentType, int numSegments, double curvePerSegment, double pitchPerSegment)
        {
            TrackSegmentComponent previousSegment = Segments.LastOrDefault();

            int sectionId = 0;
            if (previousSegment != null)
            {
                sectionId = previousSegment.SectionId + 1;
            }

            int halfNumSegments = numSegments / 2;
            for (int i = 0; i < numSegments; i++)
            {
                // Enables balancing of changes from pitch (curve is not balanced)
                int pitchSign = (i < halfNumSegments ? 1 : -1);
                Vector3 curvature = new Vector3(curvePerSegment, pitchPerSegment * pitchSign);

                TrackSegmentComponent currentSegment = new TrackSegmentComponent
                {
                    SegmentType = segmentType,
                    SegmentId = i,
                    SectionId = sectionId,
                    Curvature = curvature,
                    CumulativeSegmentId = 0,
                    CumulativeCurvature = curvature
                };

                if (previousSegment != null)
                {
                    currentSegment.CumulativeSegmentId = previousSegment.CumulativeSegmentId + 1;
                    currentSegment.CumulativeCurvature = previousSegment.CumulativeCurvature + curvature;
                }

                Segments.Add(currentSegment);

                previousSegment = currentSegment;
            }
        }

        private static void PostProcessTrack()
        {
            // Link the track segments in a doubly-linked loop

            TrackSegmentComponent previousSegment = Segments.Last();

            foreach (TrackSegmentComponent currentSegment in Segments)
            {
                currentSegment.PreviousSegment = previousSegment;
                previousSegment.NextSegment = currentSegment;

                previousSegment = currentSegment;
            }

            previousSegment.NextSegment = Segments.First();
        }
    }
}
