using System;

namespace MiniPL {
    public class Error {
        public int row;
        public int col;
        public string msg;

        public Error() {
            this.msg = "Unknown error";
        }

        public void handleError() {
            // throw new System.Exception(this.msg);
            Console.WriteLine(this);
            Environment.Exit(0);
        }

        public override string ToString() {
            return this.msg + " at (row: " + this.row + ", col: " + this.col + ").";
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
            this.col = token.col;
            handleError();
        }      
    }

    public class SemanticError : Error {
        public SemanticError() { }

        public SemanticError(string msg) {
            this.msg = msg;            
            handleError();
        }      
    }
}