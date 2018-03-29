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
            return new Token(type, value, row, col - value.Length);
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
                }
                else {
                    // TODO
                    switch (c)
                    {
                        case '(':
                            return createToken(Token.LPAR, c.ToString());
                        case ')':
                            return createToken(Token.RPAR, c.ToString());
                        case ':':
                            return createToken(Token.COL, c.ToString());
                        case ';':
                            return createToken(Token.SCOL, c.ToString());
                        default:
                            break;
                    }
                }
            }

            // range
            else if (c == '.' && lookAhead() == '.') {
                c = next();
                return createToken(Token.RANGE, "..");                
            }

            // operation
            else if (Array.IndexOf(new []{'+', '-', '*', '/', '<', '=', '&', '!'}, c) > -1) {                
                // comments                
                if (c == '/' && lookAhead() == '*') {                        
                    c = next();
                    while (true) {
                        c = next();               
                        if (lookAhead() == '*' && lookAhead(1) == '/') {                            
                            break;
                        }
                    }                    
                    c = next(3);
                    return nextToken();
                }
                else if (c == '/' && lookAhead() == '/') {                                        
                    next();
                    while (!lookAhead().Equals('\n')) {                        
                        c = next();                        
                    }
                    c = next();                    
                    return nextToken();
                }
                else {
                    switch (c)
                    {
                        case '+':
                            return createToken(Token.ADD, c.ToString());
                        case '-':
                            return createToken(Token.SUB, c.ToString());
                        case '*':
                            return createToken(Token.MUL, c.ToString());
                        case '/':
                            return createToken(Token.DIV, c.ToString());
                        case '<':
                            return createToken(Token.LT, c.ToString());
                        case '=':
                            return createToken(Token.EQ, c.ToString());
                        case '&':
                            return createToken(Token.AND, c.ToString());
                        case '!':
                            return createToken(Token.NEG, c.ToString());
                        default:
                            break;
                    }
                    // TODO
                    return createToken(Token.RANGE, "..");                                                        
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
            }

            // integer
            else if (char.IsDigit(c)) {
                string i = c.ToString();                
                while (char.IsDigit(lookAhead())) {
                    c = next();
                    i += c.ToString();
                }
                
                return createToken(Token.INT, i.ToString());                
            }

            // string literal
            else if (c == '"') {
                
                c = next();
                string s = "";

                while (c != '"') {
                    s += c.ToString();                    
                    // special characters
                    if (c == '\\') {
                        s = s.Remove(s.Length - 1); // remove the \ from built string            
                        string special = lookAhead().ToString();                           
                        switch (special)
                        {
                            case "n":
                                c = '\n';
                                s += c.ToString();
                                break;
                            case "t":
                                c = '\t';
                                s += c.ToString();
                                break;
                            case "\"":                                
                                c = '"';                                
                                s += c.ToString();
                                break;
                            default:
                                Console.WriteLine(special.Length);
                                s += special; // not so special                         
                                break;
                        }
                        
                        c = next();
                    }                    
                    c = next();                    
                    // Console.WriteLine("c at the moment: " + c);
                }
                // next(); // eat closing "                
                return createToken(Token.STRING, s.ToString());                
            }
            
            Token unknown = createToken(Token.UNKNOWN, c.ToString());
            new LexicalError(unknown);
            return unknown;
        }
	}
}