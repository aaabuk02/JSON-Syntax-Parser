using Final;
using System.Collections.Generic;
using ASTClass;
using Sy;



using System;



public class Parser {
	public const int _EOF = 0;
	public const int _number = 1;
	public const int _string = 2;
	public const int _bool = 3;
	public const int _null = 4;
	public const int maxT = 11;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public AST root;
	//public SymbolTable tab;



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

	
	void DATA() {
		root = new AST(null,"DATA"); 
		TerminalAST terminal = null;
		JSON(root);
	}

	void JSON(AST parent) {
		AST nt = new AST(parent, "JSON"); 
		TerminalAST terminal = null; 
		if (la.kind == 5) {
			Get();
			terminal = new TerminalAST(nt, "", t.val); 
			if (la.kind == 2) {
				Object(nt);
			}
			Expect(6);
			terminal = new TerminalAST(nt, "", t.val); 
		} else if (StartOf(1)) {
			Value(nt);
		} else SynErr(12);
		End();
	}

	void Object(AST parent) {
		AST nt = new AST(parent, "Object"); 
		TerminalAST terminal = null;
		Expect(2);
		StringAST str = new StringAST(nt);
		str.SetValue (t.val); 
		Expect(7);
		terminal = new TerminalAST(nt, "", t.val); 
		Value(nt);
		while (la.kind == 8) {
			Get();
			terminal = new TerminalAST(nt, "", ","); 
			Object(nt);
		}
	}

	void Value(AST parent) {
		AST nt = new AST(parent, "Value"); 
		TerminalAST terminal = null;
		switch (la.kind) {
		case 1: {
			Get();
			NumberAST number = new NumberAST(nt);
			number.Value = t.val;				
			break;
		}
		case 2: {
			Get();
			StringAST str = new StringAST(nt);
			str.SetValue (t.val); 
			break;
		}
		case 3: {
			Get();
			BoolAST b = new BoolAST(nt);
			b.Value = (t.val); 
			break;
		}
		case 4: {
			Get();
			terminal = new TerminalAST(nt, "", "null"); 
			break;
		}
		case 9: {
			Arr(nt);
			break;
		}
		case 5: {
			Get();
			terminal = new TerminalAST(nt, "", t.val); 
			if (la.kind == 2) {
				Object(nt);
			}
			Expect(6);
			terminal = new TerminalAST(nt, "", t.val); 
			break;
		}
		default: SynErr(13); break;
		}
	}

	void End() {
	}

	void Arr(AST parent) {
		AST nt = new AST(parent, "Arr"); 
		TerminalAST terminal = null;
		Expect(9);
		terminal = new TerminalAST(nt, "", t.val); 
		if (StartOf(1)) {
			Value(nt);
			while (la.kind == 8) {
				Get();
				terminal = new TerminalAST(nt, "", ","); 
				Value(nt);
			}
		}
		Expect(10);
		terminal = new TerminalAST(nt, "", t.val); 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		DATA();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
		{_x,_T,_T,_T, _T,_T,_x,_x, _x,_T,_x,_x, _x}

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
			case 1: s = "number expected"; break;
			case 2: s = "string expected"; break;
			case 3: s = "bool expected"; break;
			case 4: s = "null expected"; break;
			case 5: s = "\"{\" expected"; break;
			case 6: s = "\"}\" expected"; break;
			case 7: s = "\":\" expected"; break;
			case 8: s = "\",\" expected"; break;
			case 9: s = "\"[\" expected"; break;
			case 10: s = "\"]\" expected"; break;
			case 11: s = "??? expected"; break;
			case 12: s = "invalid JSON"; break;
			case 13: s = "invalid Value"; break;

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
