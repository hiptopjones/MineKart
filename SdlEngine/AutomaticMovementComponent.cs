using SDL2;
using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class AutomaticMovementComponent : Component
    {
        public Vector3 Velocity { get; set; }

        public override void Update()
        {
            TransformComponent transform = Owner.Transform;
            transform.Position += Velocity * Time.DeltaTime;
        }
    }
}
