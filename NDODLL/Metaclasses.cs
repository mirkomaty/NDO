#if nix
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
using System.Collections.Concurrent;
using System.Reflection;

namespace NDO
{
	/// <summary>
	/// Summary description for Metaclasses.
	/// </summary>
	internal class Metaclasses
	{
		private static ConcurrentDictionary<Type, IMetaClass2> theClasses = new ConcurrentDictionary<Type, IMetaClass2>();

		internal static IMetaClass2 GetClass( Type type )
		{
			if (type.IsGenericTypeDefinition)
				return null;

			IMetaClass2 mc;
			mc = theClasses.GetOrAdd( type, (t) =>
			{
				Type mcType = t.GetNestedType( "MetaClass", BindingFlags.NonPublic | BindingFlags.Public );
				if (null == mcType)
					throw new NDOException( 13, "Missing nested class 'MetaClass' for type '" + t.Name + "'; the type doesn't seem to be enhanced." );
				Type t2 = mcType;
				if (t2.IsGenericTypeDefinition)
					t2 = t2.MakeGenericType( t.GetGenericArguments() );
				var o = Activator.CreateInstance( t2, t );
				if (o is IMetaClass2 mc2)
					mc = mc2;
				else if (o is IMetaClass mc1)
					mc = new NDOMetaclass( t, mc1 );
				else
					throw new NDOException( 101010, $"MetaClass for type '{t.FullName}' must implement IMetaClass or IMetaClass2, but doesn't." );

				return mc;
			} );

			return mc;
		}
	}
}
#endif