using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NDOInterfaces;

namespace NDO.SqlPersistenceHandling
{
	public class SqlExecuteBehavior
	{
		private readonly IProvider provider;
		private readonly ILogger logger;

		public SqlExecuteBehavior(IProvider provider, ILogger logger)
		{
			this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
			this.logger = logger;
		}

		public string BuildBatch(string[] statements)
		{
			if (statements == null || statements.Length == 0)
				return string.Empty;

			if (this.provider.SupportsBulkCommands)
				return this.provider.GenerateBulkCommand(statements);

			return string.Join(";\n", statements);
		}

		public void LogBatch(string sql)
		{
			if (string.IsNullOrEmpty(sql) || this.logger == null)
				return;

			if (this.logger.IsEnabled(LogLevel.Debug))
				this.logger.LogDebug("Batch: \r\n" + sql);
		}
	}
}
