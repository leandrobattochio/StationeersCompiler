namespace Compiler.Lexer.Tokens;

public record SemicolonToken(int Position) : Token(TokenType.Semicolon, Position, ";");