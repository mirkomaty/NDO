using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDOEnhancer
{
	public enum FxType
	{
		NetFx,
		Standard2
	}

	class Corlib
	{
		public static FxType FxType { get; set; }

		public static string Name
		{
			get
			{
				return FxType == FxType.NetFx ? "[mscorlib]" : "[netstandard]";
			}
		}
	}
}
