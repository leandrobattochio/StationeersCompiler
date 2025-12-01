﻿using Compiler.Domain.Ast.Expressions;
using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Statements;

public sealed record IfStmt(Expr Condition, Stmt ThenBranch, Stmt? ElseBranch) : Stmt
{
    public override T Accept<T>(IStmtVisitor<T> visitor)
        => visitor.VisitIf(this);
}

