using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    class AcceleratedMovementComponent : Component
    {
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }

        public override void Update()
        {
            TransformComponent transform = Owner.Transform;
            transform.Position += Velocity * Time.DeltaTime;

            Velocity += Acceleration * Time.DeltaTime;
        }
    }
}
