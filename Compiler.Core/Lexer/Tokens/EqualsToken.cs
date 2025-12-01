namespace Compiler.Lexer.Tokens;

public record EqualsToken(int Position) : Token(TokenType.Equals, Position, "=");