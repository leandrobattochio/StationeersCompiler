namespace Compiler.Lexer.Tokens;

public record LeftBraceToken(int Position) : Token(TokenType.LBrace, Position, "{");

