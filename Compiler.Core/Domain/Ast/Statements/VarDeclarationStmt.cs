using Compiler.Domain.Ast.Expressions;
using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Statements;

public sealed record VarDeclarationStmt(string Name, Expr Initializer, string? ExplicitType = null) : Stmt
{
    public override T Accept<T>(IStmtVisitor<T> visitor)
        => visitor.VisitVarDeclaration(this);
}

