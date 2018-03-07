using System;
using System.Collections.Generic;

namespace MiniPL {
    
	public class Scanner {        
        public Source source;
        int cursor;
        int col;
        int row;
        public Scanner(Source src) {
            this.source = src;            
        }
        public Token nextToken() {
            skipWhiteSpace();
            char c = next();            
            
            // special
            if (Array.IndexOf(new []{'(', '(', ')', ':', ';'}, c) > -1) {
                if (c == ':' && lookAhead() == '=') {
                    c = next();
                    return new Token("assignment", ":=", col, row);
                }
                else {
                    return new Token(c.ToString(), null, col, row);
                }
            }

            // range
            else if (c == '.' && lookAhead() == '.') {
                c = next();
                return new Token("operation", "..", col, row);
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
                    return new Token("operation", c.ToString(), col, row);                                        
                }                
            }

            // identifier or keyword
            else if (char.IsLetter(c)) {
                string s = c.ToString();                
                while (char.IsLetter(lookAhead())) {
                    c = next();
                    s += c.ToString(); 
                }
                return new Token("identifier", s.ToString(), col, row);
            }

            // integer
            else if (char.IsDigit(c)) {
                string i = c.ToString();                
                while (char.IsDigit(lookAhead())) {
                    c = next();
                    i += c.ToString();
                }
                return new Token("integer", i.ToString(), col, row);
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
                return new Token("string", s.ToString(), col, row);
            }

            return new Token("error", c.ToString(), col, row);
        }
        public bool hasNext() {
            return cursor < source.content.Length;
        }
        private char next(int steps = 1) {            
            cursor += (steps);
            col += (steps);
            return source.content[cursor - 1];
        }
        private char lookAhead(int offset = 0) {            
            return source.content[cursor + offset];               
        }
        private void skipWhiteSpace() {
            while (char.IsWhiteSpace(lookAhead())) {           
                next();
            }            
        }
	}
}