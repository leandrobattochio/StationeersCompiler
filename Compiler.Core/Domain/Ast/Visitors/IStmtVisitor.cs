﻿using Compiler.Domain.Ast.Statements;

namespace Compiler.Domain.Ast.Visitors;

public interface IStmtVisitor<T>
{
    T VisitVarDeclaration(VarDeclarationStmt stmt);
    T VisitExprStmt(ExprStmt stmt);
    T VisitIf(IfStmt stmt);
    T VisitBlock(BlockStmt stmt);
}

