using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record StringExpr(string Value) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitString(this);
}