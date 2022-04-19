//
// Copyright (c) 2002-2019 Mirko Matytschak 
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
using MessageBox = System.Windows.Forms.MessageBox;
using EnvDTE;
using System.Text.RegularExpressions;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace NDOVsPackage.Commands
{
	/// <summary>
	/// Zusammenfassung für AddAccessorCs.
	/// </summary>
	internal class AddAccessorCs
	{
		TextDocument textDoc;
		Document document;

		public AddAccessorCs(TextDocument textDoc, Document document)
		{
			this.document = document;
			this.textDoc = textDoc;
		}

		public void DoIt()
		{
			bool genChangeEvent = false;

			try 
			{
				string result;
				int textLine = textDoc.Selection.TopLine;
				if (textLine != textDoc.Selection.BottomLine)
					return;
				textDoc.Selection.SelectLine();
				string original = textDoc.Selection.Text;
				textDoc.Selection.LineUp(false, 1);
				textDoc.Selection.SelectLine();
				string attrtext = textDoc.Selection.Text;
				textDoc.Selection.CharRight(false, 1);
				textDoc.Selection.LineDown(false, 1);

				int i = 0;
				string bl = string.Empty;
				while (char.IsWhiteSpace(original[i]))
					bl += original[i++];
				string selLine = original.Trim();
				Regex regex = new Regex(@"(private[\s]*|)([^\s]+(\s*<[^\s]+|))\s+([^\s^;^=]+)\s*(=|;)");
				Match match = regex.Match(selLine);
				if (!match.Success)
				{
					MessageBox.Show("Please select a private member variable declaration");
					return;
				}
				string typeStr = match.Groups[2].Value;
				string bigname;
				string name = match.Groups[4].Value;
				if (name.StartsWith("_"))
					bigname = name.Substring(1);
				else if (name.StartsWith("m_"))
					bigname = name.Substring(2);
				else
					bigname = name;
				bigname = bigname.Substring(0, 1).ToUpper() + bigname.Substring(1);

				string genericTypeStr = string.Empty;
				string genericArgumentType = string.Empty;
                string genericParameter = string.Empty;
				regex = new Regex(@"([^\s]+)\s*<([^>]+)>");
				match = regex.Match(typeStr);
				if (match.Success)
				{
					genericTypeStr = match.Groups[1].Value;
					genericArgumentType = match.Groups[2].Value;
                    genericParameter = "<" + genericArgumentType + '>';
				}

				bool isContainer = 	(typeStr == "IList" || typeStr == "ArrayList");
				if (genericTypeStr != string.Empty)
				{
					isContainer = isContainer || (genericTypeStr == "IList" || genericTypeStr == "List");
				}

				bool isGenericList = genericTypeStr != string.Empty && isContainer;

//				bool isIList = (typeStr == "IList" || genericTypeStr == "IList");

				if (isContainer)
				{
					attrtext = attrtext.Trim();
					string elementTyp = null;
					if (!isGenericList)
						elementTyp = GetElementTyp(attrtext);
					else
						elementTyp = genericArgumentType;

					string relationName = GetRelationName(attrtext);

					if (elementTyp == null)
					{
						isContainer = false;
					}
					else
					{

						if (relationName == null)
						{
							int p = elementTyp.LastIndexOf(".");
							relationName = elementTyp.Substring(p + 1);
						}

						bool isComposite = (attrtext.IndexOf("RelationInfo.Composite") > -1);
						string parameter = elementTyp.Substring(0,1).ToLower();
						result = string.Empty;
						if (isComposite)
						{
							result += bl + "public " + elementTyp + " New" + relationName + "()\n";
							result += bl + "{\n";
							result += bl + "\t" + elementTyp + " " + parameter + " = new " + elementTyp + "();\n";
							result += bl + "\tthis." + name + ".Add(" + parameter + ");\n";
							result += bl + "\t" + "return " + parameter + ";\n";
							result += bl + "}\n";
						}
						else
						{
							result += bl + "public void Add" + relationName + "(" + elementTyp + " " + parameter + ")\n";
							result += bl + "{\n";
							result += bl + "\tthis." + name + ".Add(" + parameter + ");\n";
							result += bl + "}\n";
						}
						result += bl + "public void Remove" + relationName + "(" + elementTyp + " " + parameter + ")\n";
						result += bl + "{\n";
						result += bl + "\tif (this." + name + ".Contains(" + parameter + "))\n";
						result += bl + "\t\tthis." + name + ".Remove(" + parameter + ");\n";
						result += bl + "}\n";
						textDoc.Selection.Insert(result, (int)vsInsertFlags.vsInsertFlagsInsertAtStart);
					}
				}
				if (!isContainer) // isContainer may change in the if case, so we test it again
				{
					ConfigurationOptions options = new ConfigurationOptions(document.ProjectItem.ContainingProject);
					if (options.GenerateChangeEvents)
					{
						genChangeEvent = true;
						result = bl + "public event EventHandler " + bigname + "Changed;\n";
						textDoc.Selection.Insert(result, (int)vsInsertFlags.vsInsertFlagsInsertAtStart);
					}
				}

				result = string.Empty;

                string ilistType = null;
                if (isGenericList)
                {
                    ilistType = "IEnumerable" + genericParameter;
                }
                else
                    ilistType = "IEnumerable"; 

				if (isContainer)
					result += bl + "public " + ilistType + " " + bigname + '\n';
				else
					result += bl + "public " + typeStr + " " + bigname + '\n';
				result += bl + "{\n";

				result += bl + "\tget => this." + name + "; \n";

				if (genChangeEvent)  // Set Accessor in mehreren Zeilen
				{
					result += bl + "\tset\n";
					result += bl + "\t{\n";
					if (isContainer)
					{
						if (!isGenericList)
							result += bl + "\t\tthis." + name + " = new ArrayList( (ICollection)value );\n";
						else
							result += bl + "\t\tthis." + name + " = value.ToList();\n";
					}
					else
					{
						result += bl + "\t\tthis." + name + " = value;\n";
					}
					result += bl + "\t\tif (" + bigname + "Changed != null)\n";
					result += bl + "\t\t\t" + bigname + "Changed(this, EventArgs.Empty);\n";
					result += bl +"\t}\n";
				}
				else  // Accessor in einer Zeile
				{
					if (isContainer)
						if (!isGenericList)
							result += bl + "\tset => this." + name + " = new ArrayList( (ICollection)value );\n";
						else
							result += bl + "\tset => this." + name + " = value.ToList();\n";
					else
						result += bl + "\tset => this." + name + " = value;\n";
				}

				result += bl + "}\n";
				TabProperty tp = TabProperties.Instance.CSharp;
				if (tp.UseSpaces)
					result = result.Replace("\t", tp.Indent);
				textDoc.Selection.Insert(result, (int)vsInsertFlags.vsInsertFlagsInsertAtStart);
			}				
			catch (Exception e) 
			{
				MessageBox.Show(e.Message, "Add Accessor Add-in");
			}
		}

		private string GetElementTyp(string attrtext)
		{
			Regex regex = new Regex(@"\[\s*NDORelation\s*\(\s*typeof\s*\(\s*([^\s^\)]+)");
			Match match = regex.Match(attrtext);
			if (match.Success)
			{
				return match.Groups[1].Value;
			}
			return null;
		}

		private string GetRelationName(string attrtext)
		{
			string result;
			Regex regex = new Regex(@"\[\s*NDORelation");
			Match match = regex.Match(attrtext);
			if (!match.Success)
				return null;
			regex = new Regex(@"("")([^""]+)("")");
			match = regex.Match(attrtext);
			if (match.Success) // wir haben einen Relationsnamen
			{
				result = match.Groups[2].Value;
				if (char.IsLower(result[0]))
					result = result.Substring(0, 1).ToUpper() + result.Substring(1);
				return result;
			}
			return null;
		}

	}
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
