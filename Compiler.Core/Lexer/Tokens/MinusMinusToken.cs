namespace Compiler.Lexer.Tokens;

public sealed record MinusMinusToken(int Position) : Token(TokenType.MinusMinus, Position, "--");

