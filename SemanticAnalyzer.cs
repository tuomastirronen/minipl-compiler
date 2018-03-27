using System;

namespace MiniPL {
	public class SemanticAnalyzer {

		public Node ast;
		public static SymbolTable symbolTable = new SymbolTable();

		public SemanticAnalyzer(Node ast) {
	        this.ast = ast;
	    }

        public void analyze() {
            // Console.WriteLine("hello from analyzer");
			foreach (Node child in ast.children)
			{
				visit(child);
			}
        }

		public void visit(Node node) {
			foreach (Node child in node.children)
			{
				if (child is BinOpNode) {
					analyzeBinOp(child);
				}
				else if (child is AssignmentNode) {
					analyzeAssignment(child);
				}
				else if (child is DeclarationNode) {
					SymbolTable.declare(child.getLeft().value, child.getRight().value);
				}
				visit(child);
			}
		}

		public void analyze(StatementNode node) {
			foreach (Node child in node.children)
			{
				Console.WriteLine(child);
			}
		}

		public void analyzeAssignment(Node node) {
			Console.WriteLine(node + " " + node.getLeft().value + node.getRight().value);
			if (SymbolTable.lookupType(node.getLeft().value) != node.getRight().type) {
				Console.WriteLine("Type mismatch");
			}
		}

		public void analyzeBinOp(Node node) {
			switch (node.value)
			{
				case "+":
					checkSameNumericType(node.getLeft(), node.getRight());
					break;
				case "-":
					checkSameNumericType(node.getLeft(), node.getRight());
					break;
				case "*":
					checkSameNumericType(node.getLeft(), node.getRight());
					break;
				case "/":
					checkSameNumericType(node.getLeft(), node.getRight());
					break;
				default:
					break;
			}
		}

		public void checkSameNumericType(Node left, Node right) {
			if (left.GetType() != right.GetType()) {
				Console.WriteLine("ERROR!!");
			}
		}
	}    
}