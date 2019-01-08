//
// Copyright (c) 2002-2019 Mirko Matytschak 
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

namespace NETDataObjects.NDOVSPackage
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
