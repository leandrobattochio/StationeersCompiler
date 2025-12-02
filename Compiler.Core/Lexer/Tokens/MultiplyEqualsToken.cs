namespace Compiler.Lexer.Tokens;

public sealed record MultiplyEqualsToken(int Position) : Token(TokenType.MultiplyEquals, Position, "*=");

