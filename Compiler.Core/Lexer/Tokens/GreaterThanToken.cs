namespace Compiler.Lexer.Tokens;

public record GreaterThanToken(int Position) : Token(TokenType.GreaterThan, Position, ">");

