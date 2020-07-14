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
using System.Collections;
using NDO.Mapping;

namespace NDO
{
	/// <summary>
	/// This class is used by the framework. Don't use this class in your own applications.
	/// </summary>
	[Serializable]
	public class LoadState
	{
		[NonSerialized]
		BitArray relationLoadState;
		[NonSerialized]
		IList lostRowInfo;
		// We implement the field load state as an internal field instead of an array
		// for faster access. There is a lot of code using this array.
		[NonSerialized]
		internal BitArray FieldLoadState;

		/// <summary>
		/// Max size of the relation count
		/// </summary>
		public const int RelationLoadStateSize = 64;

		/// <summary>
		/// Don't use this class in your applications.
		/// </summary>
		public IList LostRowInfo
		{
			get { return lostRowInfo; }
			set { lostRowInfo = value; }
		}

		/// <summary>
		/// Don't use this class in your applications.
		/// </summary>
		public BitArray RelationLoadState
		{
			get { return relationLoadState; }
			set { relationLoadState = value; }
		}

		/// <summary>
		/// Don't use this class in your applications.
		/// </summary>
		public LoadState()
		{
			this.relationLoadState = new BitArray(RelationLoadStateSize);
			lostRowInfo = null;
		}

		/// <summary>
		/// Internal helper Method
		/// </summary>
		/// <param name="r"></param>
		/// <param name="key"></param>
		public void ReplaceRowInfos(Relation r, Key key)
        {
            int i = 0;
            foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
            {
                ReplaceRowInfo(fkColumn.Name, key[i]);
            }

			if (r.ForeignKeyTypeColumnName != null)
            {
                ReplaceRowInfo(r.ForeignKeyTypeColumnName, key.TypeId);
            }
        }

		/// <summary>
		/// Internal helper Method
		/// </summary>
		/// <param name="oidKey"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public bool LostRowsEqualsOid(Key oidKey, Relation r)
        {
            int i = 0;
            bool result = true;
            new ForeignKeyIterator(r).Iterate(delegate(ForeignKeyColumn fkColumn, bool isLastEntry)
                {
                    KeyValuePair kvp = ((KeyValueList)this.LostRowInfo)[fkColumn.Name];
                    System.Diagnostics.Debug.Assert(kvp != null);
                    object oidValue = oidKey[i];
                    if (/*oidValue is Guid && */kvp.Value is string)  // If Guids are mapped to strings...
                    {
                        if (oidValue.ToString() != kvp.Value.ToString())
                            result = false;
                    }
                    else
                    {
                        if (!oidValue.Equals(kvp.Value))
                            result = false;
                    }
                    i++;
                }
            );
            return result;
        }

		/// <summary>
		/// Internal helper Method
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void ReplaceRowInfo(string key, object value)
		{

			KeyValueList kvl = (KeyValueList) lostRowInfo;
			KeyValuePair pair = kvl[key];
			if (pair.Key != null)
			{
				pair.Value = value;
			}
			else
			{
				kvl.Add(new KeyValuePair(key, value));
			}
		}

	}
}
