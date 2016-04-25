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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using VdProjTree;
using System.Text;

namespace CopyDeploymentFile
{
    class Pair
    {
        public Pair(int s, int l)
        {
            Start = s;
            Length = l;
        }
        public int Start;
        public int Length;
    }

    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("usage: CopyDeploymentFile inputFile editionString");
                return -1;
            }

            string inputFile = args[0];
            string editionString = args[1];
            string outputFile = inputFile.Replace("Enterprise", editionString);

            if (editionString == "Enterprise")
                return 0;

            if (!File.Exists(inputFile ))
            {
                Console.WriteLine("Can't find file " + inputFile);
                return -2;
            }

            string ndoPublicKey = null;
            switch (editionString)
            {
                case "Trial":
                    ndoPublicKey = "5c3414e1a08e5488";
                    break;
                case "Beta":
                    ndoPublicKey = "76240b3b84a37bf8";
                    break;
                case "Professional":
                    ndoPublicKey = "be348cfc83ad26d4";
                    break;
                case "Standard":
                    ndoPublicKey = "74d7d4ff7ba85711";
                    break;
                case "Community":
                    ndoPublicKey = "448c4b53a46b0d0f";
                    break;
            }

            
            try
            {
                StreamReader sr = new StreamReader(inputFile, Encoding.Default);
                string s = sr.ReadToEnd();
                sr.Close();
                
                s = s.Replace("Enterprise", editionString);

                Console.WriteLine("Constructing .vdproj file: " + outputFile);

                using(StreamWriter sw = new StreamWriter(outputFile, false, Encoding.Default))
                {
                    sw.Write(s);
                    sw.Close();
                }

                VdProjFile vdproj = new VdProjFile(outputFile);

                if (editionString == "Standard" || editionString == "Community")
                {
                    foreach (VdElementNode fnode in vdproj.Files.ElementNodes)
                    {
                        string sourcePath = fnode["SourcePath"];
                        if (sourcePath.IndexOf("ClassGenerator") > -1)
                        {
                            vdproj.Files.RemoveNode(fnode);
                            //                        Console.WriteLine("Removing File: " + fnode.MsiKey + " " + sourcePath);
                            RemoveHierNode(vdproj, fnode.MsiKey);
                        }
                    }
                    foreach (VdElementNode snode in vdproj.Shortcuts.ElementNodes)
                    {
                        if (snode.TrimmedAttribute("Name") == "Class Generator")
                        {
                            vdproj.Shortcuts.RemoveNode(snode);
                            //                        Console.WriteLine("Removing Shortcut: " + snode.MsiKey + " Target: " + snode["Target"]);
                            RemoveHierNode(vdproj, snode.MsiKey);
                        }
                    }
                }

                // Make sure, all Dependencies are excluded, with exception of the ClassGeneratorDll.dll
                foreach (VdElementNode fnode in vdproj.Files.ElementNodes)
                {
                    if (fnode.TrimmedAttribute("IsDependency") == "TRUE" &&
                        string.Compare(fnode.TrimmedAttribute("SourcePath"), "ClassGeneratorDll.DLL", true) != 0)
                    {
                        fnode["Exclude"] = fnode["Exclude"].Replace("FALSE", "TRUE");
                    }
                }


                string ndoVersion = null;
                foreach (VdElementNode fnode in vdproj.Files.ElementNodes)
                {
                    string asmVersion = fnode["AssemblyAsmDisplayName"];
                    if (asmVersion == null)
                        continue;
                    Regex regex = new Regex(@"NDO,\s*Version=(\d+.\d+.\d+.\d+),");
                    Match match = regex.Match(asmVersion);
                    if (match.Success)
                    {
                        if (ndoVersion == null)
                            ndoVersion = match.Groups[1].Value;
                        regex = new Regex(@"NDO,\s*Version=\d+.\d+.\d+.\d+,\s*Culture=neutral,\s*PublicKeyToken=[a-fA-F0-9]+");
                        fnode["AssemblyAsmDisplayName"] = regex.Replace(asmVersion, "NDO, Version=" + ndoVersion + ", Culture=neutral, PublicKeyToken=" + ndoPublicKey);
                    }
                }

                vdproj.Write(outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while copying a deployment file: " + ex.Message);
                return -3;
            }

            return 0;
        }


        static void RemoveHierNode(VdProjFile file, string key)
        {
            foreach (VdElementNode hnode in file.Hierarchy.ElementNodes)
            {
                if (hnode.TrimmedAttribute("MsmKey") == key ||
                    hnode.TrimmedAttribute("OwnerKey") == key)
                {
                    file.Hierarchy.RemoveNode(hnode);
//                    Console.WriteLine("  " + hnode["MsmKey"] + " " + hnode["OwnerKey"]);
                }
            }
        }

    }
}
