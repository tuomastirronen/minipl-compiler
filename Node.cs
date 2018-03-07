using System;
using System.Linq;
using System.Text;

namespace MiniPL {

    public class Node {
        
        public Token token;
        public Node left;
        public Node right;
        
        public Node(Token token, Node left = null, Node right = null) {            
            this.token = token;
            this.right = right;
            this.left = left;            
        }

        public override string ToString() {
            return this.token.lexeme;
        }

        public string displayNode()
        {
            StringBuilder output = new StringBuilder();
            displayNode(output, 0);
            return output.ToString();
        }

        private void displayNode(StringBuilder output, int depth)
        {            
            if (right != null) {            
                right.displayNode(output, depth+1);
            }                
            output.Append('\t', depth);
            output.AppendLine(token.lexeme);


            if (left != null) {
                left.displayNode(output, depth+1);
            }                
        }
    }
}