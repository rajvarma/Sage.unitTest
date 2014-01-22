using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace AspNetLab.Unity
{
    public class UnityDependencyResolver: IDependencyResolver
    {
        public UnityDependencyResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            Object result = null;

            if (_container.IsRegistered(serviceType))
            {
                result = _container.Resolve(serviceType);
            }

            return result;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            IEnumerable<Object> result = null;

            if (_container.IsRegistered(serviceType))
            {
                result = _container.ResolveAll(serviceType);
            }
            else
            {
                result = new List<Object>();
            }

            return result;
        }

        private readonly IUnityContainer _container;
    }
}