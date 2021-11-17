using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public abstract class Component
    {
        public GameObject Owner { get; set; }

        public bool IsEnabled { get; set; } = true;

        public virtual void Awake()
        {

        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void LateUpdate()
        {

        }

        //// Called on destroy if the object is being pooled
        //public virtual void Reset()
        //{

        //}
    }
}
