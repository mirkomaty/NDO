//
// Copyright (c) 2002-2024 Mirko Matytschak 
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


namespace NDO.Mapping
{
	/// <summary>
	/// This class provides information about relation fields. It is used by the NDO framework only.
	/// </summary>
	/// <remarks>Do not use this class in your own code.</remarks>
	internal class RelationFieldInfo
	{
		/// <summary>
		/// The relation
		/// </summary>
		public Relation Rel;
		/// <summary>
		/// The table name of the relation
		/// </summary>
		public string TableName;

        /// <summary>
        /// Constructur of RelationFieldInfo
        /// </summary>
        /// <param name="rel"></param>
        /// <param name="tableName"></param>
        public RelationFieldInfo( Relation rel, string tableName )
		{
			Rel = rel;
			TableName = tableName;
		}
	}
}
