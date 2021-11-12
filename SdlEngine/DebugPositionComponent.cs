using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class DebugActionComponent : Component
    {
        public Action<GameObject> DebugAction { get; set; }

        public override void Start()
        {
            if (DebugAction == null)
            {
                throw new Exception("Missing debug action");
            }
        }

        public override void Update()
        {
            DebugAction(Owner);
        }
    }
}
