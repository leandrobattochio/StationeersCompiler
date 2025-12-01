namespace Compiler.Lexer.Tokens;

public record LogicalOrToken(int Position) : Token(TokenType.LogicalOr, Position, "||");

