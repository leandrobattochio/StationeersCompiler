namespace Compiler.Lexer.Tokens;
public sealed record PlusEqualsToken(int Position) : Token(TokenType.PlusEquals, Position, "+=");


