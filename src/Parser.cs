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
        }

        private void nextStatement() {
            while (currentToken.type != Token.SCOL) {                
                nextToken();
                if (currentToken.type == Token.EOF) {
                    return;
                }
            }
            nextToken();            
        }

        private bool accept(string type) {            
            return currentToken.type == type;
        }

        private bool accept_keyword(string value) {            
            return currentToken.value == value;
        }

        private void match(string type) {
            if (currentToken.type == Token.UNKNOWN) { // There has been a lexical error
                nextToken(); // recover from error by skipping to next token
            }
            else if (currentToken.type == type) {
                nextToken();
            }                
            else {                
                new SyntaxError(currentToken, "Syntax Error: Expected " + type + ", got " + currentToken.type + " '" + currentToken.value + "'");
                nextStatement();                
            }
        }
        
        private void match_keyword(string value) {
            if (currentToken.value == value) {
                nextToken();
            }
            else {
                new SyntaxError(currentToken, "Syntax Error: Expected " + value + ", got " + currentToken.value);
                nextStatement();
            }
        }

        public ProgramNode parse() {            
            return prog();
        }

        private ProgramNode prog() {
            nextToken();

            ProgramNode program = new ProgramNode();

            program.addChild(block());

            return program;
        }

        private Node block() {
            Node block = new BlockNode();

            while (scanner.hasNext()){                
                block.addChild(stmt());
            }

            return block;
        }

        private Node stmt() {            
            Node statement = new StatementNode();                      
       
            if (currentToken.type == Token.KW) {
                if (currentToken.value == "for") {
                    // for loop
                    Node forLoop = new ForLoopNode();                    
                    match_keyword("for");

                    // Control node
                    Node control = new ForControlNode();

                    // Condition
                    Node condition = new ForConditionNode();             

                    Node assignment = new AssignmentNode();                    
                    Node id = new IdNode(currentToken);
                    assignment.addChild(id);

                    condition.addChild(id);

                    match(Token.ID);                        
                    match_keyword("in");                        
                    assignment.addChild(expr());                        
                    match(Token.RANGE);
                    
                    condition.addChild(expr());                    

                    match_keyword("do");                    
                    control.addChild(assignment);
                    control.addChild(condition);                        
                    forLoop.addChild(control);
                    statement.addChild(forLoop);


                    // Add statements
                    Node stmts = new BlockNode();
                    
                    while (!accept_keyword("end")) {
                        stmts.addChild(stmt());
                    }
                    forLoop.addChild(stmts);
                    
                    match_keyword("end");                        
                    match_keyword("for");
                }
                else if (currentToken.value == "read") {
                    Node read = new ReadNode();
                    statement.addChild(read);
                    match_keyword("read");
                    read.addChild(new IdNode(currentToken));
                    match(Token.ID);
                }
                else if (currentToken.value == "print") {
                    Node print = new PrintNode();
                    statement.addChild(print);
                    match_keyword("print");
                    print.addChild(expr());
                }
                else if (currentToken.value == "assert") {
                    Node assert = new AssertNode();                        
                    match_keyword("assert"); 
                    match(Token.LPAR);                        
                    assert.addChild(expr());                        
                    match(Token.RPAR);
                    statement.addChild(assert);                        
                }
                else if (currentToken.value == "var") {
                    // variable declaration                        
                    Node declaration = new DeclarationNode();
                    match_keyword("var");
                    Node id = new IdNode(currentToken);
                    declaration.addChild(id);
                    match(Token.ID);
                    match(Token.COL);
                    
                    if (accept_keyword("int")) {
                        id.type = Token.INT;
                        match_keyword("int");
                    }
                    else if (accept_keyword("string")) {                        
                        id.type = Token.STRING;
                        match_keyword("string");
                    }
                    else if (accept_keyword("bool")) {                        
                        id.type = Token.BOOL;                        
                        match_keyword("bool");
                    }
                    
                    else { 
                        new SyntaxError(currentToken, "Syntax Error: Expected type int, string or bool, got '" + currentToken.value + "'");
                        nextStatement();                        
                        return null;
                                         
                    }

                    statement.addChild(declaration);
                    
                    if (accept(Token.ASS)) {
                        Node assignment = new AssignmentNode();
                        assignment.addChild(id);
                        match(Token.ASS);
                        assignment.addChild(expr());          
                        statement.addChild(assignment);
                    }                        
                } 
                else {
                    new SyntaxError(currentToken, "Syntax Error: Expected keyword var, for, read, print or assert, got " + currentToken.value);                        
                    statement = new StatementNode();                     
                    return null;
                }
                match(Token.SCOL);
            }            
            else if (currentToken.type == Token.ID) {
                // assignment                
                Node id = new IdNode(currentToken);
                match(Token.ID);
                Node assignment = new AssignmentNode();
                assignment.addChild(id);                
                match(Token.ASS);                        
                assignment.addChild(expr());
                statement.addChild(assignment);
                match(Token.SCOL);
            }          

            return statement;
        }

        private Node factor() {
            // factor : INTEGER | LPAREN expr RPAREN            
            Node node = null;            
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
                case "boolean":                    
                    node = new BoolNode(currentToken);
                    match(Token.BOOL);                    
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
                case "negation":                                        
                    node = new UnOpNode(currentToken);                 
                    match(Token.NEG);
                    node.addChild(factor());                    
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
                Node node = new BinOpNode(currentToken);
                nextToken();
                node.addChild(left);
                node.addChild(factor());
                left = node;
            }            

            return left;  
        }

        private Node expr() {            
            // expr   : term ((PLUS | MINUS) term)*
            // term   : factor ((MUL | DIV) factor)*
            // factor : INTEGER | LPAREN expr RPAREN            
            
            Node left = term();

            while (currentToken.type == Token.ADD | currentToken.type == Token.SUB) {                
                    Node node = new BinOpNode(currentToken);                    
                    nextToken();
                    node.addChild(left);
                    node.addChild(term());
                    left = node;
            }    

            return left;  
        }
    }
}