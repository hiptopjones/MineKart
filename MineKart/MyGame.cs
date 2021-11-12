using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using SdlEngine;

namespace MineKart
{
    class MyGame : Game, IDisposable
    {
        private GraphicsManager GraphicsManager { get; set; }
        private EventManager EventManager { get; set; }
        private ResourceManager ResourceManager { get; set; }
        private FontManager FontManager { get; set; }
        private SceneStateMachine SceneStateMachine { get; set; }

        private Camera MainCamera { get; set; }

        public MyGame()
        {
            Initialize();
        }

        ~MyGame()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool isDisposing)' method
            Dispose(isDisposing: false);
        }

        #region IDisposable
        private bool isAlreadyDisposed;

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isAlreadyDisposed)
            {
                if (isDisposing)
                {
                    TeardownManaged();
                }

                TeardownUnmanaged();
                isAlreadyDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool isDisposing)' method
            Dispose(isDisposing: true);
            GC.SuppressFinalize(this);
        }

        private void TeardownManaged()
        {
            if (GraphicsManager != null)
            {
                GraphicsManager.Dispose();
                GraphicsManager = null;
            }

            if (EventManager != null)
            {
                EventManager.Dispose();
                EventManager = null;
            }

            if (FontManager != null)
            {
                FontManager.Dispose();
                FontManager = null;
            }
        }

        private void TeardownUnmanaged()
        {
            // N/A
        }
        #endregion

        private void Initialize()
        {
            GraphicsManager = new GraphicsManager(GameSettings.WindowTitle, GameSettings.RenderWidth, GameSettings.RenderHeight, GameSettings.WindowWidth, GameSettings.WindowHeight);
            ServiceLocator.Instance.ProvideService<GraphicsManager>(GraphicsManager);

            EventManager = new EventManager();
            ServiceLocator.Instance.ProvideService<EventManager>(EventManager);

            ResourceManager = new ResourceManager();
            ServiceLocator.Instance.ProvideService<ResourceManager>(ResourceManager);

            FontManager = new FontManager();
            FontManager.LoadFont("Debug", GameSettings.DebugFontFilePath, GameSettings.DebugFontSize);
            ServiceLocator.Instance.ProvideService<FontManager>(FontManager);

            MainCamera = new Camera(GameSettings.RenderWidth, GameSettings.RenderHeight, GameSettings.FieldOfViewDegrees, GameSettings.DrawDistance);
            ServiceLocator.Instance.ProvideService<Camera>(MainCamera);

            SceneStateMachine = new SceneStateMachine();
            ServiceLocator.Instance.ProvideService<SceneStateMachine>(SceneStateMachine);

            GameScene gameScene = new GameScene();
            int gameSceneId = SceneStateMachine.AddScene(gameScene);

            SplashScreenScene splashScreenScene = new SplashScreenScene(GameSettings.SplashScreenTextureFilePath, SceneStateMachine, gameSceneId, GameSettings.SplashScreenTransitionDelay);
            int splashScreenSceneId = SceneStateMachine.AddScene(splashScreenScene);

            SceneStateMachine.SwitchTo(gameSceneId);
        }

        protected override void HandleEvents()
        {
            EventManager.HandleEvents();

            if (EventManager.IsQuitRequested())
            {
                IsExitRequested = true;
            }
        }

        protected override void Update()
        {
            SceneStateMachine.Update();
        }

        protected override void LateUpdate()
        {
            SceneStateMachine.LateUpdate();
        }

        protected override void Render()
        {
            SDL.SDL_SetRenderDrawColor(GraphicsManager.RendererHandle, GameSettings.RenderClearColor.ToSdlColor());
            SDL.SDL_RenderClear(GraphicsManager.RendererHandle);

            SceneStateMachine.Render();

            SDL.SDL_RenderPresent(GraphicsManager.RendererHandle);
        }
    }
}
