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
					instance.RegisterType<IPersistenceHandler, SqlPersistenceHandler>();
					instance.RegisterType<RelationContextGenerator>();
					instance.RegisterType<IQueryGenerator, SqlQueryGenerator>();
                }

                return instance;
            }
        }
    }
}
