using NDO.Query;
using NDO.SqlPersistenceHandling;
using Unity;

namespace NDO.Configuration
{
    public class NDOContainer
    {
        static UnityContainer instance;

        public static UnityContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UnityContainer();
					instance.RegisterType<IPersistenceHandler, NDOPersistenceHandler>();
					instance.RegisterType<SqlQueryGenerator>();
					instance.RegisterType<RelationContextGenerator>();
                }

                return instance;
            }
        }
    }
}
