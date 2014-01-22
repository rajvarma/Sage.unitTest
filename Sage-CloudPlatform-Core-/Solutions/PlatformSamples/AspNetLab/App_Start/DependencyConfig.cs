using AspNetLab.Controllers;
using AspNetLab.Unity;
using Microsoft.Practices.Unity;
using Sage.Core.Cache;
using Sage.Core.Framework.Configuration;
using Sage.Core.Framework.Storage;
using System.Web.Mvc;

namespace AspNetLab.App_Start
{
    public class DependencyConfig
    {
        public static void RegisterDependencies()
        {
            IUnityContainer container = GetUnityContainer();
            RegisterTypes(container);

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer GetUnityContainer()
        {
            IUnityContainer container = new UnityContainer();
            return container;
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<AzureCacheController>();
            container.RegisterType<AzureQueueController>();
            container.RegisterType<AzureBlobController>();

            container.RegisterType<ICache, AzureCache>(new InjectionConstructor("default"));
            container.RegisterType<IConfigurationManager, BaseConfigurationManager>();
            container.RegisterType<IQueue, AzureQueue>();
            container.RegisterType<IBlobStore, BlobStore>();
            container.RegisterType<IVersionBlobStore, VersionBlobStore>();
            container.RegisterType<IChunkBlobStore, ChunkBlobStore>();
        }
    }
}