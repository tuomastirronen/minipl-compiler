using System;
using System.Collections.Generic;

namespace MiniPL {
	public class SemanticAnalyzer : IVisitor<object> {
				
		public Node ast;
		
		List<string[]> allowedOperations = new List<string[]>
		{
			new string[] { "+", Token.INT, Token.INT },			// Integer addition
			new string[] { "+", Token.STRING, Token.INT }, 		// Concatenate string and integer
			new string[] { "+", Token.INT, Token.STRING },		// Concatenate integer and string
			new string[] { "+", Token.STRING, Token.STRING }, 	// Concatenate strings
			new string[] { "-", Token.INT, Token.INT },			// Integer substraction
			new string[] { "*", Token.INT, Token.INT },			// Integer multiplication
			new string[] { "/", Token.INT, Token.INT }			// Integer division
		};
		
		public SemanticAnalyzer(Node ast) {
	        this.ast = ast;
	    }

        public void analyze() {            
			ast.accept(this);
        }

		object IVisitor<object>.visit(ProgramNode node) {            
            foreach (var child in node.children) {
                child.accept(this);
            }            
            return null;
        }

        object IVisitor<object>.visit(StatementsNode node) {            
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
			SymbolTable.declare(node.getLeft().value, node.getLeft().type);    
			return null;
		}
        object IVisitor<object>.visit(AssignmentNode node) {
			Node left = node.getLeft();
			Node right = node.getRight();
			
			left.accept(this);
			right.accept(this);
            			
			checkSameType(left, right);			
			return null;
		}

        object IVisitor<object>.visit(ForLoopNode node) {
			Node controlNode = node.getLeft();
			Node statements = node.getRight();
			controlNode.accept(this);
			statements.accept(this);			
			return 1;
		}

		object IVisitor<object>.visit(ControlNode node) {			
			Node assignment = node.getLeft();
			Node conditionNode = node.getRight();

			// Analyse assignment
			assignment.accept(this);
			conditionNode.accept(this);

			return null;
		}

		object IVisitor<object>.visit(ForConditionNode node) {			

			Node left = node.getLeft();
			Node right = node.getRight();
			left.accept(this);
			right.accept(this);

			// Only allow integers in range
			if (!left.type.Equals(Token.INT)) {				
				new SemanticError(left, "Semantic Error: Expected integer, got " + left.type);
			}
			if (!right.type.Equals(Token.INT)) {
				new SemanticError(right, "Semantic Error: Expected integer, got " + right.type);				
			}
			return null;
		}
        
        object IVisitor<object>.visit(PrintNode node) {
			node.getLeft().accept(this);
			return null;
		}

        object IVisitor<object>.visit(ReadNode node) { return 1; }

		object IVisitor<object>.visit(AssertNode node) { return null; }

        object IVisitor<object>.visit(BinOpNode node) {
			Node left = node.getLeft();
			Node right = node.getRight();	

			// decorate
			left.type = Convert.ToString(left.accept(this));			
			right.type = Convert.ToString(right.accept(this));
			
			
			if (left.type == Token.INT && right.type == Token.INT) {
				node.type = Token.INT;
			}
			else if (left.type == Token.STRING || right.type == Token.STRING) {
				node.type = Token.STRING;				
			}
			else if (left.type == Token.BOOL || right.type == Token.BOOL) {
				node.type = Token.BOOL;
			}			
						
			checkOperationIsAllowed(node, left, right);
			return node.type;
			
		}

        object IVisitor<object>.visit(UnOpNode node) { return null; }
        object IVisitor<object>.visit(IntNode node) {
			node.type = Token.INT;
			return node.type;
		}
        object IVisitor<object>.visit(StrNode node) {
			node.type = Token.STRING;
			return node.type;
		}
        object IVisitor<object>.visit(BoolNode node) {
			node.type = Token.BOOL;			
			return node.type;
		}
        object IVisitor<object>.visit(IdNode node) {
			node.type = SymbolTable.lookupType(node.value);			
			return node.type;
		}

		public void checkOperationIsAllowed(Node operation, Node left, Node right) {
			bool allowed = false;
			foreach (var rule in allowedOperations) {
				if (operation.value.Equals(rule[0]) & left.type.Equals(rule[1]) & right.type.Equals(rule[2])) {
					allowed = true;
				}				
			}
			if (!allowed) {
				new SemanticError(operation, "Operation not allowed: " + left.type +  " " + operation.value + " " + right.type);				
			}
		}

		public void checkSameType(Node left, Node right) {
			if (!left.type.Equals(right.type)) {
				new SemanticError(right, "Semantic Error: Expected " + left.type + ", got " + right.type);				
			}
		}		
	}    
}