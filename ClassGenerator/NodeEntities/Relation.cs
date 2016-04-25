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
using System.Xml;
using System.ComponentModel;
using System.Collections;

namespace ClassGenerator
{
	/// <summary>
	/// Zusammenfassung für Relation.
	/// </summary>
	[Serializable]
#if DEBUG
	public abstract class Relation : IXmlStorable
#else
	internal abstract class Relation : IXmlStorable
#endif
	{
		bool isForeign;

		public abstract string Name
		{
			get;
		}

		[Browsable(false)]
		public abstract bool IsElement
		{
			get;
		}

		[Browsable(false)]
		public bool IsForeign
		{
			get { return isForeign; }
			set { isForeign = value; }
		}

		public abstract bool IsComposite
		{
			get; set;
		}

		public abstract string FieldName
		{
			get; set;
		}

		public abstract string RelationName
		{
			get; set;
		}

		public abstract RelationDirection RelationDirection
		{
			get; set;
		}

		public abstract CodingStyle CodingStyle
		{
			get; set;
		}
		#region IXmlStorable Member

		public virtual void ToXml(XmlElement element)
		{
			element.SetAttribute("IsForeign", this.isForeign.ToString());
		}

		public virtual void FromXml(XmlElement element)
		{
			this.isForeign = bool.Parse(element.Attributes["IsForeign"].Value);
		}

		#endregion
	}
}
