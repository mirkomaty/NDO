﻿//
// Copyright (c) 2002-2022 Mirko Matytschak 
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

namespace NDOEnhancer.ILCode
{
	/// <summary>
	/// Summary description for ILPropertyElement.
	/// </summary>
	internal class ILPropertyElement : ILElement
	{
		public ILPropertyElement()
		{
		}

		public ILPropertyElement( string firstLine, ILElement owner )
			: base( firstLine, owner )
		{
		}

		private string m_name;

		internal class ILPropertyElementType : ILElementType
		{
			public ILPropertyElementType()
				: base( ".property", typeof( ILPropertyElement ) )
			{
			}
		}

		private static ILElementType		m_elementType = new ILPropertyElementType();
		public override bool NeedsBlock => true;

		public string Name
		{
			get
			{
				Resolve();
				return m_name;
			}
		}


		private void Resolve()
		{
			if ( this.m_name == null )
			{
				string[] all = GetAllLines().Split( ' ' );
				this.m_name = all[all.Length - 1].Replace( "()", string.Empty );
			}
		}

		public void AddGetter( string firstLine )
		{
			AddElement( new ILGetElement( firstLine ) );
		}

		public void AddSetter( string firstLine )
		{
			AddElement( new ILSetElement( firstLine ) );
		}
	}
}
