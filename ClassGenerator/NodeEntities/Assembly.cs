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
using System.Xml;
using System.ComponentModel;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für Assembly.
	/// </summary>
	[Serializable]
#if DEBUG
	public class Assembly : IXmlStorable
#else
	internal class Assembly : IXmlStorable
#endif
	{
		string projectName;
		[Description("Name of the Visual Studio Project.")]
		public string ProjectName
		{
			get { return projectName; }
			set { projectName = value; }
		}
		string rootNamespace;
		[Description("All classes and the C# project file will get this namespace as default.")]
		public string RootNamespace
		{
			get { return rootNamespace; }
			set { rootNamespace = value; }
		}
		string targetDir;
		[Description("Directory, in which NDO generates the class files and C# project file.")]
		public string TargetDir
		{
			get { return targetDir; }
			set { targetDir = value; }
		}

		TargetLanguage targetLanguage;
		[Description("Computer Language in which the classes will be generated.")]
		public TargetLanguage TargetLanguage
		{
			get { return targetLanguage; }
			set { targetLanguage = value; }
		}

		bool mapStringsAsGuids;
		[Description("Set this value to true, if a String Primary Key should be initialized with a Guid value by NDO.")]
		public bool MapStringsAsGuids
		{
			get { return mapStringsAsGuids; }
			set 
			{ 
				mapStringsAsGuids = value; 
				if (value == true)
					this.useClassField = false;
			}
		}

		bool useClassField;
		[Description("If the Primary Key is a string value it will be mapped to a class field, if this property is set to true. Otherwise the Primary Key will be initialized by a callback of the Persistence Manager.")]
		public bool UseClassField
		{
			get { return useClassField; }
			set { useClassField = value; }
		}

		public Assembly(string projectName, string rootNamespace, string targetDir, bool useClassField, bool mapStringsAsGuids, TargetLanguage targetLanguage)
		{
			this.projectName = projectName;
			this.rootNamespace = rootNamespace;
			this.targetDir = targetDir;
			this.mapStringsAsGuids = mapStringsAsGuids;
			this.useClassField = useClassField;
			this.targetLanguage = targetLanguage;
		}

		/// <summary>
		/// Used by serialization only!
		/// </summary>
		public Assembly()
		{
		}

		public virtual void FromXml(XmlElement element)
		{
			this.projectName = element.Attributes["ProjectName"].Value;
			this.rootNamespace = element.Attributes["RootNamespace"].Value;
			this.targetDir = element.Attributes["TargetDir"].Value;
			this.mapStringsAsGuids = bool.Parse(element.Attributes["MapStringsAsGuids"].Value);
			this.useClassField = bool.Parse(element.Attributes["UseClassField"].Value);
			this.targetLanguage = (TargetLanguage) Enum.Parse(typeof(TargetLanguage), element.Attributes["TargetLanguage"].Value);
		}


		public void ToXml(XmlElement thisElement)
		{
			thisElement.SetAttribute("ProjectName", this.projectName);
			thisElement.SetAttribute("RootNamespace", this.rootNamespace);
			thisElement.SetAttribute("TargetDir", this.targetDir);
			thisElement.SetAttribute("MapStringsAsGuids", this.mapStringsAsGuids.ToString());
			thisElement.SetAttribute("UseClassField", this.useClassField.ToString());
			thisElement.SetAttribute("TargetLanguage", this.targetLanguage.ToString());
		}
	}
}
