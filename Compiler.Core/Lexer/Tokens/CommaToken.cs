namespace Compiler.Lexer.Tokens;

public record CommaToken(int Position) : Token(TokenType.Comma, Position, ",");

