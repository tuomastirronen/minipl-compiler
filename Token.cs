using System;
using System.Linq;

namespace MiniPL {
    public class Token {
        
        public string[] KEYWORDS = new []{"var", "for", "end", "in", "do", "read", "print", "int", "string", "bool", "assert"};
        
        public string symbol;
		public string lexeme;
        public int col;
        public int row;

        public Token(string symbol, string lexeme, int col, int row) {

            this.col = col;
            this.row = row;
            if (KEYWORDS.Contains(lexeme)) {                
                this.symbol = "keyword";
            }   
            else {
                this.symbol = symbol;
            }            
            this.lexeme = lexeme;
        }

        public override string ToString() {
            // return "Token -> "
            // + "row: " + this.row
            // + "\tcol: " + this.col            
            // + "\tlexeme: " + this.lexeme
            // + "\tsymbol: " + this.symbol;
            return "(symbol: " + this.symbol + ", lexeme: " + this.lexeme + ")";
        }
    }
}