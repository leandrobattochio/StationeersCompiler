namespace Compiler.Lexer.Tokens;

public sealed record DivideEqualsToken(int Position) : Token(TokenType.DivideEquals, Position, "/=");

