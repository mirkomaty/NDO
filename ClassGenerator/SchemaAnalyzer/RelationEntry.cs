using System;

namespace ClassGenerator.SchemaAnalyzer
{
	// See Feature Request 1904596
	class RelationEntry
	{
		string parentTable;
		public string ParentTable
		{
			get { return parentTable; }
			set { parentTable = value; }
		}
		string childTable;
		public string ChildTable
		{
			get { return childTable; }
			set { childTable = value; }
		}
		string xPath;
		public string XPath
		{
			get { return xPath; }
			set { xPath = value; }
		}
	}
}
