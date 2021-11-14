using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    public class PlayerCollisionHandlerComponent : CollisionHandlerComponent
    {
        public override void OnCollisionEnter(GameObject other)
        {
            TrackSegmentComponent segmentComponent = other.GetComponent<TrackSegmentComponent>();
            if (segmentComponent == null)
            {
                return;
            }

            RailsMovementComponent movementComponent = Owner.GetComponent<RailsMovementComponent>();

            if (segmentComponent.SegmentType == TrackSegmentType.Hole || segmentComponent.SegmentType == TrackSegmentType.HoleExiting)
            {
                movementComponent.StartFalling();

                // After some delay, go to end screen
                EventManager eventManager = ServiceLocator.Instance.GetService<EventManager>();
                eventManager.RequestCallback(GameSettings.EndScreenDelay,
                    () => {
                        SceneStateMachine sceneStateMachine = ServiceLocator.Instance.GetService<SceneStateMachine>();
                        sceneStateMachine.SwitchTo((int)SceneType.EndScreen);
                    }); 
            }
            else
            {
                movementComponent.StopJumping();
            }
        }
    }
}
