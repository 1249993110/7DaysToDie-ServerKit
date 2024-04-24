using Autofac;
using IceCoffee.SimpleCRUD;
using SdtdServerKit.Managers;
using System.Reflection;

namespace SdtdServerKit.Extensions
{
    internal static class ContainerBuilderExtension
    {
        public static ContainerBuilder AddRepositories(this ContainerBuilder builder, Assembly assembly)
        {
            builder.RegisterInstance(DbConnectionFactory.Default).As<IDbConnectionFactory>();
            builder.RegisterType<SdtdServerKit.Data.RepositoryFactory>().As<IRepositoryFactory>().SingleInstance();
            builder.RegisterType<UnitOfWorkFactory>().As<IUnitOfWorkFactory>().SingleInstance();

            foreach (var implType in assembly.GetExportedTypes())
            {
                if (implType.IsSubclassOf(typeof(RepositoryBase)) && implType.IsAbstract == false && implType.IsGenericType == false)
                {
                    var serviceType = implType.GetInterfaces().First(p => typeof(IRepository).IsAssignableFrom(p) && p != typeof(IRepository) && p.IsGenericType == false);
                    builder.RegisterType(implType).As(serviceType);
                }
            }

            return builder;
        }
    }
}
