using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

/// <summary>
/// Represents member access expression (e.g., device.Temperature)
/// </summary>
public sealed record MemberAccessExpr(Expr Object, string MemberName) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitMemberAccess(this);
}

