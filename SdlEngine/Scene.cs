using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public abstract class Scene
    {
        // Called when scene initially created. Called once.
        public virtual void OnCreate()
        {

        }

        // Called when scene destroyed. Called at most once (if a scene 
        // is not removed from the game, this will never be called).
        public virtual void OnDestroy()
        {
        }

        // Called whenever a scene is transitioned into. Can be 
        // called many times in a typical game cycle.
        public virtual void OnActivate() 
        {
        }

        // Called whenever a transition out of a scene occurs. 
        // Can be called many times in a typical game cycle.
        public virtual void OnDeactivate()
        {
        }

        // The below functions can be overridden as necessary in our scenes.
        public virtual void ProcessInput()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void LateUpdate()
        {
        }

        public virtual void Render()
        {
        }
    }
}
