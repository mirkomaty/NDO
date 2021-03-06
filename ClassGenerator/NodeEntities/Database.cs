﻿//
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
using System.Data;
using System.Xml;
using System.Windows.Forms;
using ClassGenerator.AssemblyWizard;
using ClassGenerator.SchemaAnalyzer;
using NDOInterfaces;
using NDO;
using System.Text;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für Database.
	/// </summary>
	[Serializable]
#if DEBUG
	public class Database : IXmlStorable
#else
	internal class Database : IXmlStorable
#endif
	{
		public Database(AssemblyWizModel model)
		{
			this.connectionString = model.ConnectionString;
			this.connectionType = model.ConnectionType;
			this.ownerName = model.OwnerName;
			this.xmlSchemaFile = model.XmlSchemaFile;
			this.isXmlSchema = model.IsXmlSchema;
			try
			{
				if ( this.isXmlSchema )
				{
                    XsdAnalyzer analyzer = new XsdAnalyzer();
					this.dataSet = analyzer.Analyze( this.xmlSchemaFile );
                    if (analyzer.ValidationMessages.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (string s in analyzer.ValidationMessages)
                        {
                            sb.Append(s);
                            sb.Append("\r\n");
                        }
                        new ShowWarningsDlg(sb.ToString()).ShowDialog();
                    }
				}
				else
				{
					if ( string.IsNullOrEmpty( this.connectionString ) )
					{
						this.dataSet = new DataSet();
					}
					else
					{
						IProvider provider = NDOProviderFactory.Instance[this.connectionType];
						if ( provider == null )
							throw new Exception( "Can't find NDO provider '" + this.connectionType + "'" );

						IDbConnection conn = provider.NewConnection( this.connectionString );
						conn.Open();
						try
						{
							this.dataSet = provider.GetDatabaseStructure( conn, null );
						}
						catch ( Exception ex )
						{
							throw ex;
						}
						finally
						{
							conn.Close();
						}
					}
				}
			}
			catch ( Exception ex )
			{
				MessageBox.Show("Can't convert the schema into a dataset. Error Message: " + ex.ToString());
			}
		}

		/// <summary>
		/// For serialization only
		/// </summary>
		public Database(){}

		DataSet dataSet;
		public DataSet DataSet
		{
			get { return this.dataSet; }
		}

		string xmlSchemaFile;
		public string XmlSchemaFile
		{
			get { return this.xmlSchemaFile; }
		}
		bool isXmlSchema;
		public bool IsXmlSchema
		{
			get { return this.isXmlSchema; }
		}
		string connectionString;
		public string ConnectionString
		{
			get { return this.connectionString; }
		}
		string connectionType;
		public string ConnectionType
		{
			get { return this.connectionType; }
		}
		string ownerName;
		public string OwnerName
		{
			get { return this.ownerName; }
		}
		#region IXmlStorable Member

		public void ToXml(XmlElement element)
		{
			element.SetAttribute("ConnectionString", this.connectionString);
			element.SetAttribute("ConnectionType", this.connectionType);
			element.SetAttribute("OwnerName", this.ownerName);
		}

		public void FromXml(XmlElement element)
		{
			this.connectionString = element.Attributes["ConnectionString"].Value;
			this.connectionType = element.Attributes["ConnectionType"].Value;
			this.ownerName = element.Attributes["OwnerName"].Value;			
		}

		#endregion
	}
}
