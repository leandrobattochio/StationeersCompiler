using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Statements;

public sealed record BlockStmt(List<Stmt> Statements) : Stmt
{
    public override T Accept<T>(IStmtVisitor<T> visitor)
        => visitor.VisitBlock(this);
}

