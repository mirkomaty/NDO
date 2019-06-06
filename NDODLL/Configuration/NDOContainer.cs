using NDO.Query;
using NDO.SqlPersistenceHandling;
using Unity;

namespace NDO.Configuration
{
    public class NDOContainer
    {
        static UnityContainer instance;

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
                }

                return instance;
            }
        }
    }
}
