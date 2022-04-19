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
using System.IO;
using MessageBox = System.Windows.Forms.MessageBox;
using EnvDTE;
using System.Text.RegularExpressions;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

namespace NDOVsPackage.Commands
{
	/// <summary>
	/// Zusammenfassung für AddAccessorCs.
	/// </summary>
	internal class AddAccessorVb
	{
		TextDocument textDoc;
		Document document;

		public AddAccessorVb(TextDocument textDoc, Document document)
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
				Regex regex = new Regex(@"<[^\>]+\>");
				Match match = regex.Match(original);
				string attrtext;
				if (match.Success)
				{
					attrtext = match.Value;
				}
				else
				{
					textDoc.Selection.LineUp(false, 1);
					textDoc.Selection.SelectLine();
					match = regex.Match(textDoc.Selection.Text);
                    attrtext = match.Value;
				}
                bool hasAttribute = match.Success;
				textDoc.Selection.CharRight(false, 1);
				textDoc.Selection.LineDown(false, 1);

//				int i = 0;
//				string bl = string.Empty;
//				while (char.IsWhiteSpace(original[i]))
//					bl += original[i++];
				TabProperty tp = TabProperties.Instance.VBasic;
				string bl = tp.Indent;

                string selLine = original.Trim();
                regex = new Regex(@"(Dim|Private)\s([^\s]+)\sAs\s(New\s|)([^\s^(]+)(\s*|)(\(Of\s*([^\s]+)\)|)", RegexOptions.IgnoreCase);
				match = regex.Match(selLine);
				if (!match.Success)
				{
					MessageBox.Show("Please select a private member variable declaration");
					return;
				}
				string typeStr = match.Groups[4].Value;
				string bigname;
				string name = match.Groups[2].Value;
                string genericParameter = match.Groups[6].Value;
                string genericArgumentType = match.Groups[7].Value;
                
				bool hasPrefix = false;

				if (name.StartsWith("_"))
				{
					bigname = name.Substring(1);
					hasPrefix = true;
				}
				else if (name.StartsWith("m_"))
				{
					bigname = name.Substring(2);
					hasPrefix = true;
				}
				else
					bigname = name;

				if (!hasPrefix)
					bigname = "P" + bigname;
				else
					bigname = bigname.Substring(0, 1).ToUpper() + bigname.Substring(1);

                bool isContainer = (typeStr == "IList" || typeStr == "List" || typeStr == "ArrayList" || typeStr == "NDOArrayList" || typeStr == "NDOGenericList");
                isContainer = hasAttribute && isContainer;
				
                bool isGenericList = genericParameter != string.Empty && genericArgumentType != string.Empty;
                if (isGenericList)
                    typeStr += genericParameter;

				if (isContainer)
				{
					attrtext = attrtext.Trim();
                    string elementTyp = null;
                    if (!isGenericList)
                        elementTyp = GetElementTyp(attrtext);
                    else
                        elementTyp = genericArgumentType;
					string relationName = GetRelationName(attrtext);

					if (relationName == null)
					{
						int p = elementTyp.LastIndexOf(".");
						relationName = elementTyp.Substring(p + 1);
					}

					bool isComposite = (attrtext.IndexOf("RelationInfo.Composite") > -1);
					string parameter = elementTyp.Substring(0, 1).ToLower();
					result = string.Empty;
					if (isComposite)
					{
						result += bl + "Public Function New" + relationName + "() As " + elementTyp + "\n";
						result += bl + "\tDim " + parameter + " As " + elementTyp + " = New " + elementTyp + "\n";
						result += bl + "\t" + name + ".Add(" + parameter + ")\n";
						result += bl + "\t" + "return " + parameter + "\n";
						result += bl + "End Function\n";
					}
					else
					{
						result += bl + "Public Sub Add" + relationName + "(" + parameter + " As " + elementTyp + ")\n";
						result += bl + "\t" + name + ".Add(" + parameter + ")\n";
						result += bl + "End Sub\n";
					}
					result += bl + "Public Sub Remove" + relationName + "(" + parameter + " As " + elementTyp + ")\n";
					result += bl + "\tIf " + name + ".Contains(" + parameter + ") Then\n";
					result += bl + "\t\t" + name + ".Remove(" + parameter + ")\n";
					result += bl + "\tEnd If\n";
					result += bl + "End Sub\n";
					textDoc.Selection.Insert(result, (int)vsInsertFlags.vsInsertFlagsInsertAtStart);
				}
				else // wir haben es mit einer Elementbeziehung bzw. einem Feld zu tun
				{
					ConfigurationOptions options = new ConfigurationOptions(document.ProjectItem.ContainingProject);
					if (options.GenerateChangeEvents)
					{
						genChangeEvent = true;
						result = bl + "Public Event " + bigname + "Changed As EventHandler\n";
						textDoc.Selection.Insert(result, (int)vsInsertFlags.vsInsertFlagsInsertAtStart);
					}
				}

                string ilistType = isGenericList ? "IEnumerable " + genericParameter : "IEnumerable" ;                                    

				result = string.Empty;
				if (isContainer)
					result += bl + "Public Property " + bigname + "() As " + ilistType + "\n";
				else
					result += bl + "Public Property " + bigname + "() As " + typeStr + "\n";
				result += bl + "\tGet\n";
				result += bl + "\t\tReturn " + name + "\n";
				result += bl + "\tEnd Get\n";
				if (isContainer)
					result += bl + "\tSet(ByVal Value As " + ilistType +")\n";
				else
					result += bl + "\tSet(ByVal Value As " + typeStr + ")\n";
				if (isContainer)
					if (!isGenericList)
						result += bl + "\t\t" + name + " = New ArrayList(CType(Value, ICollection))\n";
					else
						result += bl + "\t\t" + name + " = Value.ToList()\n";
				else
					result += bl + "\t\t" + name + " = Value\n";
				if (genChangeEvent)
					result += bl + "\t\tRaiseEvent " + bigname + "Changed(Me, EventArgs.Empty)\n";
				result += bl + "\tEnd Set\n";
				result += bl + "End Property\n";
				textDoc.Selection.Insert(result, (int)vsInsertFlags.vsInsertFlagsInsertAtStart);
			}				
			catch (Exception e) 
			{
				MessageBox.Show(e.Message, "Add Accessor Add-in");
			}
		}

		private string GetElementTyp(string attrtext)
		{
			Regex regex = new Regex(@"<NDORelation\(GetType\(([^\)]+)");
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
			if (attrtext.IndexOf(@"NDORelation") == -1)
				return null;

			Regex regex = new Regex(@"("")([^""]+)("")");
			Match match = regex.Match(attrtext);
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
