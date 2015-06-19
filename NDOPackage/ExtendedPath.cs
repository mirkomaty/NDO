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

namespace NDOEnhancer
{
	/// <summary>
	/// Zusammenfassung für ExtendedPath.
	/// </summary>
	public class ExtendedPath
	{
		public static string GetRelativePath(string referencePath, string targetPath)
		{
			string pathRoot1;
			string pathRoot2;

			pathRoot1 = Path.GetPathRoot(referencePath);
			pathRoot2 = Path.GetPathRoot(targetPath);
			if (pathRoot1 == string.Empty && pathRoot2 == string.Empty)
				throw new ArgumentException("Can't compute the relative path of two relative paths", "targetPath");

			if (String.Compare(pathRoot1, pathRoot2, true) != 0) // includes pathRoot1 == empty
				return targetPath;

			string rumpf1 = referencePath.Substring(pathRoot1.Length);
			string rumpf2 = targetPath.Substring(pathRoot2.Length);
			if (rumpf1.EndsWith("\\") || rumpf1.EndsWith("/"))
				rumpf1 = rumpf1.Substring(0, rumpf1.Length - 1);

			char[] sepChars = new char[]{Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};
			string[] arr1 = rumpf1.Split(sepChars);
			string[] arr2 = rumpf2.Split(sepChars);

			int i;
			for (i = 0; i < Math.Min(arr1.Length, arr2.Length); i++)
			{
				if (string.Compare(arr1[i], arr2[i], true) != 0)
					break;
			}

			string result = string.Empty;
			string backpath = ".." + Path.DirectorySeparatorChar;
			for (int j = arr1.Length - 1; j >= i; j--)
			{
				result += backpath;
			}
			for (int j = i; j < arr2.Length; j++)
			{
				result += arr2[j];
				if (j < arr2.Length - 1)
					result += Path.DirectorySeparatorChar;
			}

			return result;
		}
	}
}
