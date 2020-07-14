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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CodeGenerator
{
	/// <summary>
	/// Renders classes.
	/// </summary>
	public class Class
	{
		string name;
		Class baseClass;
		bool isAbstract;
		private readonly string nameSpace;

		List<string> statements = new List<string>();
		List<string> attributes = new List<string>();

		public Class(bool isAbstract, string nameSpace, string name, Class baseClass)
		{
			this.name = name;
            this.baseClass = baseClass;
			this.isAbstract = isAbstract;
			this.nameSpace = nameSpace;
		}

		public Class(string nameSpace, string name) : this(false, nameSpace, name, null)
		{
		}

		public List<string> Attributes
		{
			get { return attributes; }
		}


		List<Property> properties = new List<Property>();
		public List<Property> Properties
		{
			get { return properties; }
		}
		List<Function> functions = new List<Function>();
		public List<Function> Functions
		{
			get { return functions; }
		}
		public List<string> Statements
		{
			get { return statements; }
		}

        ArrayList fields = new ArrayList();
        public ArrayList Fields
        {
            get { return fields; }
        }

        List<string> genericParameters = new List<string>();
        public List<string> GenericParameters
        {
            get { return genericParameters; }
        }


		protected virtual void AddStringCollection(StringBuilder sb, List<string> sc)
		{
			foreach(string s in sc)
			{
				sb.Append('\t');
				sb.Append(s);
				sb.Append('\n');
			}
		}

		protected virtual void GenerateStatements(StringBuilder sb)
		{
			AddStringCollection(sb, statements);
		}

		protected virtual void GenerateProperties(StringBuilder sb)
		{
			foreach(Property p in properties)
				AddStringCollection(sb, p.Text);
		}

		protected virtual void GenerateFunctions(StringBuilder sb)
		{
			foreach (Function f in functions)
			{
				if (f.Statements.Count > 0)
				AddStringCollection( sb, f.Text );
			}
		}

        protected virtual void GenerateFields(StringBuilder sb)
        {
            foreach (Field f in fields)
                AddStringCollection(sb, f.Text);
        }

		protected virtual void GenerateFooter(StringBuilder sb)
		{
			sb.Append("}\n\n");
		}

		public override string  ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (attributes.Count > 0)
			{
				sb.Append('[');
				for (int i = 0; i < attributes.Count; i++)
				{
					sb.Append(attributes[i]);
					if (i < attributes.Count - 1)
						sb.Append(", ");
				}
				sb.Append("]\n");
			}
			sb.Append(this.GenerateHeader(this.isAbstract, this.name, this.baseClass));
			GenerateStatements(sb);
            GenerateFields(sb);
			GenerateProperties(sb);
			GenerateFunctions(sb);
			GenerateFooter(sb);
			return sb.ToString();
		}

		public string Name => name;

		public string FullName => nameSpace + '.' + name;


		public bool IsAbstract
		{
			get { return isAbstract; }
		}
        protected virtual string GenerateGenericArgs()
        {
            int genParCount = this.genericParameters.Count;
            if (genParCount == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            if (genParCount > 0)
            {
                sb.Append('<');
                for (int i = 0; i < genParCount; i++)
                {
                    sb.Append(this.genericParameters[i]);
                    if (i < genParCount - 1)
                        sb.Append(',');
                }
                sb.Append('>');
            }
            return sb.ToString();
        }

		protected virtual string GenerateHeader(bool isAbstract, string name, Class baseClass)
		{
			StringBuilder sb = new StringBuilder("public ");
			if (isAbstract)
				sb.Append("abstract ");
			sb.Append("class ");
			sb.Append(name);
            sb.Append(GenerateGenericArgs());
			if (baseClass != null)
			{
				sb.Append(" : ");
				sb.Append(baseClass.name + baseClass.GenerateGenericArgs());
			}
			sb.Append("\n{\n");
			return sb.ToString();
		}

		public Function NewFunction(string resultType, string name)
		{
			Function f = new Function(resultType, name);
			this.functions.Add(f);
			return f;
		}

		public Function NewFunction(string resultType, string name, string[] paramTypes, string[] paramNames)
		{
			Function f = new Function(resultType, name, paramTypes, paramNames);
			this.functions.Add(f);
			return f;
		}


		public void AddVarAndProperty(string type, string varName)
		{
			this.fields.Add(new Field(type, varName));
			this.properties.Add(new Property(type, varName, true, true));
		}

		public void AddField(string type, string varName)
		{
			this.fields.Add( new Field( type, varName ) );
		}

	}
}
