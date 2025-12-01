namespace Compiler.Lexer.Tokens;

public record RightParentToken(int Pos) : Token(TokenType.RParen, Pos, ")");