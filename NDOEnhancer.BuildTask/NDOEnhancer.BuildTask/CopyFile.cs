using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace NDOEnhancer.BuildTask
{
	public class CopyFile : Task
	{
		private string sourceFile;
		private string destFile;

		[Required]
		public string SourceFile
		{
			get
			{
				return this.sourceFile;
			}
			set
			{
				this.sourceFile = value;
			}
		}

		[Required]
		public string DestFile
		{
			get
			{
				return this.destFile;
			}
			set
			{
				this.destFile = value;
			}
		}

		public override bool Execute()
		{
			if ( Directory.Exists( this.destFile ) )
				this.destFile = Path.Combine( this.destFile, Path.GetFileName( this.sourceFile ) );
			File.Copy( this.sourceFile, this.destFile, true );
			return true;
		}
	}
}
