using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record IncrementDecrementExpr(string Name, string Op, bool IsPrefix) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitIncrementDecrement(this);
}

