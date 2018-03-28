using System;

namespace MiniPL {
    public interface IVisitor<R> {
        
        R visit(ProgramNode node);
        R visit(StatementsNode node);
        R visit(StatementNode node);
        R visit(DeclarationNode node);
        R visit(AssignmentNode node);
        R visit(ForLoopNode node);
        R visit(ControlNode node);
        R visit(ForConditionNode node);
        
        R visit(AssertNode node);
        R visit(PrintNode node);
        R visit(ReadNode node);
        R visit(UnOpNode node);

        R visit(BinOpNode node);
        R visit(IntNode node);
        R visit(StrNode node);
        R visit(BoolNode node);
        R visit(IdNode node);
    }
    
}