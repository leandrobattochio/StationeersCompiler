using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record GroupExpr(Expr Inner) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitGroup(this);
}

