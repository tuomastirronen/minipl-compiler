using System;
using System.Collections.Generic;

namespace MiniPL {
    
	public class Scanner {        
        public Source source;        

        int cursor;
        int col = 0;
        int row = 1;

        public Scanner(Source source) {
            this.source = source;            
        }

        private Token createToken(string type, string value) {
            return new Token(type, value, row, col);
        }
        
        public bool hasNext() {
            return cursor < source.content.Length;
        }
        private char next(int steps = 1) {
            cursor += (steps);
            col++;    
            return source.content[cursor - 1];
        }
        private char lookAhead(int steps = 0) {            
            return source.content[cursor + steps];               
        }
        private void skipWhiteSpace() {
            while (char.IsWhiteSpace(lookAhead())) {
                if (lookAhead().Equals('\n')) {
                    row++;
                    col = 0;
                }
                next();
            }            
        }

        public Token nextToken() {            
            skipWhiteSpace();
            char c = next();            
            
            // special
            if (Array.IndexOf(new []{'(', ')', ':', ';'}, c) > -1) {
                if (c == ':' && lookAhead() == '=') {
                    c = next();
                    return createToken(Token.ASS, ":=");
                    // return new Token("assignment", ":=", col, row);
                }
                else {
                    // TODO
                    return createToken(Token.ASS, c.ToString());
                    // return new Token(c.ToString(), null, col, row);
                }
            }

            // range
            else if (c == '.' && lookAhead() == '.') {
                c = next();
                return createToken(Token.RANGE, "..");
                // return new Token("operation", "..", col, row);
            }

            // operation
            else if (Array.IndexOf(new []{'+', '-', '*', '/', '<', '=', '&', '!'}, c) > -1) {
                
                // comments
                if (c == '/' && lookAhead() == '*') {
                    next();
                    while (true) {
                        next();               
                        if (lookAhead() == '*' && lookAhead(1) == '/') {
                            break;
                        }
                    }                    
                    next(2);
                }
                else if (c == '/' && lookAhead() == '/') {                    
                    while (lookAhead() != '\n') {
                        next();
                    }                    
                }
                else {
                    // TODO
                    return createToken(Token.RANGE, "..");
                    // return new Token("operation", c.ToString(), col, row);                                        
                }                
            }

            // identifier or keyword
            else if (char.IsLetter(c)) {
                string s = c.ToString();                
                while (char.IsLetter(lookAhead())) {
                    c = next();
                    s += c.ToString(); 
                }
                return createToken(Token.ID, s.ToString());
                // return new Token("identifier", s.ToString(), col, row);
            }

            // integer
            else if (char.IsDigit(c)) {
                string i = c.ToString();                
                while (char.IsDigit(lookAhead())) {
                    c = next();
                    i += c.ToString();
                }
                
                return createToken(Token.INT, i.ToString());
                // return new Token("integer", i.ToString(), col, row);
            }

            // string literal
            else if (c == '"') {
                c = next();
                string s = c.ToString();                
                while (lookAhead() != '"') {
                    c = next();
                    s += c.ToString();
                }
                next(); // eat closing "
                return createToken(Token.STRING, s.ToString());
                // return new Token("string", s.ToString(), col, row);
            }

            return createToken(Token.ERROR, c.ToString());
            // return new Token("error", c.ToString(), col, row);
        }
	}
}