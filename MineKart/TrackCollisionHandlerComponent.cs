using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    public class TrackCollisionHandlerComponent : CollisionHandlerComponent
    {
        public override void OnCollisionEnter(GameObject other)
        {
            TrackSegmentComponent segmentComponent = other.GetComponent<TrackSegmentComponent>();
            if (segmentComponent == null)
            {
                return;
            }

            RailsMovementComponent movementComponent = Owner.GetComponent<RailsMovementComponent>();

            if (segmentComponent.SegmentType == TrackSegmentType.Hole)
            {
                movementComponent.StartFalling();
            }
            else
            {
                movementComponent.StopJumping();
            }
        }
    }
}
