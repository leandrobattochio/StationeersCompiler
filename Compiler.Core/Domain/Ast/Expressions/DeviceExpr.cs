using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record DeviceExpr(string DeviceName, string PropertyName) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitDevice(this);
}

