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
#if !NET11
using System.Collections.Generic;
#endif
using System.Text;
using System.Xml;

namespace ClassGenerator
{
    class NdoProject
    {
        string fileName;
        XmlDocument doc;
        public NdoProject(string fileName, string binPath, string objPath, string projPath)
        {
            this.fileName = fileName;
            doc = new XmlDocument();
            doc.LoadXml(template);
            XmlNode node = doc.SelectSingleNode("Enhancer/ProjectDescription/BinPath");
            node.InnerText = binPath;
            node = doc.SelectSingleNode("Enhancer/ProjectDescription/ObjPath");
            node.InnerText = objPath;
            node = doc.SelectSingleNode("Enhancer/ProjectDescription/ProjPath");
            node.InnerText = projPath;
        }

        public void Save()
        {
            doc.Save(fileName);
        }
        const string template = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<Enhancer>
  <Options>
    <EnableAddIn>True</EnableAddIn>
    <EnableEnhancer>True</EnableEnhancer>
    <NewMapping>False</NewMapping>
    <GenerateSQL>False</GenerateSQL>
    <DefaultConnection>
    </DefaultConnection>
    <GenerateChangeEvents>False</GenerateChangeEvents>
    <UseTimeStamps>False</UseTimeStamps>
    <Utf8Encoding>True</Utf8Encoding>
    <SQLScriptLanguage>SqlServer</SQLScriptLanguage>
    <SchemaVersion>1.0</SchemaVersion>
    <IncludeTypecodes>False</IncludeTypecodes>
    <DatabaseOwner>
    </DatabaseOwner>
    <GenerateConstraints>False</GenerateConstraints>
    <DropExistingElements>True</DropExistingElements>
  </Options>
  <ProjectDescription>
    <BinPath></BinPath>
    <ObjPath></ObjPath>
    <ProjPath></ProjPath>
    <AssemblyName />
    <Debug>True</Debug>
    <References>
    </References>
  </ProjectDescription>
</Enhancer>
        ";
    }
}
