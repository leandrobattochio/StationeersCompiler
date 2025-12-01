using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

/// <summary>
/// Represents an instance method call on an object (e.g., device.setOn())
/// </summary>
public sealed record MethodCallExpr(Expr Object, string MethodName, List<Expr> Arguments) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitMethodCall(this);
}

