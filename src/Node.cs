using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace MiniPL {

    public abstract class Node {
        
        public Token token;
        public int id;
        public string value;
        public string type;
        public List<Node> children = new List<Node>();        
        static int count = 0; // for mermaid
        public int row;
        public int col;
        
        public Node() { }

        public Node(string value) {
            this.value = value;
            generateId();
        }

        public Node(Token token) {
            this.token = token;
            this.value = token.value;
            this.row = token.row;
            this.col = token.col;
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

        public abstract R accept<R>(IVisitor<R> visitor);

        public bool hasStringChild() {            
            foreach (Node child in children) {                
                if (child is StrNode) {
                    return true;
                }
                else if (child is IdNode) {
                    if (SymbolTable.lookupType(child.value) == Token.STRING) {
                        return true;
                    }                    
                }                
            }
            return false;

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

            Console.WriteLine(this + ", INTERPRET: ");

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
            if (this.value != null) {
                return this.GetType().Name + "->" + this.value;
            }            
            return this.GetType().Name;
        }
    }

    // Program
    public class ProgramNode : Node {
        public ProgramNode() {            
            generateId();
        }      

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);            
        }  

    }
    
    // Statement
    public class BlockNode : Node {
        public BlockNode() {            
            generateId();
        }

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);            
        }
    }

    // Statement
    public class StatementNode : Node {
        public StatementNode() {            
            generateId();
        }

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);            
        }
    }

    // For Loop
    public class ForLoopNode : Node {
        public ForLoopNode() {            
            generateId();
        }      

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);
        }
    }

    // Control
    public class ControlNode : Node {
        public ControlNode() {                  
            generateId();
        }

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);            
        }
    }

    // Condition
    public class ForConditionNode : Node {
        public ForConditionNode() {                  
            generateId();
        }   

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);            
        }     
    }

    // Assert
    public class AssertNode : Node {
        public AssertNode() {                  
            generateId();
        }   

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);            
        }     
    }


    // Print
    public class PrintNode : Node {
        public PrintNode() {            
            generateId();
        }

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);            
        }
    }    
    
    // Read
    public class ReadNode : Node {
        public ReadNode() {            
            generateId();
        }

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);
        }

    }

    // Declaration
    public class DeclarationNode : Node {
        public DeclarationNode() {            
            generateId();
        }

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);        
        }

    }

    // Assignment
    public class AssignmentNode : Node {
        public AssignmentNode() {            
            generateId();
        }
        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);            
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
            this.row = token.row;
            this.col = token.col;
            generateId();
        }

        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);
        }

    }

    // Unary operation
    public class UnOpNode : Node {
        public UnOpNode(Token token) {
            this.value = token.value;
            this.row = token.row;
            this.col = token.col;
            generateId();
        }
        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);
        }

    }

    // Terminals
    // Integer
    public class IntNode : Node {
        public IntNode(Token token) {
            this.value = token.value;
            this.type = token.type;
            this.row = token.row;
            this.col = token.col;
            generateId();
        }
        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);
        }
 
    }

    // String
    public class StrNode : Node {
        public StrNode(Token token) {
            this.value = token.value;
            this.type = token.type;
            this.row = token.row;
            this.col = token.col;
            generateId();
        }
        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);
        }
    }

    public class BoolNode : Node {
        public BoolNode(Token token) {
            this.value = token.value;
            this.type = token.type;
            this.row = token.row;
            this.col = token.col;
            generateId();
        }
        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);
        }
    }

    // Identifier
    public class IdNode : Node {        
        public IdNode(Token token) {
            this.value = token.value;
            this.row = token.row;
            this.col = token.col;
            generateId();
        }
        public override R accept<R>(IVisitor<R> visitor) {
            return visitor.visit(this);
        }

    }
    
}