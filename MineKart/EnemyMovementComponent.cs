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

        public override void Update()
        {
            TransformComponent transform = Owner.Transform;
            transform.Position += new Vector3(0, 0, ForwardSpeed) * Time.DeltaTime;
        }
    }
}
