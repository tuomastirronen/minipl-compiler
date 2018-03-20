using System;
using System.Collections.Generic;

namespace MiniPL {
    
	public class Parser {        
        Scanner scanner;
        Token currentToken = null;

        public Parser(Scanner scanner) {
            this.scanner = scanner;
        }

        private void nextToken() {
            if (scanner.hasNext()) {
                currentToken = scanner.nextToken();
            }            
            else {
                currentToken = new Token(Token.EOF, null);
            }
            // Console.WriteLine("token: " + currentToken);
        }

        private bool accept(string type) {            
            return currentToken.type == type;
        }

        private bool accept_keyword(string value) {            
            return currentToken.value == value;
        }

        private void match(string type) {
            // Console.WriteLine("\t-> match " + type);
            if (currentToken.type == type) {
                nextToken();
            }                
            else {                
                new SyntaxError(currentToken, "Syntax Error: Expected " + type + ", got " + currentToken.type + " '" + currentToken.value + "'");
            }                
        }
        
        private void match_keyword(string value) {
            // Console.WriteLine("\t-> match keyword " + value);       
            if (currentToken.value == value) {
                nextToken();
            }                
            else {                
                new SyntaxError(currentToken, "Syntax Error: Expected " + value + ", got " + currentToken.value);
            }                
        }

        public Node parse() {            
            return prog();
        }

        private Node prog() {
            nextToken();

            Node program = new ProgramNode();

            program.addChild(stmts());

            return program;
        }

        private Node stmts() {
            Node stmts = new Node("stmts");

            while (scanner.hasNext()){                
                stmts.addChild(stmt());
            }

            return stmts;
        }

        private Node stmt() {
            Node statement = new StatementNode();

            if (currentToken.type == Token.KW) {
                switch (currentToken.value)
                {                    
                    case "for":
                        // for loop
                        statement.addChild(new Node(currentToken));
                        match_keyword("for");
                        statement.addChild(new Node(currentToken));
                        match(Token.ID);
                        statement.addChild(new Node(currentToken));
                        match_keyword("in");
                        statement.addChild(expr());
                        statement.addChild(new Node(currentToken));
                        match(Token.RANGE);
                        statement.addChild(expr());
                        statement.addChild(new Node(currentToken));
                        match_keyword("do");

                        Node stmts = new Node("stmts");
                        
                        while (!accept_keyword("end")) {                            
                            stmts.addChild(stmt());
                        }
                        statement.addChild(stmts);

                        statement.addChild(new Node(currentToken));
                        match_keyword("end");
                        statement.addChild(new Node(currentToken));
                        match_keyword("for");
                        break;
                    case "read":                        
                        Node read = new ReadNode();
                        statement.addChild(read);
                        match_keyword("read");
                        read.addChild(new IdNode(currentToken));
                        match(Token.ID);
                        break;
                    case "print":
                        Node print = new PrintNode();
                        statement.addChild(print);
                        match_keyword("print");
                        print.addChild(expr());
                        break;
                    case "assert":
                        statement.addChild(new Node("assert"));
                        match_keyword("assert");
                        statement.addChild(new Node(currentToken));
                        match(Token.LPAR);                        
                        statement.addChild(expr());
                        statement.addChild(new Node(currentToken));
                        match(Token.RPAR);
                        break;
                    case "var":
                        // variable declaration                        
                        Node declaration = new DeclarationNode();
                        match_keyword("var");
                        Node id = new IdNode(currentToken);
                        declaration.addChild(id);
                        match(Token.ID);                        
                        match(Token.COL);
                        declaration.addChild(new Node(currentToken));

                        statement.addChild(declaration);
                        
                        if (accept_keyword("int")) {
                            match_keyword("int");
                        }
                        else if (accept_keyword("string")) {
                            match_keyword("string");
                        }
                        else if (accept_keyword("bool")) {
                            match_keyword("bool");
                        }

                        else new SyntaxError(currentToken, "Syntax Error: Expected int, string or bool, got " + currentToken.value);
                        
                        if (accept(Token.ASS)) {
                            Node assignment = new AssignmentNode();
                            assignment.addChild(id);
                            match(Token.ASS);
                            assignment.addChild(expr());          
                            statement.addChild(assignment);
                        }                        
                        break;
                    default:
                        new SyntaxError(currentToken, "Syntax Error: Expected keyword var, for, read, print or assert, got " + currentToken.value);                        
                        break;
                }
            }
            if (currentToken.type == Token.ID) {
                // assignment
                // statement.addChild(new IdNode(currentToken));
                Node id = new IdNode(currentToken);
                match(Token.ID);
                Node assignment = new AssignmentNode();
                assignment.addChild(id);                
                match(Token.ASS);                        
                assignment.addChild(expr());
                statement.addChild(assignment);
            }

            match(Token.SCOL);
            return statement;
        }

        private Node factor() {
            // factor : INTEGER | LPAREN expr RPAREN            
            Node node = new Node();

            switch (currentToken.type)
            {                
                case "integer":                    
                    node = new IntNode(currentToken);
                    match(Token.INT);
                    break;
                case "string":                    
                    node = new StrNode(currentToken);
                    match(Token.STRING);
                    break;
                case "identifier":                    
                    node = new IdNode(currentToken);
                    match(Token.ID);
                    break;
                case "lpar":                    
                    nextToken();                    
                    node = expr();                     
                    match(Token.RPAR);
                    break;
                default:                    
                    new SyntaxError(currentToken, "Syntax Error: Expected integer, string, bool, identifier or (, got " + currentToken.value);
                    break;
            }

            return node;
        }        

        private Node term() {
            // term : factor ((MUL | DIV) factor)*
            
            Node left = factor();            

            while (currentToken.type == Token.MUL | currentToken.type == Token.DIV | currentToken.type == Token.LT | currentToken.type == Token.AND | currentToken.type == Token.EQ) {                
                if (currentToken.type == Token.MUL) {
                    Node node = new MultiplicationNode();                    
                    nextToken();
                    node.addChild(left);
                    node.addChild(factor());
                    left = node;
                }
                else if (currentToken.type == Token.DIV) {
                    Node node = new DivisionNode();                    
                    nextToken();
                    node.addChild(left);
                    node.addChild(factor());
                    left = node;
                }
            }

            return left;  
        }

        private Node expr() {            
            // expr   : term ((PLUS | MINUS) term)*
            // term   : factor ((MUL | DIV) factor)*
            // factor : INTEGER | LPAREN expr RPAREN            
            
            Node left = term();

            while (currentToken.type == Token.ADD | currentToken.type == Token.SUB) {
                if (currentToken.type == Token.ADD) {
                    Node node = new AdditionNode();                    
                    nextToken();
                    node.addChild(left);
                    node.addChild(term());
                    left = node;
                    
                } 
                else if (currentToken.type == Token.SUB)  {
                    Node node = new SubstractionNode();                    
                    nextToken();
                    node.addChild(left);
                    node.addChild(term());
                    left = node;
                    
                }                                           
            }

            return left;  
        }
    }
}