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

            Node six = new Node(new Token("integer", "6", 0, 0));
            Node two = new Node(new Token("integer", "2", 0, 0));
            Node mul = new Node(new Token("operation", "*", 0, 0), six, two);
            Node four = new Node(new Token("integer", "4", 0, 0));
            Node add = new Node(new Token("operation", "+", 0, 0), four, mul);
            Console.WriteLine(add.displayNode());
        }

        public Node parse() {
            currentToken = scanner.nextToken();            
            return expr();
        }

        private void error(string msg) {
            Console.WriteLine(msg);
        }

        // # compare the current token type with the passed token
        // # type and if they match then "eat" the current token
        // # and assign the next token to the self.current_token,
        // # otherwise raise an exception. 
        private void eat(string symbol) {
            // Console.WriteLine("eat " + symbol);
            if (currentToken.symbol == symbol) {
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
            if (token.symbol == "integer") {
                eat("integer");
                // Console.WriteLine("create integer");
                return new Node(token);
            }
                
            else if (token.symbol == "(") {
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
            // while (currentToken.symbol == "*" | currentToken.symbol == "/") {
            while (currentToken.symbol == "operation") {
                Token token = currentToken;
                // if (token.symbol == "*") {
                //     eat("*");
                // }
                // else if (token.symbol == "/") {
                //     eat("/");
                // }

                eat(token.symbol);

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

            // while (currentToken.symbol == "+" | currentToken.symbol == "-") {
            while (currentToken.symbol == "operation") {
                Token token = currentToken;
                // if (token.symbol == "+") {
                //     eat("+");
                // }
                // else if (currentToken.symbol == "-") {
                //     eat("-");
                // }
                eat(token.symbol);
                node = new Node(token, node, term());

            }
            return node;
        }
    }
}