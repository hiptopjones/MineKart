using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    class DrawableSystem
    {
        public void Render(List<GameObject> gameObjects)
        {
            // TODO: Use a more efficient structure / method (like sorted set?)
            foreach (GameObject gameObject in gameObjects.OrderByDescending(x => x.Transform.Position.Z))
            {
                if (gameObject.IsAlive)
                {
                    gameObject.Render();
                }
            }
        }
    }
}
