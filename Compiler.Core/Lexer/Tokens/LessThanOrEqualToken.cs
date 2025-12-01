namespace Compiler.Lexer.Tokens;

public record LessThanOrEqualToken(int Position) : Token(TokenType.LessThanOrEqual, Position, "<=");

