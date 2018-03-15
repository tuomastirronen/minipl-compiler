using System;
using System.Collections.Generic;

namespace MiniPL {
    
	public class Parser {        
        Scanner scanner;
        Token currentToken = null;

        public Parser(Scanner scanner) {
            this.scanner = scanner;
        }        

        private void error(string msg) {
            // Console.WriteLine(msg);
            // Environment.Exit(0);
            throw new System.Exception(msg);
        }

        private void nextToken() {
            if (scanner.hasNext()) {
                currentToken = scanner.nextToken();
            }            
            else {
                currentToken = new Token(Token.EOF, null);
            }
            Console.WriteLine("token: " + currentToken);
        }

        private bool accept(string type) {
            Console.WriteLine(currentToken.type + " vs. " + type);
            return currentToken.type == type;
        }

        private bool accept_keyword(string value) {
            Console.WriteLine(currentToken.value + " vs. " + value);
            return currentToken.value == value;
        }

        private void match(string type) {
            Console.WriteLine("\t-> match " + type);
            if (currentToken.type == type) {
                nextToken();
            }                
            else {
                error("syntax error: expected " + type + ", got " + currentToken.type + " '" + currentToken.value + "'");
            }                
        }
        
        private void match_keyword(string value) {
            Console.WriteLine("\t-> match keyword " + value);       
            if (currentToken.value == value) {
                nextToken();
            }                
            else {
                error("syntax error: expected " + value + ", got " + currentToken.value);
            }                
        }

        public Node parse() {            
            return prog();
        }

        private Node prog() {
            nextToken();

            Node program = new Node("program");

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
            Node statement = new Node("stmt");

            if (currentToken.type == Token.KW) {
                switch (currentToken.value)
                {
                    case "var":
                        // variable declaration
                        statement.addChild(new Node(currentToken));        
                        match_keyword("var");
                        statement.addChild(new Node(currentToken));
                        match(Token.ID);
                        statement.addChild(new Node(currentToken));
                        match(Token.COL);
                        statement.addChild(new Node(currentToken));
                        
                        if (accept_keyword("int")) {
                            match_keyword("int");
                        }
                        else if (accept_keyword("string")) {
                            match_keyword("string");
                        }
                        else if (accept_keyword("bool")) {
                            match_keyword("bool");
                        }
                        else error("syntax error: expected int, string or bool, got " + currentToken.value);
                        
                        if (accept(Token.ASS)) {
                            statement.addChild(new Node(currentToken));
                            match(Token.ASS);                        
                            statement.addChild(expr());
                        }                        
                        break;
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
                        statement.addChild(new Node("read"));
                        match_keyword("read");
                        statement.addChild(new Node(currentToken));
                        match(Token.ID);
                        break;
                    case "print":
                        statement.addChild(new Node("print"));
                        match_keyword("print");
                        statement.addChild(expr());
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
                    default:
                        error("syntax error: expected var, for, read, print or assert, got " + currentToken.value);
                        break;
                }
            }
            if (currentToken.type == Token.ID) {
                // assignment
                statement.addChild(new Node(currentToken));
                match(Token.ID);
                statement.addChild(new Node(currentToken));
                match(Token.ASS);                        
                statement.addChild(expr());
            }

            match(Token.SCOL);
            return statement;
        }

        private Node factor() {
            // factor : INTEGER | LPAREN expr RPAREN
            Node factor = new Node("factor");
            switch (currentToken.type)
            {                
                case "integer":
                    factor.addChild(new Node(currentToken));
                    match(Token.INT);
                    break;
                case "string":
                    factor.addChild(new Node(currentToken));
                    match(Token.STRING);
                    break;
                case "identifier":
                    factor.addChild(new Node(currentToken));
                    match(Token.ID);
                    break;
                case "lpar":
                    factor.addChild(new Node(currentToken));
                    nextToken();
                    factor.addChild(expr());
                    factor.addChild(new Node(currentToken));         
                    match(Token.RPAR);
                    break;
                default:
                    error("syntax error: expected integer, string, bool, identifier or (, got " + currentToken.value);
                    break;
            }

            return factor;
        }        

        private Node term() {
            // term : factor ((MUL | DIV) factor)*
            Node term = new Node("term");

            term.addChild(factor());
            while (currentToken.type == Token.MUL | currentToken.type == Token.DIV) {
                term.addChild(new Node(currentToken));                 
                nextToken();
                term.addChild(factor());
            }

            return term;  
        }
        private Node expr() {            
            // expr   : term ((PLUS | MINUS) term)*
            // term   : factor ((MUL | DIV) factor)*
            // factor : INTEGER | LPAREN expr RPAREN            
            Node expr = new Node("expr");
            expr.addChild(term());

            while (currentToken.type == Token.ADD | currentToken.type == Token.SUB) {
                expr.addChild(new Node(currentToken));
                nextToken();
                expr.addChild(term());
            }

            return expr;  
        }
    }
}