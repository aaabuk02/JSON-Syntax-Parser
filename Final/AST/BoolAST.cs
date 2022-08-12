using System;
using System.Collections.Generic;
using System.Text;

namespace ASTClass
{
    public class BoolAST : StringAST
    {

        public BoolAST() : base() { name = "Bool"; }
        public BoolAST(AST parent) : base(parent) { name = "Bool"; }


    }
}
