using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace MiniPL {

    public class Node {
        
        public Token token;
        public int id;
        public string value;
        public List<Node> children = new List<Node>();
        public static SymbolTable symbolTable = new SymbolTable();
        static int count = 0; // for mermaid

        public virtual int interpret() {
            foreach (Node child in children)
            {
                child.interpret();
            }
            return 1;
        }
    
        public Node() { }

        public Node(string value) {
            this.value = value;
            generateId();
        }

        public Node(Token token) {
            this.token = token;
            this.value = token.value;
            generateId();                 
        }       

        public void addChild(Node node) {
            children.Add(node);
        }

        public Node getLeft() {
            return children[0];
        }

        public Node getRight() {
            return children[1];
        }

        protected void generateId() {
            count++;
            this.id = count;
        }

        public void print(string indent, bool last) {

            Console.Write(indent);
            if (last) {
                Console.Write("\\-");
                indent += "  ";
            }
            else {
                Console.Write("|-");
                indent += "| ";
            }

            Console.WriteLine(this + ", INTERPRET: " + this.interpret());

            for (int i = 0; i < children.Count; i++)
                children[i].print(indent, i == children.Count - 1);
        }

        public void mermaid(string indent, bool last, bool first) {            

            Console.Write(indent);
            if (!first) {
                Console.Write("-->");
                
            }
            else {
                Console.WriteLine("graph TD");
            }

            Console.WriteLine(id + "[" + this + "]");

            for (int i = 0; i < children.Count; i++) {
                Console.Write(id);
                children[i].mermaid(indent, i == children.Count - 1, false);
            }
        }

        public override string ToString() {
            return this.GetType().Name + " -> " + this.value;
        }
    }

    // Program
    public class ProgramNode : Node {
        public ProgramNode() {            
            generateId();
        }        

    }

    // Statement
    public class StatementNode : Node {
        public StatementNode() {            
            generateId();
        }
    }

    // For Loop
    public class ForLoopNode : Node {
        public ForLoopNode() {            
            generateId();
        }

        public override int interpret() {            
            Node controlNode = getLeft();
            
            // Control variable initial assignment
            int control = controlNode.getLeft().interpret();
            
            do {                                            
                // Interpret                
                getRight().interpret();
                control++;
                symbolTable.assign(controlNode.getLeft().getLeft().value, control.ToString());
                                
            } while (controlNode.getRight().interpret() != 0);
            return 1;
        }
    }

    // Control
    public class ControlNode : Node {
        public ControlNode() {                  
            generateId();
        }        
        public override int interpret() {
            return 0;
        }
    }

    // Assert
    public class AssertNode : Node {
        public AssertNode() {                  
            generateId();
        }        
        public override int interpret() {
            if (getLeft().interpret() != 0) {
                // Assertion true
                return 1;
            }
            // Assertion false            
            return 0;
        }
    }


    // Print
    public class PrintNode : Node {
        public PrintNode() {            
            generateId();
        }
        public override int interpret() {
            // System call
            Console.WriteLine(this.getLeft().interpret());
            return 1;
        }
    }    
    
    // Read
    public class ReadNode : Node {
        public ReadNode() {            
            generateId();
        }
        public override int interpret() {            
            Console.Write(">>> ");
            int value = Convert.ToInt32(Console.ReadLine());
            symbolTable.assign(getLeft().value, value.ToString());
            return value;
        }
    }

    // Declaration
    public class DeclarationNode : Node {
        public DeclarationNode() {            
            generateId();
        }

        public override int interpret() {
            symbolTable.declare(getLeft().value);        
            return 1;
        }
    }

    // Assignment
    public class AssignmentNode : Node {
        public AssignmentNode() {            
            generateId();
        }

        public override int interpret() {
            symbolTable.assign(getLeft().value, getRight().interpret().ToString());            
            return getRight().interpret();
        }
    }

    // Integer
    public class IntNode : Node {
        public IntNode(Token token) {
            this.value = token.value;            
            generateId();
        }        
        public override int interpret() {
            return Int32.Parse(this.value);
        }
    }

    // String
    public class StrNode : Node {
        public StrNode(Token token) {
            this.value = token.value;            
            generateId();
        }
    }

    // Identifier
    public class IdNode : Node {
        public IdNode(Token token) {
            this.value = token.value;            
            generateId();
        }

        public override int interpret() {            
            return symbolTable.lookup(this.value);
        }
    }

    // Operations

    // Binary operation
    public class BinOpNode : Node {
        public BinOpNode(string operand) {
            this.value = operand;
            generateId();
        }

        public BinOpNode(Token token) {
            this.value = token.value;
            generateId();
        }

        public override int interpret() {
            switch (this.value)
            {
                case "*":
                    return this.getLeft().interpret() * this.getRight().interpret();
                case "/":
                    return this.getLeft().interpret() / this.getRight().interpret();
                case "+":
                    return this.getLeft().interpret() + this.getRight().interpret();
                case "-":
                    return this.getLeft().interpret() - this.getRight().interpret();
                case "=":
                    if (this.getLeft().interpret() == this.getRight().interpret()) {
                        return 1;
                    }
                    else return 0;
                case "<":                    
                    if (this.getLeft().interpret() < this.getRight().interpret()) {
                        return 1;
                    }
                    else return 0;
                default:
                    break;
            }
            return 0;
        }
    }
}