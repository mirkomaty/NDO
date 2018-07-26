using System.Collections.Generic;
using System.Text;
using System.Globalization; 
using NDOql.Expressions; 



using System;
#pragma warning disable 3008

namespace NDOql {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _realCon = 2;
	public const int _intCon = 3;
	public const int _stringCon = 4;
	public const int _AND = 5;
	public const int _OR = 6;
	public const int _NOT = 7;
	public const int _LIKE = 8;
	public const int _ESCAPE = 9;
	public const int _BETWEEN = 10;
	public const int _IS = 11;
	public const int _NULL = 12;
	public const int _TRUE = 13;
	public const int _MOD = 14;
	public const int _IN = 15;
	public const int _parameter = 16;
	public const int maxT = 39;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

OqlExpression rootExpression;
public OqlExpression RootExpression
{
	get { return rootExpression; }
}



/*------------------------------------------------------------------------*
 *----- SCANNER DESCRIPTION ----------------------------------------------*
 *------------------------------------------------------------------------*/



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void NDOQL() {
		RootExpr(out this.rootExpression);
	}

	void RootExpr(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		bool negated = false; 
		
		if (la.kind == 7) {
			Get();
			negated = true; 
		}
		OrExpr(out child);
		if (negated) result.UnaryOp = "NOT"; 
		result.Add(child); 
		expression = result.Simplify(); 
		
	}

	void OrExpr(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		
		AndExpr(out child);
		result.Add(child); 
		while (la.kind == 6) {
			Get();
			bool negated = false; 
			if (la.kind == 7) {
				Get();
				negated = true; 
			}
			AndExpr(out child);
			if (negated) child.UnaryOp = "NOT"; result.Add(child, "OR"); 
		}
		expression = result.Simplify(); 
	}

	void AndExpr(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		
		EqlExpr(out child);
		result.Add(child); 
		while (la.kind == 5) {
			Get();
			bool negated = false; 
			if (la.kind == 7) {
				Get();
				negated = true; 
			}
			EqlExpr(out child);
			if (negated) child.UnaryOp = "NOT"; result.Add(child, "AND"); 
		}
		expression = result.Simplify(); 
	}

	void EqlExpr(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		string newOp = null;
		bool negated = false;
		
		if (!(scanner.Peek().kind==_IN)) {
			RelExpr(out child);
			result.Add(child); 
			while (StartOf(1)) {
				if (la.kind == 17) {
					Get();
					newOp = "<>"; 
				} else if (la.kind == 18) {
					Get();
					newOp = "<>"; 
				} else if (la.kind == 19) {
					Get();
					newOp = "="; 
					if (la.kind == 7) {
						Get();
						negated = true; 
					}
				} else {
					Get();
					newOp = "LIKE"; 
				}
				RelExpr(out child);
				if (negated) child.UnaryOp = "NOT"; result.Add(child, newOp); 
			}
		} else if (la.kind == 1) {
			Identifier(out child);
			result.Add(child); 
			Expect(15);
			Expect(20);
			if (la.kind == 2 || la.kind == 3) {
				NumList(out child);
				child.HasBrackets = true; result.Add(child, "IN"); 
			} else if (la.kind == 4) {
				StringList(out child);
				child.HasBrackets = true; result.Add(child, "IN"); 
			} else SynErr(40);
			Expect(21);
		} else SynErr(41);
		expression = result.Simplify(); 
	}

	void RelExpr(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		string newOp = null;
		bool negated = false;
		
		BitOrExpr(out child);
		result.Add(child); 
		if (StartOf(2)) {
			if (StartOf(3)) {
				switch (la.kind) {
				case 22: {
					Get();
					newOp = "<"; 
					break;
				}
				case 23: {
					Get();
					newOp = ">"; 
					break;
				}
				case 24: {
					Get();
					newOp = "<="; 
					break;
				}
				case 25: {
					Get();
					newOp = "<="; 
					break;
				}
				case 26: {
					Get();
					newOp = ">="; 
					break;
				}
				case 27: {
					Get();
					newOp = "<="; 
					break;
				}
				}
				BitOrExpr(out child);
				result.Add(child, newOp); 
			} else if (la.kind == 11) {
				Get();
				if (la.kind == 7) {
					Get();
					negated = true; 
				}
				Expect(12);
				result.Add(new NamedConstantExpression("NULL", negated, t.line, t.col), "IS"); 
			} else {
				if (la.kind == 7) {
					Get();
					negated = true; 
				}
				Expect(10);
				BitOrExpr(out child);
				result.Add(child, "BETWEEN"); 
				Expect(5);
				BitOrExpr(out child);
				result.Add(child, "BETWEEN"); 
			}
		}
		expression = result.Simplify(); 
	}

