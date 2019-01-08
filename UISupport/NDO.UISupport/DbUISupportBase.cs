using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NDO.UISupport
{
	public abstract class DbUISupportBase : IDbUISupport
	{
		private IProvider provider;

		/// <summary>
		/// Gets the NDO database provider instance
		/// </summary>
		public IProvider Provider => this.provider;

		/// <summary>
		/// Name of the provider
		/// </summary>
		/// <remarks>
		/// Note that this property must be overriden
		/// </remarks>
		public virtual string Name => throw new NotImplementedException();

		/// <summary>
		/// Initializes the DbUISupportBase instance
		/// </summary>
		/// <param name="provider"></param>
		public virtual void Initialize(IProvider provider)
		{
			this.provider = provider;
		}

		/// <summary>
		/// See <see cref="IDbUISupport">IProvider interface</see>.
		/// </summary>
		public virtual NdoDialogResult ShowConnectionDialog( ref string connectionString )
		{
			GenericConnectionDialog dlg = new GenericConnectionDialog( connectionString );
			NdoDialogResult result = (NdoDialogResult) dlg.ShowDialog();
			if (result != NdoDialogResult.Cancel)
				connectionString = dlg.ConnectionString;
			return result;
		}

		/// <summary>
		/// See <see cref="IDbUISupport">IProvider interface</see>.
		/// </summary>
		/// <remarks>
		/// The parameter is an object since overridden implementations may use other data structures.
		/// </remarks>
		public virtual NdoDialogResult ShowCreateDbDialog( ref object necessaryData )
		{
			DefaultCreateDbDialog dlg = new DefaultCreateDbDialog( this, (NDOCreateDbParameter) necessaryData );
			NdoDialogResult result = (NdoDialogResult) dlg.ShowDialog();
			if (result != NdoDialogResult.Cancel)
				necessaryData = dlg.NecessaryData;
			return result;
		}

		/// <summary>
		/// See <see cref="IDbUISupport">IProvider interface</see>.
		/// </summary>
		public virtual string CreateDatabase( object necessaryData )
		{
			NDOCreateDbParameter par = necessaryData as NDOCreateDbParameter;
			if (par == null)
				throw new ArgumentException( $"{nameof( DbUISupportBase )}: parameter type {necessaryData.GetType().FullName} is wrong.", nameof( necessaryData ) );
			try
			{
				return this.provider.CreateDatabase( par.DatabaseName, par.ConnectionString, null );
			}
			catch (Exception ex)
			{
				throw new NDOException( 19, "Error while attempting to create a database: Exception Type: " + ex.GetType().Name + " Message: " + ex.Message );
			}
		}
	}
}
