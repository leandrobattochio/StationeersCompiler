using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record NumberExpr(int Value) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitNumber(this);
}

