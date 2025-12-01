namespace Compiler.Lexer.Tokens;

public record LeftParentToken(int Pos) : Token(TokenType.LParen, Pos, "(");