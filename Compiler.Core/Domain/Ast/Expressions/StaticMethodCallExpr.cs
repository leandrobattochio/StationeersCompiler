using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

/// <summary>
/// Represents a static method call on a global object (e.g., Math.convertToCelsius(...))
/// </summary>
public sealed record StaticMethodCallExpr(string ObjectName, string MethodName, List<Expr> Arguments) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitStaticMethodCall(this);
}

