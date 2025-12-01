namespace Compiler.Lexer.Tokens;

public record EqualsEqualsToken(int Position) : Token(TokenType.EqualsEquals, Position, "==");

