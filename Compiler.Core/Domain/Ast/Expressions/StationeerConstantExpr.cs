using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record StationeerConstantExpr(string Value) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitStationeerConstant(this);
}