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
using System.Text.RegularExpressions;
using System.Collections;


namespace NDO
{

	/// <summary>
	/// Tokens are results of the scanning process
	/// </summary>
	internal class Token
	{
		/// <summary>
		/// Token type
		/// </summary>
		internal enum Type
		{
			OpNot,
			OpAnd,
			OpOr,
			OpLike,
			OpIs,
			OpNull,
			OpEquals,
			OpGt,
			OpLt,
			OpGe,
			OpLe,
			OpNe,
			OpTrue,
			OpBetween,
			OpLBracket,
			OpEscape,
			OpRBracket,
			OpMult,
			OpDiv,
			OpAdd,
			OpSub,
			OpBitAnd,
			OpBitOr,
			OpBitXor,
			OpBitNot,
			OpMod,
			OpIn,
			Name,
			Number,
			StringLiteral,
			Parameter,
			FileTime,
			FileTimeUtc,
			Eof
		}

		public bool IsOperator
		{
			get
			{
				return type >= Type.OpNot && type <= Type.OpIn;
			}
		}


		protected object _value;

		protected Type type;

		/// <summary>
		/// Type of the token data
		/// </summary>
		internal Type TokenType
		{
			get { return type; }
		}

		/// <summary>
		/// Token value - this is used to construct the where clause of a SQL statement
		/// </summary>
		internal object Value
		{
			get { return _value; }
		}

