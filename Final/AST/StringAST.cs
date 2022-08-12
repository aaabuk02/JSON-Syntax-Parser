using System;
using System.Collections.Generic;
using System.Text;

namespace ASTClass
{
    public class StringAST : TerminalAST
    {

        public StringAST() : base() { name = "String"; }
        public StringAST(AST parent) : base(parent) { name = "String"; }

        public override AST Clone()
        {
            var ast = new StringAST();
            ast.operators = operators;
            foreach (var c in children)
                ast.children.Add(c.Clone());
            ast.val = val;
            return ast;
        }

        public void SetValue(string val)
        {
            // Remove double quotes from the string
            if (val.Length >= 2)
                Value = val.Substring(1, val.Length - 2);
            else
                Value = "";

        }

        public string String()
        {
            return val;
        }



    }
}
