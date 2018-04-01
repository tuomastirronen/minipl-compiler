using System;

namespace MiniPL {
	public class Interpreter : IVisitor<object> {
        Node ast;
		
		public Interpreter(Node node) {
            this.ast = node;
	    }

        public void interpret() {
            ast.accept(this);
        }

        object IVisitor<object>.visit(ProgramNode node) {            
            foreach (var child in node.children) {
                child.accept(this);
            }            
            return null;
        }

        object IVisitor<object>.visit(BlockNode node) {            
            foreach (var child in node.children) {
                child.accept(this);
            }            
            return null;
        }

        object IVisitor<object>.visit(StatementNode node) {            
            foreach (var child in node.children) {                
                child.accept(this);
            }            
            return null;
        }

        object IVisitor<object>.visit(DeclarationNode node) {
            // Declarations are done during semantic analysis    
            return null;
        }

        object IVisitor<object>.visit(AssignmentNode node) {
            SymbolTable.assign(node.getLeft().value, node.getRight().accept(new Evaluator()).ToString());    
            return null;
        }

        object IVisitor<object>.visit(ForLoopNode node) {
            Node ForControlNode = node.getLeft();
                        
            // Control variable initial assignment                        
            int control = Convert.ToInt32(ForControlNode.getLeft().accept(new Evaluator()));
            int times = Convert.ToInt32(ForControlNode.getRight().getRight().accept(new Evaluator()));

            for (int i = control; i <= times; i++)
            {
                SymbolTable.assign(ForControlNode.getLeft().getLeft().value, i.ToString());
                node.getRight().accept(this);
            }

            return null;
        }

        object IVisitor<object>.visit(ForControlNode node) { return null; }
        object IVisitor<object>.visit(ForConditionNode node) { return null; }

        object IVisitor<object>.visit(PrintNode node) {
            Node child = node.getLeft();
            Console.Write(child.accept(new Evaluator()));
            return null;
        }

        object IVisitor<object>.visit(ReadNode node) {
            Node child = node.getLeft();            
            Console.Write(">>> ");
            string value = Console.ReadLine();            
            if (child.type == Token.INT) {                
                try {
                    int value_i = int.Parse(value);
                    SymbolTable.assign(child.value, value_i.ToString());
                }
                catch {                    
                    new RuntimeError(child, "Runtime Error: Could not parse interger from the input.");
                }
            }
            else {
                SymbolTable.assign(child.value, value);
            }      
            return null;
        }

        object IVisitor<object>.visit(AssertNode node) {
            return node.accept(new Evaluator());
        }

        object IVisitor<object>.visit(BinOpNode node) { return null; }
        object IVisitor<object>.visit(UnOpNode node) { return null; }
        object IVisitor<object>.visit(IntNode node) { return null; }
        object IVisitor<object>.visit(StrNode node) { return null; }
        object IVisitor<object>.visit(BoolNode node) { return null; }
        object IVisitor<object>.visit(IdNode node) { return null; }        
        
 	}     
}