using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record CallExpr(Expr Callee, List<Expr> Arguments) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitCall(this);
}

