using NDOInterfaces;
using System;

namespace NDO.UISupport
{
    public interface IDbUISupport
    {
		/// <summary>
		/// Initializes the instance and connects it with the NDO provider.
		/// </summary>
		/// <param name="provider"></param>
		void Initialize( IProvider provider );

		/// <summary>
		/// Builds a ADO.NET connection string using a dialog in which the user 
		/// can enter the parameters of the connection string.
		/// </summary>
		/// <param name="connectionString">Existing ADO.NET connection string or null, 
		/// if a new connection string is to be built.</param>
		/// <returns>The DialogResult, determining, if the Cancel or OK buttons have been pressed.</returns>
		NdoDialogResult ShowConnectionDialog( ref string connectionString );

		/// <summary>
		/// This dialog is used to enter all necessary data needed to create a database.
		/// </summary>
		/// <param name="necessaryData">A data structure, which is able to hold all necessary data to perform the creation.</param>
		/// <returns>The DialogResult, determining, if the Cancel or OK buttons have been pressed.</returns>
		/// <remarks>
		/// The parameter object will be passed to CreateDatabase by NDO. 
		/// The default implementation of CreateDatabase in the class NDOAbstractProvider expects a <see cref="NDOCreateDbParameter">NDOCreateDbParameter object</see>.
		/// <seealso cref="DbUISupportBase"/>
		/// <seealso cref="NDOCreateDbParameter"/>
		/// </remarks>
		NdoDialogResult ShowCreateDbDialog( ref object necessaryData );

		/// <summary>
		/// Gets the name of the UIProvider.
		/// </summary>
		/// <remarks>
		/// The name must be the same name as the corresponding IProvider implementation.
		/// </remarks>
		string Name { get; }

		/// <summary>
		/// Creates a database. 
		/// </summary>
		/// <param name="necessaryData">All data, necessary to perform the creation.</param>
		/// <returns>The connection string, which can be used to access the created database. 
		/// The default implementation always returns an empty string.</returns>
		/// <remarks>
		/// The data structure of the parameter object should match the data structure returned by the ShowCreateDialog function.
		/// The default implementation uses the CREATE DATABASE statement end expects a NDOCreateDbParameter object as parameter.
		/// <seealso cref="DbUISupportBase"/>
		/// <seealso cref="NDOCreateDbParameter"/>
		/// </remarks>
		string CreateDatabase( object necessaryData );
	}
}
