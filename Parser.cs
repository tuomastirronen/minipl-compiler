using System;
using System.Collections.Generic;

namespace MiniPL {
    
	public class Parser {        
        Scanner scanner;
        Token currentToken = null;

        public Parser(Scanner sc) {
            this.scanner = sc;
        }
        
        public void parse_() {
            while (scanner.hasNext()) {
                Token t = scanner.nextToken();
                if (t != null) {
                    Console.WriteLine(t);
                    // currentToken = t;
                }
            }
            // Console.WriteLine(add.displayNode());
        }

        public Node parse() {
            // currentToken = scanner.nextToken();            
            // return expr();

            while (scanner.hasNext()){
                Token t = scanner.nextToken();
                Console.WriteLine(t);
        }

            return null;
        }

        private void error(string msg) {
            Console.WriteLine(msg);
        }

        // # compare the current token type with the passed token
        // # type and if they match then "eat" the current token
        // # and assign the next token to the self.current_token,
        // # otherwise raise an exception. 
        private void eat(string type) {
            // Console.WriteLine("eat " + type);
            if (currentToken.type == type) {
                currentToken = scanner.nextToken();
            }                
            else {
                error("syntax error");
            }                
        }       

        private Node factor() {
            // factor : INTEGER | LPAREN expr RPAREN
            Token token = currentToken;
            // Console.WriteLine("factor");   
            if (token.type == "integer") {
                eat("integer");
                // Console.WriteLine("create integer");
                return new Node(token);
            }
                
            else if (token.type == "(") {
                eat("(");
                Node node = expr();
                eat(")");
                return node;
            }
            error("factor error");
            return null;   
        }        

        private Node term() {
            // term : factor ((MUL | DIV) factor)*
            // Console.WriteLine("term");
            Node node = factor(); // return integer
            // Console.WriteLine("term " + node);
            // while (currentToken.type == "*" | currentToken.type == "/") {
            while (currentToken.type == "operation") {
                Token token = currentToken;
                // if (token.type == "*") {
                //     eat("*");
                // }
                // else if (token.type == "/") {
                //     eat("/");
                // }

                eat(token.type);

                node = new Node(token, node, factor());

            }

            return node;
        }
        private Node expr() {            
            // expr   : term ((PLUS | MINUS) term)*
            // term   : factor ((MUL | DIV) factor)*
            // factor : INTEGER | LPAREN expr RPAREN            
            Node node = term();
            // Console.WriteLine("expr " + node);            

            // while (currentToken.type == "+" | currentToken.type == "-") {
            while (currentToken.type == "operation") {
                Token token = currentToken;
                // if (token.type == "+") {
                //     eat("+");
                // }
                // else if (currentToken.type == "-") {
                //     eat("-");
                // }
                eat(token.type);
                node = new Node(token, node, term());

            }
            return node;
        }
    }
}