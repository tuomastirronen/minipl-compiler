using System;

using MiniPL;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args) {                        
            Parser p = new Parser(new Scanner(new Source(args[0])));
            ProgramNode ast = p.parse();
                        
            // If there are no lexical or syntax errors, prcodeed to semantic analysis       
            if (Error.errors.Count == 0) {
                // ast.mermaid("", true, true);
                new SemanticAnalyzer(ast).analyze();

                // If there are no semantic errors, interpret the program
                if (Error.errors.Count == 0) {
                    new Interpreter(ast).interpret();                
                }
                else {
                    Error.printErrors();
                }
            }
            else {
                Error.printErrors();
            }                              
        }
    }
}