using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    public enum TrackSegmentType
    {
        Track,
        TrackBreaking,
        HoleEntering,
        Hole,
        HoleExiting,
        TrackFixing,
    }

    public class TrackSegmentComponent : Component
    {
        public TrackSegmentType SegmentType { get; set; }
        public int SegmentId { get; set; } // Index within section
        public Vector3 Curvature { get; set; } // The ddx, ddy per segment
        public Vector3 CumulativeCurvature { get; set; }
        public TrackSegmentComponent NextSegment { get; set; }
        public TrackSegmentComponent PreviousSegment { get; set; }

        public override string ToString()
        {
            return $"[ {SegmentType} Id: {SegmentId} Curve: {Curvature.X} Pitch: {Curvature.Y} Cumulative Curve: {CumulativeCurvature} ]";
        }
    }
}
