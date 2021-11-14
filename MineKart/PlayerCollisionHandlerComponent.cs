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
        private EventManager EventManager { get; set; }
        private SceneManager SceneManager { get; set; }

        public override void Awake()
        {
            EventManager = ServiceLocator.Instance.GetService<EventManager>();
            if (EventManager == null)
            {
                throw new Exception($"Unable to retrieve event manager from service locator");
            }

            SceneManager = ServiceLocator.Instance.GetService<SceneManager>();
            if (SceneManager == null)
            {
                throw new Exception($"Unable to retrieve scene manager from service locator");
            }
        }

        public override void OnCollisionEnter(GameObject other)
        {
            bool isDead = false;
            RailsMovementComponent movementComponent = Owner.GetComponent<RailsMovementComponent>();

            TrackSegmentComponent segmentComponent = other.GetComponent<TrackSegmentComponent>();
            if (segmentComponent != null)
            {
                if (segmentComponent.SegmentType == TrackSegmentType.Hole)
                {
                    // Fell in a hole
                    isDead = true;
                }
                else
                {
                    movementComponent.StopJumping();
                }
            }
            else
            {
                EnemyMovementComponent enemyComponent = other.GetComponent<EnemyMovementComponent>();
                if (enemyComponent != null)
                {
                    // Collided with the enemy
                    isDead = true;
                }
            }

            if (isDead)
            {
                movementComponent.StartFalling();

                ExplosionSpawnerComponent explosionComponent = Owner.GetComponent<ExplosionSpawnerComponent>();
                explosionComponent.StartSpawning();

                // After some delay, go to end screen
                EventManager.RequestCallback(GameSettings.EndScreenDelay, () => { SceneManager.SwitchTo((int)SceneType.EndScreen); });
            }
        }
    }
}
