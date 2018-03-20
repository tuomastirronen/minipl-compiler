using System;

using MiniPL;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser p = new Parser(new Scanner(new Source(args[0])));
               
            Node ast = p.parse();
            
            if (args.Length > 1) {
                if (args[1] == "mermaid") {
                    ast.mermaid("", true, true);
                }
                else if (args[1] == "tree") {
                    ast.print("", true);
                }
                else if (args[1] == "interpret") {
                    ast.interpret();
                }
            }
                    
        }
    }
}