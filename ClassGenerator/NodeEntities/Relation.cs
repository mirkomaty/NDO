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
