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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Generator
{

	class Diff
	{
		/// <summary>
		/// Difference Item.
		/// </summary>
		public struct Item
		{
			/// <summary>Start Line number in Data A.</summary>
			public int StartA;
			/// <summary>Start Line number in Data B.</summary>
			public int StartB;

			/// <summary>Number of changes in Data A.</summary>
			public int DeletedA;
			/// <summary>Number of changes in Data B.</summary>
			public int InsertedB;
		} // Item

		/// <summary>
		/// Shortest Middle Snake Return Data
		/// </summary>
		private struct SmsReturnData
		{
			internal int x, y;
		}




		/// <summary>
		/// Find the difference in 2 text documents, comparing by textlines.
		/// The algorithm itself is comparing 2 arrays of numbers so when comparing 2 text documents
		/// each line is converted into a (hash) number. This hash-value is computed by storing all
		/// textlines into a common hashtable so i can find dublicates in there, and generating a 
		/// new number each time a new textline is inserted.
		/// </summary>
		/// <param name="TextA">A-version of the text (usualy the old one)</param>
		/// <param name="TextB">B-version of the text (usualy the new one)</param>
		/// <param name="trimSpace">When set to true, all leading and trailing whitespace characters are stripped out before the comparation is done.</param>
		/// <param name="ignoreSpace">When set to true, all whitespace characters are converted to a single space character before the comparation is done.</param>
		/// <param name="ignoreCase">When set to true, all characters are converted to their lowercase equivivalence before the comparation is done.</param>
		/// <returns>Returns a array of Items that describe the differences.</returns>
		public static Item[] DiffText( List<string> TextA, List<string> TextB, bool trimSpace, bool ignoreSpace, bool ignoreCase )
		{
			Hashtable codeHash = new Hashtable( TextA.Count + TextB.Count );

			DiffData DataA = new DiffData( GenerateDiffCodes( TextA, codeHash, trimSpace, ignoreSpace, ignoreCase ) );

			DiffData DataB = new DiffData( GenerateDiffCodes( TextB, codeHash, trimSpace, ignoreSpace, ignoreCase ) );

			codeHash = null; // free up hashtable memory (maybe)

			int combinedLength = DataA.Length + DataB.Length + 1;
			/// vector for the (0,0) to (x,y) search
			int[] DownVector = new int[2 * combinedLength + 2];
			/// vector for the (u,v) to (N,M) search
			int[] UpVector = new int[2 * combinedLength + 2];

			LongesCommonSequence( DataA, 0, DataA.Length, DataB, 0, DataB.Length, DownVector, UpVector );

			Optimize( DataA );
			Optimize( DataB );
			return CreateDiffs( DataA, DataB );
		} // DiffText


		/// <summary>
		/// If a sequence of modified lines starts with a line that contains the same content
		/// as the line that appends the changes, the difference sequence is modified so that the
		/// appended line and not the starting line is marked as modified.
		/// This leads to more readable diff sequences when comparing text files.
		/// </summary>
		/// <param name="Data">A Diff data buffer containing the identified changes.</param>
		private static void Optimize( DiffData Data )
		{
			int StartPos, EndPos;

			StartPos = 0;
			while ( StartPos < Data.Length )
			{
				while ( (StartPos < Data.Length) && (Data.modified[StartPos] == false) )
					StartPos++;
				EndPos = StartPos;
				while ( (EndPos < Data.Length) && (Data.modified[EndPos] == true) )
					EndPos++;

				if ( (EndPos < Data.Length) && (Data.data[StartPos] == Data.data[EndPos]) )
				{
					Data.modified[StartPos] = false;
					Data.modified[EndPos] = true;
				}
				else
				{
					StartPos = EndPos;
				} // if
			} // while
		} // Optimize


		/// <summary>
		/// Find the difference in 2 arrays of integers.
		/// </summary>
		/// <param name="ArrayA">A-version of the numbers (usualy the old one)</param>
		/// <param name="ArrayB">B-version of the numbers (usualy the new one)</param>
		/// <returns>Returns a array of Items that describe the differences.</returns>
		public static Item[] DiffInt( int[] ArrayA, int[] ArrayB )
		{
			// The A-Version of the data (original data) to be compared.
			DiffData DataA = new DiffData( ArrayA );

			// The B-Version of the data (modified data) to be compared.
			DiffData DataB = new DiffData( ArrayB );

			int MAX = DataA.Length + DataB.Length + 1;
			/// vector for the (0,0) to (x,y) search
			int[] DownVector = new int[2 * MAX + 2];
			/// vector for the (u,v) to (N,M) search
			int[] UpVector = new int[2 * MAX + 2];

			LongesCommonSequence( DataA, 0, DataA.Length, DataB, 0, DataB.Length, DownVector, UpVector );
			return CreateDiffs( DataA, DataB );
		} // Diff


		/// <summary>
		/// This method converts all textlines of the text into unique numbers for every unique textline.
		/// That way the core algorithm works with simple int numbers instead of strings.
		/// </summary>
		/// <param name="lines">The input text as string list.</param>
		/// <param name="codeHash">This extern initialized hashtable is used for storing all ever used textlines.</param>
		/// <param name="trimSpace">Ignore leading and trailing space characters</param>
		/// <param name="ignoreSpace">Transform blocks of white space into a single space before transforming a line into a string code.</param>
		/// <param name="ignoreCase">Ignore case.</param>
		/// <returns>An array of integers representing the string codes.</returns>
		private static int[] GenerateDiffCodes( List<string> lines, Hashtable codeHash, bool trimSpace, bool ignoreSpace, bool ignoreCase )
		{
			// get all codes of the text
			int[] codes;
			int lastUsedCode = codeHash.Count;
			object aCode;
			string s;


			codes = new int[lines.Count];

			for ( int i = 0; i < lines.Count; ++i )
			{
				s = lines[i];
				if ( trimSpace )
					s = s.Trim();

				if ( ignoreSpace )
				{
					s = Regex.Replace( s, "\\s+", "" );            // TODO: optimization: faster blank removal.
				}

				if ( ignoreCase )
					s = s.ToLower();

				aCode = codeHash[s];
				if ( aCode == null )
				{
					lastUsedCode++;
					codeHash[s] = lastUsedCode;
					codes[i] = lastUsedCode;
				}
				else
				{
					codes[i] = (int) aCode;
				} // if
			} // for
			return (codes);
		} // GenerateDiffCodes




		/// <summary>
		/// This is the algorithm to find the Shortest Middle Snake (SMS).
		/// </summary>
		/// <param name="DataA">sequence A</param>
		/// <param name="LowerA">lower bound of the actual range in DataA</param>
		/// <param name="UpperA">upper bound of the actual range in DataA (exclusive)</param>
		/// <param name="DataB">sequence B</param>
		/// <param name="LowerB">lower bound of the actual range in DataB</param>
		/// <param name="UpperB">upper bound of the actual range in DataB (exclusive)</param>
		/// <param name="DownVector">a vector for the (0,0) to (x,y) search. Passed as a parameter for speed reasons.</param>
		/// <param name="UpVector">a vector for the (u,v) to (N,M) search. Passed as a parameter for speed reasons.</param>
		/// <returns>a MiddleSnakeData record containing x,y and u,v</returns>
		private static SmsReturnData ShortestMiddleSnake( DiffData DataA, int LowerA, int UpperA, DiffData DataB, int LowerB, int UpperB,
		  int[] DownVector, int[] UpVector )
		{

			SmsReturnData ret;
			int MAX = DataA.Length + DataB.Length + 1;

			int DownK = LowerA - LowerB; // the k-line to start the forward search
			int UpK = UpperA - UpperB; // the k-line to start the reverse search

			int Delta = (UpperA - LowerA) - (UpperB - LowerB);
			bool oddDelta = (Delta & 1) != 0;

			// The vectors in the publication accepts negative indexes. the vectors implemented here are 0-based
			// and are access using a specific offset: UpOffset UpVector and DownOffset for DownVektor
			int DownOffset = MAX - DownK;
			int UpOffset = MAX - UpK;

			int MaxD = ((UpperA - LowerA + UpperB - LowerB) / 2) + 1;

			// Debug.Write(2, "SMS", String.Format("Search the box: A[{0}-{1}] to B[{2}-{3}]", LowerA, UpperA, LowerB, UpperB));

			// init vectors
			DownVector[DownOffset + DownK + 1] = LowerA;
			UpVector[UpOffset + UpK - 1] = UpperA;

			for ( int D = 0; D <= MaxD; D++ )
			{

				// Extend the forward path.
				for ( int k = DownK - D; k <= DownK + D; k += 2 )
				{
					// Debug.Write(0, "SMS", "extend forward path " + k.ToString());

					// find the only or better starting point
					int x, y;
					if ( k == DownK - D )
					{
						x = DownVector[DownOffset + k + 1]; // down
					}
					else
					{
						x = DownVector[DownOffset + k - 1] + 1; // a step to the right
						if ( (k < DownK + D) && (DownVector[DownOffset + k + 1] >= x) )
							x = DownVector[DownOffset + k + 1]; // down
					}
					y = x - k;

					// find the end of the furthest reaching forward D-path in diagonal k.
					while ( (x < UpperA) && (y < UpperB) && (DataA.data[x] == DataB.data[y]) )
					{
						x++;
						y++;
					}
					DownVector[DownOffset + k] = x;

					// overlap ?
					if ( oddDelta && (UpK - D < k) && (k < UpK + D) )
					{
						if ( UpVector[UpOffset + k] <= DownVector[DownOffset + k] )
						{
							ret.x = DownVector[DownOffset + k];
							ret.y = DownVector[DownOffset + k] - k;
							// ret.u = UpVector[UpOffset + k];      // 2002.09.20: no need for 2 points 
							// ret.v = UpVector[UpOffset + k] - k;
							return (ret);
						} // if
					} // if

				} // for k

				// Extend the reverse path.
				for ( int k = UpK - D; k <= UpK + D; k += 2 )
				{
					// Debug.Write(0, "SMS", "extend reverse path " + k.ToString());

					// find the only or better starting point
					int x, y;
					if ( k == UpK + D )
					{
						x = UpVector[UpOffset + k - 1]; // up
					}
					else
					{
						x = UpVector[UpOffset + k + 1] - 1; // left
						if ( (k > UpK - D) && (UpVector[UpOffset + k - 1] < x) )
							x = UpVector[UpOffset + k - 1]; // up
					} // if
					y = x - k;

					while ( (x > LowerA) && (y > LowerB) && (DataA.data[x - 1] == DataB.data[y - 1]) )
					{
						x--;
						y--; // diagonal
					}
					UpVector[UpOffset + k] = x;

					// overlap ?
					if ( !oddDelta && (DownK - D <= k) && (k <= DownK + D) )
					{
						if ( UpVector[UpOffset + k] <= DownVector[DownOffset + k] )
						{
							ret.x = DownVector[DownOffset + k];
							ret.y = DownVector[DownOffset + k] - k;
							// ret.u = UpVector[UpOffset + k];     // 2002.09.20: no need for 2 points 
							// ret.v = UpVector[UpOffset + k] - k;
							return (ret);
						} // if
					} // if

				} // for k

			} // for D

			throw new ApplicationException( "the algorithm should never come here." );
		} // SMS


		/// <summary>
		/// This is the divide-and-conquer implementation of the longes common-subsequence (LCS) 
		/// algorithm.
		/// The published algorithm passes recursively parts of the A and B sequences.
		/// To avoid copying these arrays the lower and upper bounds are passed while the sequences stay constant.
		/// </summary>
		/// <param name="DataA">sequence A</param>
		/// <param name="LowerA">lower bound of the actual range in DataA</param>
		/// <param name="UpperA">upper bound of the actual range in DataA (exclusive)</param>
		/// <param name="DataB">sequence B</param>
		/// <param name="LowerB">lower bound of the actual range in DataB</param>
		/// <param name="UpperB">upper bound of the actual range in DataB (exclusive)</param>
		/// <param name="DownVector">a vector for the (0,0) to (x,y) search. Passed as a parameter for speed reasons.</param>
		/// <param name="UpVector">a vector for the (u,v) to (N,M) search. Passed as a parameter for speed reasons.</param>
		private static void LongesCommonSequence( DiffData DataA, int LowerA, int UpperA, DiffData DataB, int LowerB, int UpperB, int[] DownVector, int[] UpVector )
		{
			// Debug.Write(2, "LCS", String.Format("Analyse the box: A[{0}-{1}] to B[{2}-{3}]", LowerA, UpperA, LowerB, UpperB));

			// Fast walkthrough equal lines at the start
			while ( LowerA < UpperA && LowerB < UpperB && DataA.data[LowerA] == DataB.data[LowerB] )
			{
				LowerA++;
				LowerB++;
			}

			// Fast walkthrough equal lines at the end
			while ( LowerA < UpperA && LowerB < UpperB && DataA.data[UpperA - 1] == DataB.data[UpperB - 1] )
			{
				--UpperA;
				--UpperB;
			}

			if ( LowerA == UpperA )
			{
				// mark as inserted lines.
				while ( LowerB < UpperB )
					DataB.modified[LowerB++] = true;

			}
			else if ( LowerB == UpperB )
			{
				// mark as deleted lines.
				while ( LowerA < UpperA )
					DataA.modified[LowerA++] = true;

			}
			else
			{
				// Find the middle snakea and length of an optimal path for A and B
				SmsReturnData smsrd = ShortestMiddleSnake( DataA, LowerA, UpperA, DataB, LowerB, UpperB, DownVector, UpVector );
				// Debug.Write(2, "MiddleSnakeData", String.Format("{0},{1}", smsrd.x, smsrd.y));

				// The path is from LowerX to (x,y) and (x,y) to UpperX
				LongesCommonSequence( DataA, LowerA, smsrd.x, DataB, LowerB, smsrd.y, DownVector, UpVector );
				LongesCommonSequence( DataA, smsrd.x, UpperA, DataB, smsrd.y, UpperB, DownVector, UpVector );  // 2002.09.20: no need for 2 points 
			}
		} // LongesCommonSequence


		/// <summary>
		/// Scan the tables of which lines are inserted and deleted,
		/// producing an edit script in forward order.
		/// </summary>
		/// dynamic array
		private static Item[] CreateDiffs( DiffData DataA, DiffData DataB )
		{
			ArrayList a = new ArrayList();
			Item aItem;
			Item[] result;

			int StartA, StartB;
			int LineA, LineB;

			LineA = 0;
			LineB = 0;
			while ( LineA < DataA.Length || LineB < DataB.Length )
			{
				if ( (LineA < DataA.Length) && (!DataA.modified[LineA])
          && (LineB < DataB.Length) && (!DataB.modified[LineB]) )
				{
					// equal lines
					LineA++;
					LineB++;

				}
				else
				{
					// maybe deleted and/or inserted lines
					StartA = LineA;
					StartB = LineB;

					while ( LineA < DataA.Length && (LineB >= DataB.Length || DataA.modified[LineA]) )
						// while (LineA < DataA.Length && DataA.modified[LineA])
						LineA++;

					while ( LineB < DataB.Length && (LineA >= DataA.Length || DataB.modified[LineB]) )
						// while (LineB < DataB.Length && DataB.modified[LineB])
						LineB++;

					if ( (StartA < LineA) || (StartB < LineB) )
					{
						// store a new difference-item
						aItem = new Item();
						aItem.StartA = StartA;
						aItem.StartB = StartB;
						aItem.DeletedA = LineA - StartA;
						aItem.InsertedB = LineB - StartB;
						a.Add( aItem );
					} // if
				} // if
			} // while

			result = new Item[a.Count];
			a.CopyTo( result );

			return (result);
		}

	} // class Diff

	/// <summary>Data on one input file being compared.  
	/// </summary>
	internal class DiffData
	{

		/// <summary>Number of elements (lines).</summary>
		internal int Length;

		/// <summary>Buffer of numbers that will be compared.</summary>
		internal int[] data;

		/// <summary>
		/// Array of booleans that flag for modified data.
		/// This is the result of the diff.
		/// This means deletedA in the first Data or inserted in the second Data.
		/// </summary>
		internal bool[] modified;

		/// <summary>
		/// Initialize the Diff-Data buffer.
		/// </summary>
		/// <param name="data">reference to the buffer</param>
		internal DiffData( int[] initData )
		{
			data = initData;
			Length = initData.Length;
			modified = new bool[Length + 2];
		} // DiffData

	} // class DiffData

} // namespace