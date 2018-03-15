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
        static int count = 0;
        
        public Node() {
            this.token = null;
            generateId();
        }

        public Node(string value) {
            this.value = value;
            generateId();
        }

        public Node(Token token) {
            this.token = token;
            this.value = token.value;
            generateId();                 
        }

        private void generateId() {
            count++;
            this.id = count;
        }

        public void addChild(Node node) {
            children.Add(node);
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

            Console.WriteLine(value);

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

            Console.WriteLine(id + "(" + value + ")");

            for (int i = 0; i < children.Count; i++) {
                Console.Write(id);
                children[i].mermaid(indent, i == children.Count - 1, false);
            }
        }

        public override string ToString() {
            return this.value;
        }
    }    
}