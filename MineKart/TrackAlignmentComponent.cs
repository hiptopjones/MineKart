using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    class TrackAlignmentComponent : Component
    {
        private TrackCollection TrackCollection { get; set; }

        public override void Awake()
        {
            TrackCollection = ServiceLocator.Instance.GetService<TrackCollection>();
            if (TrackCollection == null)
            {
                throw new Exception($"Unable to retrieve track collection from service locator");
            }
        }

        public override void LateUpdate()
        {
            // Everything moves down the Z by default, but the track is rendered skewed one way or the other
            // This code follows the track skew to make the enemy rendeirng line up with the track

            TransformComponent transform = Owner.Transform;
            int segmentId = (int)transform.Position.Z;

            List<SpriteComponent> spriteComponents = Owner.GetComponents<SpriteComponent>();

            GameObject currentSegment;
            if (TrackCollection.TrackSegments.TryGetValue(segmentId, out currentSegment))
            {
                TrackSegmentComponent currentSegmentComponent = currentSegment.GetComponent<TrackSegmentComponent>();
                TrackSegmentComponent previousSegmentComponent = currentSegmentComponent.PreviousSegment; // Previous means closer to the viewer
                if (previousSegmentComponent == null)
                {
                    return;
                }

                TrackSegmentDrawableComponent currentDrawableComponent = currentSegment.GetComponent<TrackSegmentDrawableComponent>();
                TrackSegmentDrawableComponent previousDrawableComponent = previousSegmentComponent.Owner.GetComponent<TrackSegmentDrawableComponent>();

                double fractionalZ = transform.Position.Z - (int)transform.Position.Z;
                foreach (SpriteComponent spriteComponent in spriteComponents)
                {
                    spriteComponent.DrawPosition = new Vector3
                    {
                        X = transform.Position.X + Utilities.Lerp(previousDrawableComponent.DrawPosition.X, currentDrawableComponent.DrawPosition.X, fractionalZ),
                        Y = /*transform.Position.Y +*/ Utilities.Lerp(previousDrawableComponent.DrawPosition.Y, currentDrawableComponent.DrawPosition.Y, fractionalZ),
                        Z = transform.Position.Z
                    };
                }
            }
        }
    }
}
