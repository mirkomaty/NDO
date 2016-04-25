//
// Copyright (c) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using System;
using System.Windows.Forms;
using System.Data;
using NDOInterfaces;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für ColumnNode.
	/// </summary>
	[Serializable]
#if DEBUG
	public class ColumnNode : NDOTreeNode
#else
	internal class ColumnNode : NDOTreeNode
#endif
	{
		public event EventHandler IsPrimaryChanged;

		public Column Column
		{
			get { return this.myObject as Column; }
		}

		public ColumnNode(string name, bool isPrimary, bool isAutoIncremented, string type, string comment, TableNode parent) : base (name, parent)
		{
			bool useFieldNamePrefix = ApplicationController.Instance.AssemblyNode.Assembly.TargetLanguage == TargetLanguage.VB;
			Column column = new Column(name, isPrimary, isAutoIncremented, type, useFieldNamePrefix, comment);
			this.myObject = column;
			column.OnIsPrimaryChanged += new EventHandler(OnIsPrimaryChanged);
			column.OnIsMappedChanged += new EventHandler(OnIsPrimaryChanged);

			CalculateIndex();
		}

		TableNode MyTableNode
		{
			get { return this.Parent as TableNode; }
		}

		public bool IsMapped
		{
			get { return this.Column.IsMapped; }
			set { this.Column.IsMapped = value; }
		}

		protected override void CalculateIndex()
		{
			if (Column.IsPrimary && Column.IsMapped)
				SetImageIndex(7);
			else if (Column.IsPrimary)
				SetImageIndex(6);
			else if (Column.IsMapped)
				SetImageIndex(16);
			else
				SetImageIndex(17);
		}

		public bool IsPrimary
		{
			get { return this.Column.IsPrimary; }
		}
			
		private void OnIsPrimaryChanged(Object sender, EventArgs e)
		{
			CalculateIndex();
			if (IsPrimaryChanged != null)
				IsPrimaryChanged(this, e);
			ApplicationController.Instance.MappingChanged();
		}

		public void ChangePrimary(Object sender, EventArgs e)
		{
			this.Column.IsPrimary = !this.Column.IsPrimary;
			if (this.Column.IsPrimary)
			{
				Type t = Type.GetType(this.Column.Type);
				bool mapToGuid = ApplicationController.Instance.AssemblyNode.Assembly.MapStringsAsGuids;
				bool useClassField = ApplicationController.Instance.AssemblyNode.Assembly.UseClassField;
				if (t == typeof(string) && mapToGuid)
					t = typeof(Guid);
				if (useClassField && t == typeof(string))
				{
					if (!this.IsMapped)
						ChangeMapping(null, EventArgs.Empty);
				}
				else if (t != typeof(int))
				{
					if (t == typeof(Decimal))
						t = typeof(int);
					if (t != typeof(int) && t != typeof(string) && t != typeof(Guid))
						MessageBox.Show("Can't treat type '" + t.Name + "' as object id.");
					((TableNode)this.Parent).Table.NdoOidType = t.ToString();
				}
			}
		}

		public void ChangeMapping(Object sender, EventArgs e)
		{
			this.Column.IsMapped = !this.Column.IsMapped;
		}

		public void MakeForeignKey(Object sender, EventArgs e)
		{
			ApplicationController.Instance.MakeForeignKey(this);		
		}

		public new string Name
		{
			get { return Column.Name; }
		}


		public override ContextMenu GetContextMenu()
		{			
			ContextMenu menu = new ContextMenu();

			TableMappingType tmt = ((TableNode)this.Parent).Table.MappingType;
			if (tmt != TableMappingType.MappedAsClass && tmt != TableMappingType.MappedAsIntermediateClass)
				return menu;

			if (Column.IsPrimary)
				menu.MenuItems.Add(new MenuItem("Don't map as primary key", new EventHandler(this.ChangePrimary)));
			if (Column.IsMapped)				
				menu.MenuItems.Add(new MenuItem("Don't map as field", new EventHandler(this.ChangeMapping)));

			if (!Column.IsMapped)
				menu.MenuItems.Add(new MenuItem("Map column as field", new EventHandler(this.ChangeMapping)));
			if (!MyTableNode.HasPrimary)
				menu.MenuItems.Add(new MenuItem("Map as primary key", new EventHandler(this.ChangePrimary)));
			if (!Column.IsMapped && !Column.IsPrimary)
				menu.MenuItems.Add(new MenuItem("Map as foreign key...", new EventHandler(this.MakeForeignKey)));					
			
			return menu;
		}

		public string ShortType
		{
			get
			{
                // Conversion in short type names is no longer necessary, since we use CodeDom now.
                return this.Column.Type;
            }
		}

		/// <summary>
		/// Used by serialization only
		/// </summary>
		public ColumnNode()
		{
		}

		public override void FromXml(System.Xml.XmlElement element)
		{
			base.FromXml (element);
			Column.OnIsPrimaryChanged += new EventHandler(OnIsPrimaryChanged);
			Column.OnIsMappedChanged += new EventHandler(OnIsPrimaryChanged);
		}


#if SoapSerialisation
		protected ColumnNode(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
			Column.OnIsPrimaryChanged += new EventHandler(OnIsPrimaryChanged);
			Column.OnIsMappedChanged += new EventHandler(OnIsPrimaryChanged);
		}
#endif
	}
}
