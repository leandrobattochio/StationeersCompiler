namespace Compiler.Lexer.Tokens;

public record GreaterThanOrEqualToken(int Position) : Token(TokenType.GreaterThanOrEqual, Position, ">=");

