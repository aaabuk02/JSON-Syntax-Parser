using System;

//From Coco/R user manual

namespace Sy
{

    public class Obj
    { // object decribing a declared name
        public string name; // name of the object
        public int type; // type of the object (undef for procs)
        public Obj next; // to next object in same scope
        public int kind; // var, proc, scope
        public int adr; // address in memory or start of proc
        public int level; // nesting level; 0=global, 1=local
        public Obj locals; // scopes: to locally declared objects
        public int nextAdr; // scopes: next free address in this scope
    }
    public class SymbolTable
    {
        const int // types
        undef = 0, integer = 1, boolean = 2;
        const int var = 0, proc = 1, scope = 2;

        public int curLevel; // nesting level of current scope
        public Obj undefObj; // object node for erroneous symbols
        public Obj topScope; // topmost procedure scope
        Parser parser;

        public SymbolTable(Parser parser)
        {
            this.parser = parser;
        }

        // open a new scope and make it the current scope (topScope)
        public void OpenScope()
        {
            Obj scop = new Obj();
            scop.name = ""; scop.kind = scope;
            scop.locals = null; scop.nextAdr = 0;
            scop.next = topScope; topScope = scop;
            curLevel++;
        }
        // close the current scope
        public void CloseScope()
        {
            topScope = topScope.next; curLevel--;
        }
        // create a new object node in the current scope
        public Obj NewObj(string name, int kind, int type)
        {
            Obj p, last, obj = new Obj();
            obj.name = name; obj.kind = kind; obj.type = type;
            obj.level = curLevel;
            p = topScope.locals; last = null;
            while (p != null)
            {
                if (p.name == name) parser.SemErr("name declared twice");
                last = p; p = p.next;
            }
            if (last == null) topScope.locals = obj; else last.next = obj;
            if (kind == var) obj.adr = topScope.nextAdr++;
            return obj;
        }
    }
}