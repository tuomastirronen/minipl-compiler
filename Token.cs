using System;
using System.Linq;

namespace MiniPL {
    public class Token {

        // Types
        public static string ID = "identifier";
        public static string KW = "keyword";
        public static string LPAR = "lpar";
        public static string RPAR = "rpar";
        public static string MUL = "multiplication";
        public static string DIV = "division";
        public static string ADD = "addition";
        public static string SUB = "substraction";
        public static string EQ = "equals";
        public static string NEG = "negation";
        public static string LT = "lthan";
        public static string AND = "and";
        public static string COL = "colon";
        public static string SCOL = "semicolon";        
        public static string ASS = "assignment";
        public static string RANGE = "range";
        public static string INT = "integer";
        public static string STRING = "string";
        public static string BOOL = "boolean";        
        public static string EOF = "eof";
        public static string UNKNOWN = "unknown";        
        
        public string[] KEYWORDS = new []{"var", "for", "end", "in", "do", "read", "print", "int", "string", "bool", "assert"};
        
        public string type;
        public string value;
        public int col;
        public int row;

        public Token(string type, string value, int row = 0, int col = 0) {

            this.row = row;
            this.col = col;

            if (KEYWORDS.Contains(value)) {                
                this.type = KW;
            }   
            else {
                this.type = type;
            }            
            this.value = value;
        }

        public override string ToString() {
            return "(" + this.type + ", " + this.value + ")";
        }
    }
}