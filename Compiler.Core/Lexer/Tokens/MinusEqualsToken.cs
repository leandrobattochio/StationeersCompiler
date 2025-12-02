namespace Compiler.Lexer.Tokens;

public sealed record MinusEqualsToken(int Position) : Token(TokenType.MinusEquals, Position, "-=");

