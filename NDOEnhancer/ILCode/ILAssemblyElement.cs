//
// Copyright (c) 2002-2022 Mirko Matytschak 
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
using System.Text.RegularExpressions;
using NDOEnhancer.Ecma335;

namespace NDOEnhancer.ILCode
{
    /// <summary>
    /// Summary description for ILAssemblyElement.
    /// </summary>
    internal class ILAssemblyElement : ILElement
    {
        public ILAssemblyElement( string firstLine, ILElement owner )
            : base( firstLine, owner )
        {
        }

        internal class ILAssemblyElementType : ILElementType
        {
            public ILAssemblyElementType()
                : base( ".assembly", typeof( ILAssemblyElement ) )
            {
            }
        }

        private static ILElementType m_elementType = new ILAssemblyElementType();

        private string m_name;
        private bool m_extern;
        private string versionString;
        private int major;
        private int minor;
        private int build;
        private int revision;

        public string VersionString
        {
            get { return versionString; }
            set { versionString = value; }
        }
        public int Major
        {
            get { return major; }
            set { major = value; }
        }
        public int Minor
        {
            get { return minor; }
            set { minor = value; }
        }
        public int Build
        {
            get { return build; }
            set { build = value; }
        }
        public int Revision
        {
            get { return revision; }
            set { revision = value; }
        }


        private void
        Resolve()
        {
            /*
             .assembly extern System.Data { .publickeytoken = (B7 7A 5C 56 19 34 E0 89 ) .ver 2:0:0:0 }
             * */
            if (null != m_name)
                return;

            Regex regex = new Regex(@"\.ver\s([\d:]+)");
            Match match;

            foreach (var el in Elements)
            {
                match = regex.Match( el.GetAllLines() );
                if (match.Success)
                {
                    versionString = match.Groups[1].Value;
                    regex = new Regex( @"(\d+):(\d+):(\d+):(\d+)" );
                    match = regex.Match( versionString );

                    major = int.Parse( match.Groups[1].Value );
                    minor = int.Parse( match.Groups[2].Value );
                    build = int.Parse( match.Groups[3].Value );
                    revision = int.Parse( match.Groups[4].Value );
                }
            }

            string line = this.GetAllLines();

            regex = new Regex( @"\.assembly\s+(extern|)" );
            match = regex.Match( line );

            if (match.Groups[1].Value != string.Empty)
                m_extern = true;

            string s = line.Substring(match.Length).Trim();
            EcmaDottedName dn = new EcmaDottedName();
            dn.Parse( s );
            this.m_name = dn.Content;
        }

        protected override void
        Unresolve()
        {
            m_name = null;
        }

        public string
        Name
        {
            get
            {
                Resolve();
                return m_name;
            }
        }

        public bool
        IsExtern
        {
            get
            {
                Resolve();
                return m_extern;
            }
        }
	}
}
    