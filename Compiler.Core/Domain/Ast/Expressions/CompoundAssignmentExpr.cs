using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record CompoundAssignmentExpr(string Name, string Op, Expr Value) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitCompoundAssignment(this);
}

