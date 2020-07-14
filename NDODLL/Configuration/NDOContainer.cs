using NDO.Query;
using NDO.SqlPersistenceHandling;
using Unity;
using Unity.Lifetime;

namespace NDO.Configuration
{
	/// <summary>
	/// Wrapper class for an application-wide IoC container
	/// </summary>
    public class NDOContainer
    {
        static UnityContainer instance;

		/// <summary>
		/// Gets the root instance of the unity container.
		/// </summary>
        public static IUnityContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UnityContainer();
					instance.RegisterType<IPersistenceHandler, SqlPersistenceHandler>();
					instance.RegisterType<RelationContextGenerator>();
					instance.RegisterType<IQueryGenerator, SqlQueryGenerator>();
					instance.RegisterType<IPersistenceHandlerManager, NDOPersistenceHandlerManager>();
					// ContainerControlled means in this case, that there is only one instance per application, 
					// but the registration can be overriden in child containers.
					instance.RegisterType<IPersistenceHandlerPool, NDOPersistenceHandlerPool>(new ContainerControlledLifetimeManager());
                }

                return instance;
            }
        }
    }
}
