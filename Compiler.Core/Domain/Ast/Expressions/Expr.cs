using Compiler.Domain.Ast.Visitors;

namespace Compiler.Domain.Ast.Expressions;

public abstract record Expr
{
    public abstract T Accept<T>(IExprVisitor<T> visitor);
}


