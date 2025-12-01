namespace Compiler.Lexer.Tokens;

public sealed record DotToken(int Position) : Token(TokenType.Dot, Position, ".");

