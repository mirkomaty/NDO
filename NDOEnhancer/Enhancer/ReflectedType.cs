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
#if !NET11
using System.Collections.Generic;
#endif
using System.Text;

namespace NDOEnhancer
{
    public class ReflectedType
    {
        Type t;
        string ownAssemblyName = null;

		public ReflectedType(Type t) : this(t, null)
		{
		}

        public ReflectedType(Type t, string ownAssemblyName)
        {
            this.t = t;
            if (ownAssemblyName != null)
                this.ownAssemblyName = ownAssemblyName.Replace("'", string.Empty);
        }

		public bool IsStorable
		{
			// Die Logik ist auch in NDO -> FieldMap.cs, und im Mapping Tool enthalten.
			get					
			{
				return NDO.StorableTypes.Contains(t);
				//if (t == typeof(System.IntPtr))
				//    return false;
				//return t.IsPrimitive || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(Guid) || t.IsSubclassOf(typeof(System.Enum));
			}
		}

		string GetIlName(bool withPrefix, bool isQuoted)
		{
            if (t.IsGenericParameter)
                return "!" + t.Name;

			Type t2 = null;
			string arrBrackets = null;
			if (t.IsArray)
			{
				arrBrackets = "[]";
				t2 = t.GetElementType();
			}
			else
			{
				arrBrackets = string.Empty;
				t2 = t;
			}
			string tname = null;

			if (t2 == typeof(System.String))
				tname = "string";
			else if (t2 == typeof(System.Int32))
				tname = "int32";
			else if (t2 == typeof(System.Boolean))
				tname = "bool";
			else if (t2 == typeof(System.Byte))
				tname = "unsigned int8";
			else if (t2 == typeof(System.SByte))
				tname = "int8";
			else if (t2 == typeof(System.SByte))
				tname = "sbyte";
			else if (t2 == typeof(System.Char))
				tname = "char";
			else if (t2 == typeof(System.Int16))
				tname = "int16";
			else if (t2 == typeof(System.UInt16))
				tname = "unsigned int16";
			else if (t2 == typeof(System.UInt32))
				tname = "unsigned int32";
			else if (t2 == typeof(System.Int64))
				tname = "int64";
			else if (t2 == typeof(System.UInt64))
				tname = "unsigned int64";
			else if (t2 == typeof(System.Single))
				tname = "float32";
			else if (t2 == typeof(System.Double))
				tname = "float64";
			else if (t2 == typeof(object))
				tname = "object";
			else if (t2.FullName != null && t2.FullName == "System.Void")
				tname = "void";

			if (tname != null)
				return tname + arrBrackets;

			string vtPrefix = string.Empty;
			if (withPrefix)
				vtPrefix = t2.IsValueType ? "valuetype " : "class ";
			string assPrefix = string.Empty;
            if (t2.Assembly.GetName().Name != ownAssemblyName)
            {
                string assemblyName = t.Assembly.GetName().Name;
                if (isQuoted)
                    assemblyName = QuotedName.Convert(assemblyName);
                assPrefix = "[" + assemblyName + "]";
            }
            string tn = t2.FullName;
#if !NET11
            string genericArgs = string.Empty;
            if (t2.IsGenericType)
            {
                tn = t2.GetGenericTypeDefinition().FullName;
                StringBuilder sb = new StringBuilder("<");
                Type[] args = t2.GetGenericArguments();
                for (int i = 0; i < args.Length; i++)
                {
                    Type arg = args[i];
                    if (arg.FullName == null) // Type definition
                    {
                        sb.Append(arg.Name);
                    }
                    else
                    {
                        if (!isQuoted)
                            sb.Append(new ReflectedType(arg, ownAssemblyName).ILName);
                        else
                            sb.Append(new ReflectedType(arg, ownAssemblyName).QuotedILName);
                    }
                    if (i < args.Length - 1)
                        sb.Append(',');
                }
                sb.Append('>');
                int p = tn.IndexOf("[[");
                if (p > -1)
                    tn = tn.Substring(0, p);
                genericArgs = sb.ToString();
            }

            if (tn == null)
                throw new Exception("FullName of Type '" + t2.Name + "' is null.");
            tn = tn.Replace('+', '/');

            if (isQuoted)
                tn = QuotedName.ConvertTypename(tn);
            return vtPrefix + assPrefix + tn + genericArgs + arrBrackets;
#else
            if (isQuoted)
                tn = UmlautName.Convert(tn);
			return vtPrefix + assPrefix + tn;
#endif
        }
        public string ILName
        {
            get { return GetIlName(true, false); }
        }
		public string ILNameWithoutPrefix
		{
			get { return GetIlName(false, false); }
		}
        public string QuotedILName
        {
            get { return GetIlName(true, true); }
        }
        public string QuotedILNameWithoutPrefix
        {
            get { return GetIlName(false, true); }
        }
    }
}

