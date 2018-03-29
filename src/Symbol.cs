using System;
using System.Collections.Generic;

namespace MiniPL {
    public class Symbol {
        public string name;
        public string type;
        public string value;

        public Symbol(string name, string type) {
            this.name = name;
            this.type = type;
            this.value = "0";
        }

    }

	public class SymbolTable {

        public static List<Symbol> symbols = new List<Symbol>();

		public SymbolTable() { }

        public static void declare(string name, string type) {            
            symbols.Add(new Symbol(name, type));
        }

        public static void assign(string name, string value) {            
            foreach (var symbol in symbols) {
                if(symbol.name.Equals(name))
                    symbol.value = value;                              
            }        
        }

        public static string lookupType(string name) {                      
            foreach (var symbol in symbols) {
                if(symbol.name.Equals(name))
                    return symbol.type;
            }        
            return null;
        }

        public static string lookup(string name) {                    
            foreach (var symbol in symbols) {
                if(symbol.name.Equals(name))                    
                    return symbol.value;
            }                        
            return null;
        }
	}    
}