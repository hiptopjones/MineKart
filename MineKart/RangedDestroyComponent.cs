using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    // TODO: Could make this genreic, rather than specific to minekart
    class RangedDestroyComponent : Component
    {
        public double PruneDistance { get; set; }

        private GameObject Player { get; set; }

        public override void Awake()
        {
            Player = ServiceLocator.Instance.GetService<GameObject>("Player");
            if (Player == null)
            {
                throw new Exception($"Unable to retrieve player from service locator");
            }
        }

        public override void Update()
        {
            if (Owner.Transform.Position.Z < Player.Transform.Position.Z - PruneDistance)
            {
                Owner.Destroy();
            }
        }
    }
}
