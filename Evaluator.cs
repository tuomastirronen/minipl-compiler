using System;

namespace MiniPL {
    public class Evaluator : IVisitor<object> {                

        public object evaluate(Node node) {
            return node.accept(this);
        }
        object IVisitor<object>.visit(ProgramNode node) { return 1; }

        object IVisitor<object>.visit(StatementsNode node) { return 1; }

        object IVisitor<object>.visit(StatementNode node) { return 1; }

        object IVisitor<object>.visit(DeclarationNode node) { return 1; }
        object IVisitor<object>.visit(AssignmentNode node) { return 1; }

        object IVisitor<object>.visit(ForLoopNode node) { return 1; }
        object IVisitor<object>.visit(ControlNode node) { return 1; }
        object IVisitor<object>.visit(ForConditionNode node) { return 1; }
        
        object IVisitor<object>.visit(PrintNode node) { return 1; }

        object IVisitor<object>.visit(ReadNode node) { return 1; }


        object IVisitor<object>.visit(AssertNode node) {
            if (Convert.ToBoolean(evaluate(node.getLeft()))) {
                Console.WriteLine("Assertion succeeded");
                return true;
            }
            Console.WriteLine("Assertion failed");
            return false;
        }

        object IVisitor<object>.visit(UnOpNode node) {
            switch (node.value)
            {
                case "!":                    
                    if (Convert.ToInt32(evaluate(node.getLeft())) > 0) {
                        return 0;
                    }
                    else return 1;                    
                default:
                    break;
            }            
            return null;
        }

        object IVisitor<object>.visit(BinOpNode node) {
            object left = evaluate(node.getLeft());
            object right = evaluate(node.getRight());
            
            switch (node.value)
            {
                case "+":
                    // concatenation
                    if (left is string || right is string) {
                        return left.ToString() + right.ToString();
                    }
                    else {
                        return Convert.ToInt32(left) + Convert.ToInt32(right);
                    }
                    
                case "-":
                    return Convert.ToInt32(left) - Convert.ToInt32(right);
                case "*":
                    return Convert.ToInt32(left) * Convert.ToInt32(right);
                case "/":
                    return Convert.ToInt32(left) / Convert.ToInt32(right);
                case "=":
                    return Convert.ToInt32(left) == Convert.ToInt32(right);
                case "<":                    
                    return Convert.ToInt32(left) < Convert.ToInt32(right);
                case "&":
                    return Convert.ToBoolean(left) & Convert.ToBoolean(right);
                default:
                    break;                
            }
            return null;     
        }
        object IVisitor<object>.visit(IntNode node) {            
            return Convert.ToInt32(node.value);
        }
        
        object IVisitor<object>.visit(StrNode node) {            
            return node.value;
        }

        object IVisitor<object>.visit(BoolNode node) {            
            return Convert.ToInt32(node.value);
        }

        object IVisitor<object>.visit(IdNode node) {            
            return Convert.ToInt32(SymbolTable.lookup(node.value));
        }    
    }
}