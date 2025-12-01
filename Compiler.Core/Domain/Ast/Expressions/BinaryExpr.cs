using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record BinaryExpr(Expr Left, string Op, Expr Right) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitBinary(this);
}