		/// <summary>
		/// Used for debugging purposes
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.GetType().Name + " " + type.ToString() + " " + _value.ToString();
		}

		
	}

	/// <summary>
	/// FileTime token class
	/// </summary>
	internal class FileTime : Token
	{
		private FileTime (Match match, string content, ref int position, Token.Type tokenType)
		{
			string result = match.ToString();
			Regex re = new Regex(@"\d+");
			Match match2 = re.Match(result);
			if (!match2.Success)
				throw new QueryException(10008, "FileTime or FileTimeUtc without value");
			_value = long.Parse(match2.ToString());
			position += match.ToString().Length;
			type = tokenType;
		}

		internal static FileTime Get (string content, ref int position)
		{
			Regex re = new Regex(@"FileTime\s*\(\s*\d+\s*\)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			Match match = re.Match(content);
			if (match.Success && match.Index == 0)
			{
				return new FileTime(match, content, ref position, Token.Type.FileTime);
			}
			re = new Regex(@"FileTimeUtc\s*\(\s*\d+\s*\)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			match = re.Match(content);
			if (match.Success && match.Index == 0)
			{
				return new FileTime(match, content, ref position, Token.Type.FileTimeUtc);
			}
			return null;
		}			 
	}

	/// <summary>
	/// Number token class
	/// </summary>
	internal class Number : Token
	{
		private Number (Match match, string content, ref int position)
		{
			string result = match.ToString();
			_value = result;
			position += result.Length;
			type = Token.Type.Number;
		}

		internal static Number Get (string content, ref int position)
		{
			Regex re = new Regex(@"(-|)(\d+|)(\.|)(0x[\da-fA-F]+|\d+)", RegexOptions.Singleline);
			Match match = re.Match(content);
			if (match.Success && match.Index == 0)
			{
				return new Number(match, content, ref position);
			}
			return null;
		}			 
	}
	
	/// <summary>
	/// String literal token class
	/// </summary>
	internal class StringLiteral : Token
	{
		private StringLiteral (Match match, string content, ref int position)
		{
			_value = match.ToString();
			position += ((string)_value).Length;
			type = Token.Type.StringLiteral;
		}

		internal static StringLiteral Get (string content, ref int position)
		{
//			Regex re = new Regex(@"'[^']*'|""[^""]*""|#[^#]+#|\{[^\{^\}]+\}", RegexOptions.Singleline);
			Regex re = new Regex( @"'[^']*'|""[^""]*""|#[^#]+#", RegexOptions.Singleline );
			Match match = re.Match( content );
			if (match.Success && match.Index == 0)
			{
				return new StringLiteral(match, content, ref position);
			}
			return null;
		}			 
	}


	/// <summary>
	/// Name token class - this is the token which represents field names
	/// </summary>
	internal class Name : Token
	{
		private Name (Match match, string content, ref int position)
		{
			string m = match.ToString();
			position += m.Length;
			if (m.StartsWith("["))
				m = m .Substring(1);
			if (m.EndsWith("]"))
				m = m .Substring(0, m.Length - 1);
			_value = m;
			type = Token.Type.Name;
		}

		internal static Name Get (string content, ref int position)
		{
			Regex re = new Regex(@"[A-Za-z_öäüßÖÄÜ\[\]][A-Za-z0-9öäüÖÄÜß_\.\[\]]*", RegexOptions.Singleline);
			Match match = re.Match(content);
			if (match.Success && match.Index == 0)
			{
                if (match.Value == "oid")
                {
                    re = new Regex(@"oid(\s*\(\s*\d+\s*\)|)");
                    match = re.Match(content);
                }
				return new Name(match, content, ref position);
			}
			return null;
		}			 
	}

	/// <summary>
	/// Parameter token type - parameters are written {x} where x is a number from 0..n
	/// </summary>
	internal class ScParameter : Token
	{
		int index;
		public int Index
		{
			get { return index; }
		}
		private ScParameter (Match match, string content, ref int position)
		{
			this._value = match.ToString();
			string strIndex = match.ToString().Replace( "{", string.Empty ).Replace( "}", string.Empty );
			this.index = -1;
			int.TryParse( strIndex, out this.index );
			position += ((string)_value).Length;
			this.type = Token.Type.Parameter;
		}

		internal static ScParameter Get (string content, ref int position)
		{
			Regex re = new Regex(@"\{\d\}", RegexOptions.Singleline);
			Match match = re.Match(content);
			if (match.Success && match.Index == 0)
			{
				return new ScParameter(match, content, ref position);
			}
			return null;
		}			 
	}

	/// <summary>
	/// IN Parameter list
	/// </summary>
	internal class InClause : Token
	{

		private InClause (Match match, string content, ref int position)
		{
			_value = match.ToString();
			position += ((string)_value).Length;
			type = Token.Type.OpIn;
		}


		internal static InClause Get (string content, ref int position)
		{
			Regex re = new Regex(@"IN\s*\(.*\)", RegexOptions.IgnoreCase);
			Match match = re.Match(content);
			if (match.Success && match.Index == 0)
			{
				return new InClause(match, content, ref position);
			}
			return null;
		}			 
	}


	/// <summary>
	/// Keywords used in NDOql
	/// </summary>
	internal class Keyword : Token
	{
		static Hashtable tokens = null;

		static Keyword()
		{
			tokens = new Hashtable();
			tokens.Add("AND", Token.Type.OpAnd);
			tokens.Add("OR", Token.Type.OpOr);
			tokens.Add("IS", Token.Type.OpIs);
			tokens.Add("LIKE", Token.Type.OpLike);
			tokens.Add("NULL", Token.Type.OpNull);
			tokens.Add("NOT", Token.Type.OpNot);
			tokens.Add("TRUE", Token.Type.OpTrue);
			tokens.Add("BETWEEN", Token.Type.OpBetween);
			tokens.Add("ESCAPE", Token.Type.OpEscape);
		}

		private Keyword (string val, Token.Type tokenType)
		{
			_value = val;
			type = tokenType;
		}

		internal static Keyword Get (string content, ref int position)
		{
			string upperContent = content.ToUpper();
			foreach (DictionaryEntry e in tokens)
			{
				string val = (string)e.Key;
				if (upperContent.StartsWith(val))
				{
					if (val.Length == upperContent.Length ||
						!(char.IsLetterOrDigit(upperContent[val.Length]) || 
						 (upperContent[val.Length] == '_')))
					{
						position += val.Length;
						return new Keyword(val, (Token.Type) e.Value);
					}
				}
			}
			return null;
		}
	}

	/// <summary>
	/// Operators used in NDOql
	/// </summary>
	internal class Operator : Token
	{
		static ArrayList tokens = null;
		static ArrayList strings = null;
		static object lockObject = new object();
		private static void InitTokens()
		{
			lock (lockObject)
			{
				if (tokens != null)
					return;
				tokens = new ArrayList();
				strings = new ArrayList();
				tokens.Add( Token.Type.OpGe ); strings.Add( ">=" );
				tokens.Add( Token.Type.OpGe ); strings.Add( "!<" );
				tokens.Add( Token.Type.OpLe ); strings.Add( "<=" );
				tokens.Add( Token.Type.OpLe ); strings.Add( "!>" );
				tokens.Add( Token.Type.OpNe ); strings.Add( "<>" );
				tokens.Add( Token.Type.OpGt ); strings.Add( ">" );
				tokens.Add( Token.Type.OpLt ); strings.Add( "<" );
				tokens.Add( Token.Type.OpEquals ); strings.Add( "=" );
				tokens.Add( Token.Type.OpNe ); strings.Add( "!=" );
				tokens.Add( Token.Type.OpLBracket ); strings.Add( "(" );
				tokens.Add( Token.Type.OpRBracket ); strings.Add( ")" );
				tokens.Add( Token.Type.OpMult ); strings.Add( "*" );
				tokens.Add( Token.Type.OpDiv ); strings.Add( "/" );
				tokens.Add( Token.Type.OpAdd ); strings.Add( "+" );
				tokens.Add( Token.Type.OpSub ); strings.Add( "-" );
				tokens.Add( Token.Type.OpBitOr ); strings.Add( "|" );
				tokens.Add( Token.Type.OpBitAnd ); strings.Add( "&" );
				tokens.Add( Token.Type.OpBitXor ); strings.Add( "^" );
				tokens.Add( Token.Type.OpBitNot ); strings.Add( "~" );
				tokens.Add( Token.Type.OpMod ); strings.Add( "%" );
				tokens.Add( Token.Type.OpMod ); strings.Add( "MOD" );
			}
        }

		private Operator (string content, ref int position)
		{
			if (tokens == null) InitTokens();

			for (int i = 0; i < strings.Count; i++)
			{
				string s = (string) strings[i];
				if (content.StartsWith(s))
				{
					_value = s;
					type = (Token.Type) tokens[i];
					position += s.Length;
					break;
				}
			}
		}

		internal static Operator Get (string content, ref int position)
		{
			if (tokens == null) InitTokens();

			char c = content[0];
			foreach(string s in strings)
				if (s[0] == c)
					return new Operator(content, ref position);
			return null;
		}
	}

	
	/// <summary>
	/// NDOql Scanner class. This class is used to construct SQL statements.
	/// </summary>
	internal class Scanner
	{
		int position;
		string content;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="c">The string to be scanned</param>
		internal Scanner(string c)
		{
			content = c;
		}

		void SkipBlank()
		{
			while (position < content.Length && char.IsWhiteSpace(content[position]))
				position++;
		}

		/// <summary>
		/// Retrieve the next token of the token stream
		/// </summary>
		/// <returns>An object of type Token</returns>
		internal Token NextToken()
		{
			SkipBlank();
			if (position == content.Length) return null;
			Token t;
			if ((t = Number.Get(content.Substring(position), ref position)) != null) return t;
			if ((t = FileTime.Get(content.Substring(position), ref position)) != null) return t;
			if ((t = StringLiteral.Get(content.Substring(position), ref position)) != null) return t;
			if ((t = Keyword.Get(content.Substring(position), ref position)) != null) return t;
			if ((t = InClause.Get(content.Substring(position), ref position)) != null) return t;
			if ((t = Name.Get(content.Substring(position), ref position)) != null) return t;
			if ((t = Operator.Get(content.Substring(position), ref position)) != null) return t;	
			if ((t = ScParameter.Get(content.Substring(position), ref position)) != null) return t;	
			throw new Exception("Ungültiger Code beim Scannen eines Abfragestrings. Position: " + position.ToString() + " String ab Position: " + content.Substring(position));
		}
	}
}
