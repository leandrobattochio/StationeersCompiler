namespace Compiler.Lexer.Tokens;

public record LessThanToken(int Position) : Token(TokenType.LessThan, Position, "<");

