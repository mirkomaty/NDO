using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NDOEnhancer.BuildTask
{
	public class SetProperty : Task
	{
		private string inputValue;
		private string outputValue;

		public string InputValue
		{
			get
			{
				return this.inputValue;
			}
			set
			{
				this.inputValue = value;
			}
		}

		[Output]
		public string OutputValue
		{
			get
			{
				return this.outputValue;
			}
		}

		public override bool Execute()
		{
			this.outputValue = this.inputValue;
			return true;
		}
	}
}
