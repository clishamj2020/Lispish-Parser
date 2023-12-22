using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LispishParser
{
    public class Parser
    {
        List<Node> Tokens = new List<Node>();
        int TokenIndex = 0;
        public Parser(Node[] nodes)
        {
            Tokens = new List<Node>(nodes);
            Tokens.Add(new Node(Symbols.DUMMY, ""));
        }

        // <Program> ::= {<SExpr>} 

        public Node ParsePROGRAM()
        {
            List<Node> children = new List<Node>();
            while (Tokens[TokenIndex].Symbol != Symbols.DUMMY)
            {
                children.Add(ParseSEXPR());
            }
            return new Node(Symbols.Program, children.ToArray());
        }

        // <SExpr> ::= <Atom> | <List> 
        public Node ParseSEXPR()
        {
            if (Tokens[TokenIndex].Symbol == Symbols.ID || Tokens[TokenIndex].Symbol == Symbols.INT || Tokens[TokenIndex].Symbol == Symbols.REAL || Tokens[TokenIndex].Symbol == Symbols.STRING)
            {

                var atom = ParseATOM();
                return new Node(Symbols.SExpr, atom);

            }
            else
            {
                var list = ParseLIST();
                return new Node(Symbols.SExpr, list);
            }

        }
        // <List> ::= () | ( <Seq> ) 
        public Node ParseLIST()
        {
            var lparen = ParseLITERAL("(");
            if (Tokens[TokenIndex].Symbol == Symbols.LITERAL && Tokens[TokenIndex].Text == ")")
            {

                var rparen = ParseLITERAL(")");
                return new Node(Symbols.List, lparen, rparen);
            }
            else
            {
                var seq = ParseSEQ();
                var rparn = ParseLITERAL(")");
                return new Node(Symbols.List, lparen, seq, rparn);
            }

        }
        // <Seq> ::= <SExpr> <Seq> | <SExpr> 
        public Node ParseSEQ()
        {
            var sexpr = ParseSEXPR();
            if (Tokens[TokenIndex].Symbol != Symbols.LITERAL || Tokens[TokenIndex].Text != ")")
            {
                var seq = ParseSEQ();
                return new Node(Symbols.Seq, sexpr, seq);
            }
            else
            {
                return new Node(Symbols.Seq, sexpr);
            }
        }
        // <Atom> ::= ID | INT | REAL | STRING
        public Node ParseATOM()
        {
            return new Node(Symbols.Atom, Tokens[TokenIndex++]);
        }

        public Node ParseLITERAL(string text)
        {
            if (Tokens[TokenIndex].Text == text)
            {
                return Tokens[TokenIndex++];
            }
            else
            {
                throw new Exception($"Unexpected token {Tokens[TokenIndex].Text} of type {Tokens[TokenIndex].Symbol}.");
            }
        }

    }

    public enum Symbols
    {
        INVALID,
        ID,
        REAL,
        INT,
        STRING,
        LITERAL,
        DUMMY,

        Program,
        SExpr,
        List,
        Seq,
        Atom
    }

    public class Node
    {
        public Symbols Symbol;
        public string Text = "";
        public List<Node> children = new List<Node>();

        public Node(Symbols symbol, params Node[] childrens)
        {
            Symbol = symbol;
            Text = "";
            children = new List<Node>(childrens);
        }

        public Node(Symbols symbol, string text)
        {
            Symbol = symbol;
            Text = text;
        }

        public void Print(string prefix = "")
        {
            Console.WriteLine($"{prefix}{Symbol.ToString().PadRight(42 - prefix.Length)} {Text}");
            foreach (var child in children)
            {
                child.Print(prefix + "  ");
            }
        }
    }
    static public List<Node> Tokenize(String theText)
    {

        var result = new List<Node>();
        int pos = 0;
        Match m;
        var WS = new Regex(@"\G\s");
        var REAL = new Regex(@"\G[+-]?[0-9]*\.[0-9]+");
        var INT = new Regex(@"\G[+-]?[0-9]+");
        var STRING = new Regex(@"\G""(?>\\.|[^\\""])*""");
        var ID = new Regex(@"\G[^\s""()]+");
        var LITERAL = new Regex(@"\G[\\(\\)]");

        while (pos < theText.Length)
        {
            if ((m = WS.Match(theText, pos)).Success)
            {
                pos += m.Length;
            }
            else if ((m = LITERAL.Match(theText, pos)).Success)
            {
                result.Add(new Node(Symbols.LITERAL, m.Value));
                pos += m.Length;
            }
            else if ((m = REAL.Match(theText, pos)).Success)
            {
                result.Add(new Node(Symbols.REAL, m.Value));
                pos += m.Length;
            }
            else if ((m = INT.Match(theText, pos)).Success)
            {
                result.Add(new Node(Symbols.INT, m.Value));
                pos += m.Length;
            }
            else if ((m = STRING.Match(theText, pos)).Success)
            {
                result.Add(new Node(Symbols.STRING, m.Value));
                pos += m.Length;
            }
            else if ((m = ID.Match(theText, pos)).Success)
            {
                result.Add(new Node(Symbols.ID, m.Value));
                pos += m.Length;
            }
            else
            {
                throw new Exception("Lexer error");
            }
        }
        return result;
    }
    public static Node Parse(Node[] tokens)
    {
        var parser = new Parser(tokens);
        var tree = parser.ParsePROGRAM();
        return tree;
    }
    static private void CheckString(string lispcode)
    {
        try
        {
            Console.WriteLine(new String('=', 50));
            Console.Write("Input: ");
            Console.WriteLine(lispcode);
            Console.WriteLine(new String('-', 50));

            Node[] tokens = Tokenize(lispcode).ToArray();

            Console.WriteLine("Tokens");
            Console.WriteLine(new String('-', 50));
            foreach (Node node in tokens)
            {
                Console.WriteLine($"{node.Symbol,-21}\t: {node.Text}");
            }
            Console.WriteLine(new String('-', 50));
            Node parseTree = Parse(tokens);

            Console.WriteLine("Parse Tree");
            Console.WriteLine(new String('-', 50));
            parseTree.Print();
            Console.WriteLine(new String('-', 50));
        }
        catch (Exception)
        {
            Console.WriteLine("Threw an exception on invalid input.");
        }
    }
    public static void Main(string[] args)
    {
        CheckString(@"(define foo 3)");
        // CheckString(@"(define foo ""bananas"")");
        // CheckString(@"(define foo ""Say \\""Chease!\\"" "")");
        // CheckString(@"(define foo ""Say \\""Chease!\\)");
        // CheckString(@"(+ 3 4)");
        // CheckString(@"(+ 3.14 (* 4 7))");
        // CheckString(@"(+ 3.14 (* 4 7)");
    }
}