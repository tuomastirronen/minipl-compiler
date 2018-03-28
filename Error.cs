using System;
using System.Collections.Generic;

namespace MiniPL {
    public class Error {
        public int row;
        public int col;
        public string msg;
        
        public static List<Error> errors = new List<Error>();

        public Error() {
            this.msg = "Unknown error";
        }

        public void handleError() {
            // throw new System.Exception(this.msg);              
            errors.Add(this);            
            // Console.WriteLine(this);
            // Environment.Exit(0);
        }

        public static void printErrors() {
            Console.WriteLine("Compilation failed:");
            foreach (Error error in Error.errors) {  
                Console.WriteLine(error);
            }
        }

        public override string ToString() {
            return "\t" + this.msg + " (at line " + this.row + ").";
        }

    }

    public class LexicalError : Error {        
        public LexicalError() { }

        public LexicalError(Token token) {
            this.msg = "Lexical Error: Unexpected token '" + token.value + "'";
            this.row = token.row;
            this.col = token.col;
            handleError();
        }
    }

    public class SyntaxError : Error {
        public SyntaxError() { }

        public SyntaxError(Token token, string msg) {
            this.msg = msg;
            this.row = token.row;
            this.col = token.col; // substract the word length
            handleError();
        }      
    }

    public class SemanticError : Error {
        public SemanticError() { }

        public SemanticError(Node node, string msg) {
            this.msg = msg;
            this.row = node.row;
            this.col = node.col;
            handleError();
        }      
    }
}