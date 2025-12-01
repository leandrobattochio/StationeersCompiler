using Compiler.Domain.Ast.Expressions;
using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Statements;

public sealed record ExprStmt(Expr Expression) : Stmt
{
    public override T Accept<T>(IStmtVisitor<T> visitor)
        => visitor.VisitExprStmt(this);
}

