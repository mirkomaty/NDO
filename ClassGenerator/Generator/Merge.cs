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
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Generator
{
	class Merge
	{
		static string commentPrefix;
		public static string CommentPrefix
		{
			get { return commentPrefix; }
			set { commentPrefix = value; }
		}

		static bool ignoreSpaces;
		public static bool IgnoreSpaces
		{
			get { return ignoreSpaces; }
			set { ignoreSpaces = value; }
		}

		public static bool MergeFiles( string fileNameA, Stream fileStreamB, string mergedFileName )
		{
			List<string> oldLines = ExtractLines( fileNameA );
			List<string> newLines = ExtractLines( fileStreamB );
			return MergeLines( oldLines, newLines, mergedFileName );
		}

		public static bool MergeFiles( string fileNameA, string fileNameB, string mergedFileName )
		{
			List<string> oldLines = ExtractLines( fileNameA );
			List<string> newLines = ExtractLines( fileNameB );
			return MergeLines( oldLines, newLines, mergedFileName );
		}

		private static bool MergeLines( List<string> oldLines, List<string> newLines, string mergedFileName )
		{
			Diff.Item[] items = Diff.DiffText( oldLines, newLines, true, ignoreSpaces, false );
			StreamWriter sw = new StreamWriter( mergedFileName );
			bool hasConflicts = Merge.Write( items, sw, oldLines, newLines );
			sw.Close();
			return hasConflicts;
		}

		private static List<string> ExtractLines( string fileName )
		{
			List<string> lines = new List<string>(100);
			using ( StreamReader sr = new StreamReader( fileName ) )
			{
				string line;
				while ( ( line = sr.ReadLine() ) != null )
					lines.Add( line );
				sr.Close();
			}
			return lines;
		}

		private static List<string> ExtractLines( Stream fileStream )
		{
			List<string> lines = new List<string>(100);
			// The stream remains open in this case, since it is passed in from the outside.
			StreamReader sr = new StreamReader( fileStream );
			string line;
			while ( (line = sr.ReadLine()) != null )
				lines.Add( line );

			return lines;
		}

		static bool Write( Diff.Item[] items, StreamWriter sw, List<string> oldLines, List<string> newLines )
		{
			bool hasConflicts = false;
			int nextToWrite = 0;
			foreach ( Diff.Item item in items )
			{
				if ( item.InsertedB == 0 )  // ignore the deletion and write the old data
					continue;
				for ( int i = nextToWrite; i < item.StartA; i++ )
					sw.WriteLine( oldLines[i] );
				nextToWrite = item.StartA;
				if ( item.DeletedA > 0 )  // conflict
				{
					hasConflicts = true;
					sw.WriteLine( commentPrefix + " !!!! ClassGenerator merge conflict !!!! Your code follows: -->" );
					for ( int i = item.StartA; i < item.StartA + item.DeletedA; i++ )
						sw.WriteLine( oldLines[i] );
					nextToWrite = item.StartA + item.DeletedA;
					sw.WriteLine( commentPrefix + " !!!! The ClassGenerator's code follows: -->" );
					for ( int i = item.StartB; i < item.StartB + item.InsertedB; i++ )
						sw.WriteLine( newLines[i] );
					sw.WriteLine( commentPrefix + " !!!! End of merge conflict -->" );
				}
				else
				{
					for ( int i = item.StartB; i < item.StartB + item.InsertedB; i++ )
						sw.WriteLine( newLines[i] );
				}

			}
			if ( nextToWrite < oldLines.Count - 1 )
			{
				for ( int i = nextToWrite; i < oldLines.Count; i++ )
					sw.WriteLine( oldLines[i] );
			}
			return hasConflicts;
		}
	}
}
