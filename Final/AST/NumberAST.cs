using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ASTClass
{
    class NumberAST : TerminalAST
    {
        public NumberAST() : base() { name = "Number"; }
        public NumberAST(AST parent) : base(parent) { name = "Number"; }

        public override AST Clone()
        {
            NumberAST ast = new NumberAST();
            ast.operators = operators;
            foreach (AST c in children)
                ast.children.Add(c.Clone());

            return ast;
        }
    }
}
