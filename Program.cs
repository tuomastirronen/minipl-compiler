﻿using System;

using MiniPL;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args) {                        
            Parser p = new Parser(new Scanner(new Source(args[0])));            
            
            ProgramNode ast = p.parse();

            // SemanticAnalyzer sa = new SemanticAnalyzer(ast);

            // // sa.analyze();
            
            if (args.Length > 1) {
                if (args[1] == "mermaid") {
                    ast.mermaid("", true, true);
                }
                else if (args[1] == "tree") {
                    // ast.print("", true);
                }
                else if (args[1] == "interpret") {
                    Interpreter i = new Interpreter(ast);
                    i.interpret();          
                }
            }

            // v.visitProgramNode(p);

                    
        }
    }
}