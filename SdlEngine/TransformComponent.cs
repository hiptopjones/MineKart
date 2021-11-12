using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class TransformComponent : Component
    {
        public Vector3 Position { get; set; } = new Vector3();
    }
}
