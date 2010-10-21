//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Collections;
using System.Xml;

namespace NDOEnhancer
{
    /// <summary>
    /// Attention!!!!! This code is essentially the same as in ConfigurationOptions.cs 
    /// in the Add-in project.
    /// So, if you change one of them, be aware to change the second.
    /// </summary>
    internal class ConfigurationOptions
	{
		public ConfigurationOptions()
		{
			if (m_globals == null)
				m_globals = new Hashtable();
		}

		public ConfigurationOptions( XmlDocument doc )
		{
			if (m_globals == null)
				m_globals = new Hashtable();

			string pns = XmlHelper.Pns(doc);
			XmlNode node = doc.SelectSingleNode("//" + pns + "Enhancer/" + pns + "Options", XmlHelper.Nsmgr);
			if (node == null)
				throw new Exception("Parameters must have at least one //Enhancer/Options entry.");

			this.NewMapping = (bool) XmlHelper.GetNode(node, pns + "NewMapping", false);
			this.GenerateSQL = (bool) XmlHelper.GetNode(node, pns + "GenerateSQL", true);
			this.SQLScriptLanguage = (string) XmlHelper.GetNode(node, pns + "SQLScriptLanguage", "SqlServer");
			this.SchemaVersion = (string)XmlHelper.GetNode(node, pns + "SchemaVersion", "");
			this.UseTimeStamps = (bool) XmlHelper.GetNode(node, pns + "UseTimeStamps", false);
			this.DatabaseOwner = (string) XmlHelper.GetNode(node, pns + "DatabaseOwner", string.Empty);
			this.DefaultConnection = (string) XmlHelper.GetNode(node, pns + "DefaultConnection", string.Empty);
			this.EnableAddIn = (bool) XmlHelper.GetNode(node, pns + "EnableAddIn", true);
			this.EnableEnhancer = (bool) XmlHelper.GetNode(node, pns + "EnableEnhancer", true);
            this.IncludeTypecodes = (bool)XmlHelper.GetNode(node, pns + "IncludeTypecodes", false);
            this.GenerateChangeEvents = (bool)XmlHelper.GetNode(node, pns + "GenerateChangeEvents", false);
			this.Utf8Encoding = (bool) XmlHelper.GetNode(node, pns + "Utf8Encoding", true);
            this.VerboseMode = (bool)XmlHelper.GetNode(node, pns + "VerboseMode", false);
			this.GenerateConstraints = (bool)XmlHelper.GetNode(node, pns + "GenerateConstraints", false);
			this.DropExistingElements = (bool)XmlHelper.GetNode(node, pns + "DropExistingElements", true);
		}

		private static Hashtable m_globals = null;

		private void SetBoolValue(string fieldName, bool val)
		{
			if (null == m_globals) return;

			if (m_globals.Contains(fieldName))
				m_globals.Remove(fieldName);
			
			m_globals[fieldName] = val;
		}

		private bool GetBoolValue(string fieldName)
		{
			if (!m_globals.Contains(fieldName))
				return false;
			return (bool) m_globals[fieldName];
		}


		private void SetStringValue(string fieldName, string val)
		{
			if (null == m_globals) return;

			if (m_globals.Contains(fieldName))
				m_globals.Remove(fieldName);
			
			m_globals[fieldName] = val;
		}

		private String GetStringValue(string fieldName)
		{
			if (!m_globals.Contains(fieldName))
				return string.Empty;
			return (string) m_globals[fieldName];
		}


		public bool	EnableAddIn
		{
			get
			{
				return GetBoolValue("NDOEnableAddIn");
			}
			set
			{
				SetBoolValue("NDOEnableAddIn", value);
			}
		}


        public bool IncludeTypecodes
		{
			get
			{
                return GetBoolValue("IncludeTypecodes");
			}
			set
			{
                SetBoolValue("IncludeTypecodes", value);
			}
		}

        public bool VerboseMode
        {
            get
            {
                return GetBoolValue("VerboseMode");
            }
            set
            {
                SetBoolValue("VerboseMode", value);
            }
        }

		public bool	UseTimeStamps
		{
			get
			{
				return GetBoolValue("NDOUseTimeStamps");
			}
			set
			{
				SetBoolValue("NDOUseTimeStamps", value);
			}
		}

		public bool	GenerateChangeEvents
		{
			get
			{
				return GetBoolValue("NDOGenerateChangeEvents");
			}
			set
			{
				SetBoolValue("NDOGenerateChangeEvents", value);
			}
		}

		public bool	EnableEnhancer
		{
			get
			{
				return GetBoolValue("NDOEnableEnhancer");
			}
			set
			{
				SetBoolValue("NDOEnableEnhancer", value);
			}
		}

		public bool Utf8Encoding
		{
			get { return GetBoolValue("NDOUtf8Encoding"); }
			set { SetBoolValue("NDOUtf8Encoding", value); }
		}

		public bool	GenerateSQL
		{
			get
			{
				return GetBoolValue("NDOGenerateSQL");
			}
			set
			{
				SetBoolValue("NDOGenerateSQL", value);
			}
		}

		public bool	GenerateConstraints
		{
			get
			{
				return GetBoolValue("NDOGenerateConstraints");
			}
			set
			{
				SetBoolValue("NDOGenerateConstraints", value);
			}
		}

		public bool	DropExistingElements
		{
			get
			{
				return GetBoolValue("NDODropExistingElements");
			}
			set
			{
				SetBoolValue("NDODropExistingElements", value);
			}
		}


		public bool	NewMapping
		{
			get
			{
				return GetBoolValue("NDONewMapping");
			}
			set
			{
				SetBoolValue("NDONewMapping", value);
			}
		}

		public string DefaultConnection
		{
			get
			{
				return GetStringValue("NDODefaultConnection");
			}
			set
			{
				SetStringValue("NDODefaultConnection", value);
			}
		}

		public string SchemaVersion
		{
			get
			{
				return GetStringValue("SchemaVersion");
			}
			set
			{
				SetStringValue("SchemaVersion", value);
			}
		}

		public string SQLScriptLanguage
		{
			get
			{
				string s = GetStringValue("NDOSQLScriptLanguage");
				if (s == string.Empty)
					return "SqlServer";
				return s;
			}
			set
			{
				SetStringValue("NDOSQLScriptLanguage", value);
			}
		}

		public string DatabaseOwner
		{
			get 
			{
				return GetStringValue("DatabaseOwner");
			}
			set
			{
				SetStringValue("DatabaseOwner", value);
			}
		}

	}
}
