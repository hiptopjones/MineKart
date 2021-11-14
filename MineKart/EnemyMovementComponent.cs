using SDL2;
using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    public class EnemyMovementComponent : Component
    {
        public double ForwardSpeed { get; set; }
        public double PruneDistance { get; set; } // Distance enemy must be past the player before it can be pruned

        private TrackCollection TrackCollection { get; set; }
        private GameObject Player { get; set; }

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
        }

        public override void Update()
        {
            TransformComponent transform = Owner.Transform;
            transform.Position += new Vector3(0, 0, ForwardSpeed) * Time.DeltaTime;

            // TODO: Should this be in a separate (range destroy) component?
            if (transform.Position.Z < Player.Transform.Position.Z - PruneDistance)
            {
                Owner.Destroy();
            }
        }

        // TODO: Should this be in a separate (track alignment) component?
        public override void LateUpdate()
        {
            // Everything moves down the Z by default, but the track is rendered skewed one way or the other
            // This code follows the track skew to make the enemy rendeirng line up with the track

            TransformComponent transform = Owner.Transform;
            int segmentId = (int)transform.Position.Z;

            SpriteComponent spriteComponent = Owner.GetComponent<SpriteComponent>();

            GameObject currentSegment;
            if (TrackCollection.TrackSegments.TryGetValue(segmentId, out currentSegment))
            {
                TrackSegmentComponent currentSegmentComponent = currentSegment.GetComponent<TrackSegmentComponent>();
                TrackSegmentComponent previousSegmentComponent = currentSegmentComponent.PreviousSegment; // Previous means closer to the viewer

                TrackSegmentDrawableComponent currentDrawableComponent = currentSegment.GetComponent<TrackSegmentDrawableComponent>();
                TrackSegmentDrawableComponent previousDrawableComponent = previousSegmentComponent.Owner.GetComponent<TrackSegmentDrawableComponent>();

                double fractionalZ = transform.Position.Z - (int)transform.Position.Z;
                spriteComponent.DrawPosition = new Vector3
                {
                    X = Utilities.Lerp(previousDrawableComponent.DrawPosition.X, currentDrawableComponent.DrawPosition.X, fractionalZ),
                    Y = Utilities.Lerp(previousDrawableComponent.DrawPosition.Y, currentDrawableComponent.DrawPosition.Y, fractionalZ),
                    Z = transform.Position.Z
                };

            }
        }
    }
}
