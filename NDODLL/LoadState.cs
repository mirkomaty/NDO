//
// Copyright (C) 2002-2014 Mirko Matytschak 
// (www.netdataobjects.de)
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
// there is a commercial licence available at www.netdataobjects.de.
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

        public void ReplaceRowInfos(Relation r, Key key)
        {
            int i = 0;
            foreach (ForeignKeyColumn fkColumn in r.ForeignKeyColumns)
            {
                ReplaceRowInfo(fkColumn.Name, key[i]);
            }
#if PRO
            if (r.ForeignKeyTypeColumnName != null)
            {
                ReplaceRowInfo(r.ForeignKeyTypeColumnName, key.TypeId);
            }
#endif
        }


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
