using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Code.Core
{
    public class ServiceLocator
    {
        private static ServiceLocator _instance;
        public static ServiceLocator Instance => _instance ?? (_instance = new ServiceLocator());

        private readonly Dictionary<Type, object> _services;

        public ServiceLocator()
        {
            _services = new Dictionary<Type, object>();
        }

        public void RegisterService<T>(T service)
        {
            var type = typeof(T);
            Assert.IsFalse(_services.ContainsKey(type), $"Service {type} already registered");

            _services.Add(type, service);        
        }
        
        public void UnregisterService<T>()
        {
            var type = typeof(T);
            Assert.IsTrue(_services.ContainsKey(type), $"Service {type} is not registered");

            _services.Remove(type);        
        }

        public T GetService<T>()
        {
            var type = typeof(T);
            if (!_services.TryGetValue(type, out var service))
            {
                throw new Exception($"Service {type} not registered");
            }
        
            return (T) service;
        }

        public bool Contains<T>()
        {
            var type = typeof(T);
            return _services.ContainsKey(type);
        }
    }
}