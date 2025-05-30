using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdoDllUnitTests
{
	class MockType
	{
		/// <summary>
		/// Creates a Type mock which can be used in updateRanks and other
		/// Lists and Dictionaries
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns>The type mock</returns>
		public static Mock<Type> CreateType( string typeName )
		{
			Mock<Type> type = new Mock<Type>();
			type.Setup( t => t.FullName ).Returns( typeName );
			type.Setup( t => t.GetHashCode() ).Returns( typeName.GetHashCode() );
			type.Setup( t => t.Equals( It.IsAny<Type>() ) ).Returns( ( Type t ) => t.FullName == typeName );
			type.Setup( t => t.Equals( It.IsAny<object>() ) ).Returns( ( Type t ) => t.FullName == typeName );
			type.Setup( t => t.Name ).Returns( typeName.Substring( typeName.LastIndexOf( '.' ) + 1 ) );
			type.Setup( t => t.ToString() ).Returns( typeName );
			type.Setup( t => t.UnderlyingSystemType ).Returns( type.Object );
			return type;
		}
	}
}
