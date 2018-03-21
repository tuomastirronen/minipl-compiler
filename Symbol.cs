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

        public List<Symbol> symbols = new List<Symbol>();

		public SymbolTable() { }

        public void declare(string name, string type) {            
            symbols.Add(new Symbol(name, type));
        }

        public void assign(string name, string value) {            
            bool found = false;
            foreach (var symbol in symbols) {
                if(symbol.name.Equals(name))
                    symbol.value = value;
                    found = true;                  
            }
            if (!found) {
                new SemanticError("Semantic Error: " + name + " does not exist in this context.");
            }            
        }

        public int lookup(string name) {
            bool found = false;
            foreach (var symbol in symbols) {
                if(symbol.name.Equals(name))                    
                    return Int32.Parse(symbol.value);
            }
            if (!found) {
                new SemanticError("Semantic Error: " + name + " does not exist in this context.");
            }
            return -1;
        }    
	}    
}