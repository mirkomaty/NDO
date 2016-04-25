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

namespace MakeHxTFlat
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("usage: MakeHxTFlat hxt-filename");
                return -1;
            }
            string fileName = args[0];
            if (!File.Exists(fileName))
            {
                Console.WriteLine("File doesn't exist: " + fileName);
                return -1;
            }
            StreamReader sr = new StreamReader(fileName, System.Text.ASCIIEncoding.Default);
            string content = sr.ReadToEnd();
            sr.Close();

            string orig = @"PluginStyle=""Hierarchical""";
            string repl = @"PluginStyle=""Flat""";

            if (content.IndexOf(orig) > -1)
            {
                content = content.Replace(orig, repl);
                StreamWriter sw = new StreamWriter(fileName, false, System.Text.ASCIIEncoding.Default);
                sw.Write(content);
                sw.Close();
            }
            return 0;
        }
    }
}