	void Identifier(out OqlExpression expression) {
		OqlExpression result = null;
		
		Expect(1);
		result = new IdentifierExpression(t.val, t.line, t.col); 
		expression = result.Simplify(); 
	}

	void NumList(out OqlExpression expression) {
		OqlExpression result = new OqlExpression(la.line, la.col);
		
		if (la.kind == 2) {
			Get();
			result.Add(new NumberExpression(double.Parse(t.val, CultureInfo.InvariantCulture ), t.line, t.col)); 
		} else if (la.kind == 3) {
			Get();
			result.Add(new NumberExpression(int.Parse(t.val), t.line, t.col)); 
		} else SynErr(42);
		while (la.kind == 37) {
			Get();
			if (la.kind == 2) {
				Get();
				result.Add(new NumberExpression(double.Parse(t.val, CultureInfo.InvariantCulture ), t.line, t.col), ","); 
			} else if (la.kind == 3) {
				Get();
				result.Add(new NumberExpression(int.Parse(t.val), t.line, t.col), ","); 
			} else SynErr(43);
		}
		expression = result.Simplify(); 
	}

	void StringList(out OqlExpression expression) {
		OqlExpression result = new OqlExpression(la.line, la.col);
		
		Expect(4);
		result.Add(new StringLiteralExpression(t.val, t.line, t.col)); 
		while (la.kind == 37) {
			Get();
			Expect(4);
			result.Add(new StringLiteralExpression(t.val, t.line, t.col), ","); 
		}
		expression = result.Simplify(); 
	}

	void BitOrExpr(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		
		BitXorExpr(out child);
		result.Add(child); 
		while (la.kind == 28) {
			Get();
			BitXorExpr(out child);
			result.Add(child, "|"); 
		}
		expression = result.Simplify(); 
	}

	void BitXorExpr(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		
		BitAndExpr(out child);
		result.Add(child); 
		while (la.kind == 29) {
			Get();
			BitAndExpr(out child);
			result.Add(child, "^"); 
		}
		expression = result.Simplify(); 
	}

	void BitAndExpr(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		
		AddExpr(out child);
		result.Add(child); 
		while (la.kind == 30) {
			Get();
			AddExpr(out child);
			result.Add(child, "&"); 
		}
		expression = result.Simplify(); 
	}

	void AddExpr(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		string newOp = null;
		
		MulExpr(out child);
		result.Add(child); 
		while (la.kind == 31 || la.kind == 32) {
			if (la.kind == 31) {
				Get();
				newOp = "+"; 
			} else {
				Get();
				newOp = "-"; 
			}
			MulExpr(out child);
			result.Add(child, newOp); 
		}
		expression = result.Simplify(); 
	}

	void MulExpr(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		string newOp = null;
		
		Unary(out child);
		result.Add(child); 
		while (StartOf(4)) {
			if (la.kind == 33) {
				Get();
				newOp = "*"; 
			} else if (la.kind == 34) {
				Get();
				newOp = "/"; 
			} else if (la.kind == 35) {
				Get();
				newOp = "%"; 
			} else {
				Get();
				newOp = "MOD"; 
			}
			Unary(out child);
			result.Add(child, newOp); 
		}
		expression = result.Simplify(); 
	}

