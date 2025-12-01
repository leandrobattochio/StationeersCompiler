using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record IdentifierExpr(string Name) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitIdentifier(this);
}

