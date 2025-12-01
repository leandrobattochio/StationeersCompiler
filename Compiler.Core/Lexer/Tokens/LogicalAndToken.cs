namespace Compiler.Lexer.Tokens;

public record LogicalAndToken(int Position) : Token(TokenType.LogicalAnd, Position, "&&");