	void Unary(out OqlExpression expression) {
		OqlExpression child;
		OqlExpression result = new OqlExpression(la.line, la.col);
		string newOp = null;
		
		if (la.kind == 31 || la.kind == 32 || la.kind == 36) {
			if (la.kind == 31) {
				Get();
				newOp = "+"; 
			} else if (la.kind == 32) {
				Get();
				newOp = "-"; 
			} else {
				Get();
				newOp = "~"; 
			}
		}
		Primary(out child);
		result.Add(child); child.UnaryOp = newOp; 
		expression = result.Simplify(); 
	}

	void Primary(out OqlExpression expression) {
		OqlExpression result = null;
		
		if (la.kind == 1) {
			Get();
			result = new IdentifierExpression(t.val, t.line, t.col); 
		} else if (StartOf(5)) {
			Literal(out result);
		} else if (la.kind == 16) {
			Get();
			result = new ParameterExpression(t.val, t.line, t.col); 
		} else if (la.kind == 20) {
			Get();
			RootExpr(out result);
			result.HasBrackets = true; 
			Expect(21);
		} else SynErr(44);
		expression = result.Simplify(); 
	}

	void Literal(out OqlExpression expression) {
		OqlExpression result = null;
		
		switch (la.kind) {
		case 2: {
			Get();
			result = new NumberExpression(double.Parse(t.val, CultureInfo.InvariantCulture ), t.line, t.col); 
			break;
		}
		case 3: {
			Get();
			result = new NumberExpression(int.Parse(t.val), t.line, t.col); 
			break;
		}
		case 4: {
			Get();
			result = new StringLiteralExpression(t.val, t.line, t.col); 
			break;
		}
		case 13: {
			Get();
			result = new NamedConstantExpression("TRUE", t.line, t.col); 
			break;
		}
		case 38: {
			Get();
			result = new NamedConstantExpression("FALSE", t.line, t.col); 
			break;
		}
		case 12: {
			Get();
			result = new NamedConstantExpression("NULL", t.line, t.col); 
			break;
		}
		default: SynErr(45); break;
		}
		expression = result.Simplify(); 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		NDOQL();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x},
		{x,x,T,T, T,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "realCon expected"; break;
			case 3: s = "intCon expected"; break;
			case 4: s = "stringCon expected"; break;
			case 5: s = "AND expected"; break;
			case 6: s = "OR expected"; break;
			case 7: s = "NOT expected"; break;
			case 8: s = "LIKE expected"; break;
			case 9: s = "ESCAPE expected"; break;
			case 10: s = "BETWEEN expected"; break;
			case 11: s = "IS expected"; break;
			case 12: s = "NULL expected"; break;
			case 13: s = "TRUE expected"; break;
			case 14: s = "MOD expected"; break;
			case 15: s = "IN expected"; break;
			case 16: s = "parameter expected"; break;
			case 17: s = "\"!=\" expected"; break;
			case 18: s = "\"<>\" expected"; break;
			case 19: s = "\"=\" expected"; break;
			case 20: s = "\"(\" expected"; break;
			case 21: s = "\")\" expected"; break;
			case 22: s = "\"<\" expected"; break;
			case 23: s = "\">\" expected"; break;
			case 24: s = "\"<=\" expected"; break;
			case 25: s = "\">=\" expected"; break;
			case 26: s = "\"!<\" expected"; break;
			case 27: s = "\"!>\" expected"; break;
			case 28: s = "\"|\" expected"; break;
			case 29: s = "\"^\" expected"; break;
			case 30: s = "\"&\" expected"; break;
			case 31: s = "\"+\" expected"; break;
			case 32: s = "\"-\" expected"; break;
			case 33: s = "\"*\" expected"; break;
			case 34: s = "\"/\" expected"; break;
			case 35: s = "\"%\" expected"; break;
			case 36: s = "\"~\" expected"; break;
			case 37: s = "\",\" expected"; break;
			case 38: s = "\"false\" expected"; break;
			case 39: s = "??? expected"; break;
			case 40: s = "invalid EqlExpr"; break;
			case 41: s = "invalid EqlExpr"; break;
			case 42: s = "invalid NumList"; break;
			case 43: s = "invalid NumList"; break;
			case 44: s = "invalid Primary"; break;
			case 45: s = "invalid Literal"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}