using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public abstract class Collider
    {
        public abstract bool Intersects(Collider collider, out Vector3 overlap);
    }
}
