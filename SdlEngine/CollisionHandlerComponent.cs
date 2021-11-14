using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class CollisionHandlerComponent : Component
    {
        public virtual void OnCollisionEnter(GameObject other)
        {

        }

        public virtual void OnCollisionExit(GameObject other)
        {

        }

        public virtual void OnCollisionStay(GameObject other)
        {

        }
    }
}
