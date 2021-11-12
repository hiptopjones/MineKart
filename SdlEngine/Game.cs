using NLog;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public abstract class Game
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        protected bool IsExitRequested { get; set; }

        public void StartLoop()
        {
            while (!IsExitRequested)
            {
                Time.NextFrame();

                HandleEvents();
                Update();
                LateUpdate();
                Render();
            }
        }

        protected abstract void HandleEvents();
        protected abstract void Update();
        protected abstract void LateUpdate();
        protected abstract void Render();
    }
}
