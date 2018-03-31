using System;
using System.Collections.Generic;

namespace MiniPL {
	public class SemanticAnalyzer : IVisitor<object> {
				
		public Node ast;
		
		List<string[]> allowedOperations = new List<string[]>
		{
			new string[] { "+", Token.INT, Token.INT },			// Integer addition
			new string[] { "-", Token.INT, Token.INT },			// Integer substraction
			new string[] { "*", Token.INT, Token.INT },			// Integer multipication
			new string[] { "/", Token.INT, Token.INT },			// Integer division
			
			new string[] { "+", Token.STRING, Token.STRING }, 	// String concatenation

			new string[] { "&", Token.BOOL, Token.BOOL }, 		// Logical and
			new string[] { "!", Token.BOOL, null }, 			// Logical not

			new string[] { "=", Token.INT, Token.INT }, 		// Equality comparison
			new string[] { "<", Token.INT, Token.INT }, 		// Less-than concatenation			
			new string[] { "=", Token.STRING, Token.STRING }, 	// Equality comparison
			new string[] { "<", Token.STRING, Token.STRING }, 	// Less-than concatenation			
			new string[] { "=", Token.BOOL, Token.BOOL }, 		// Equality comparison
			new string[] { "<", Token.BOOL, Token.BOOL }, 		// Less-than concatenation			
		};
		
		List<string[]> allowedAssignments = new List<string[]>
		{
			new string[] {Token.INT, Token.INT },
			new string[] {Token.INT, Token.BOOL },
			new string[] {Token.STRING, Token.STRING },
			new string[] {Token.BOOL, Token.BOOL },
			new string[] {Token.BOOL, Token.INT }
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
			Node ForControlNode = node.getLeft();
			Node statements = node.getRight();
			ForControlNode.accept(this);
			statements.accept(this);			
			return 1;
		}

		object IVisitor<object>.visit(ForControlNode node) {			
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
			if (!(left != null & right != null)) { // Cannot proceed if there has been an error
				if (!left.type.Equals(Token.INT)) {				
					new SemanticError(left, "Semantic Error: Expected integer, got " + left.type);
				}
				if (!right.type.Equals(Token.INT)) {
					new SemanticError(right, "Semantic Error: Expected integer, got " + right.type);
				}
			}
			return null;
		}
        
        object IVisitor<object>.visit(PrintNode node) {
			node.getLeft().accept(this);
			return null;
		}

        object IVisitor<object>.visit(ReadNode node) { 
			node.getLeft().accept(this);
			return null;
		 }

		object IVisitor<object>.visit(AssertNode node) {
			Node child = node.getLeft();
			child.accept(this);
			return null;
		}

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

        object IVisitor<object>.visit(UnOpNode node) {
			Node child = node.getLeft();
			child.accept(this);			
			return null;
		}
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
			string type = SymbolTable.lookupType(node.value);
			string value = SymbolTable.lookup(node.value);			
			if (value != null) {				
				node.type = type;		
				return node.type;
			}			
			else {				
				new SemanticError(node, "Semantic Error: " + node.value + " does not exist in this context.");				
			}
			return null;
		}

		public void checkOperationIsAllowed(Node operation, Node left, Node right) {
			if (SymbolTable.lookup(left.value)  == null || SymbolTable.lookup(right.value)  == null) {
				return; // return as one of the children does not exist
			}
			bool allowed = false;
			foreach (var rule in allowedOperations) {
				if (operation.value.Equals(rule[0]) & left.type.Equals(rule[1]) & right.type.Equals(rule[2])) {
					allowed = true;
				}				
			}
			if (!allowed) {
				new SemanticError(operation, "Semantic Error: Operation not allowed: " + left.type +  " " + operation.value + " " + right.type);				
			}
		}

		public void checkSameType(Node left, Node right) {			
			if (SymbolTable.lookup(left.value)  == null || SymbolTable.lookup(right.value)  == null) {
				return; // return as one of the children does not exist
			}
			bool allowed = false;
			foreach (var rule in allowedAssignments) {
				if (left.type.Equals(rule[0]) & right.type.Equals(rule[1])) {
					allowed = true;
				}				
			}
			if (!allowed) {
				new SemanticError(right, "Semantic Error: Expected " + left.type + ", got " + right.type);			
			}
		}		
	}    
}