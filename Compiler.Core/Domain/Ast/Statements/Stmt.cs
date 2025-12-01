using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Statements;

public abstract record Stmt
{
    public abstract T Accept<T>(IStmtVisitor<T> visitor);
}

