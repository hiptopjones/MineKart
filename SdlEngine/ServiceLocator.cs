using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public class ServiceLocator
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private Dictionary<string, object> ServicesMap { get; set; } = new Dictionary<string, object>();

        private static ServiceLocator _instance;
        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                }

                return _instance;
            }
        }
        
        private ServiceLocator()
        {
            // Singleton
        }

        public void ProvideService<T>(object service) where T : class
        {
            string typeName = typeof(T).Name;
            ProvideService(typeName, service);
        }

        public void ProvideService(string key, object service)
        {
            if (ServicesMap.ContainsKey(key))
            {
                Logger.Info($"Replacing existing service for key: {key}");
            }
            else
            {
                Logger.Info($"Adding service for key: {key}");
            }

            ServicesMap[key] = service;
        }

        public T GetService<T>() where T : class
        {
            string typeName = typeof(T).Name;
            return GetService<T>(typeName);
        }

        public T GetService<T>(string key) where T : class
        {
            object service;
            if (false == ServicesMap.TryGetValue(key, out service))
            {
                throw new Exception($"Unable to locate service for {key}");
            }

            return (T)service;
        }

    }
}
