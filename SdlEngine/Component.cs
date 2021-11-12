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
    }
}
