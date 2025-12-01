using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record DevicePropertyExpr(string PropertyName) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitDeviceProperty(this);
}

