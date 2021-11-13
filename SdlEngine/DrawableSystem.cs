using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    class DrawableSystem
    {
        public void Render(List<GameObject> drawableObjects)
        {
            // TODO: Use a more efficient structure / method (like sorted set?)
            foreach (GameObject drawableObject in drawableObjects.OrderByDescending(x => x.Transform.Position.Z))
            {
                if (drawableObject.IsAlive)
                {
                    drawableObject.Render();
                }
            }
        }
    }
}
