using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public sealed record MemberAssignmentExpr(Expr Object, string MemberName, Expr Value) : Expr
{
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitMemberAssignment(this);
}

