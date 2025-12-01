using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record LoadDeviceExpr(DeviceExpr Device, string Property) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitLoadDevice(this);
}

