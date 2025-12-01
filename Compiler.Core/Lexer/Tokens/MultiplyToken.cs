namespace Compiler.Lexer.Tokens;

public record MultiplyToken(int Position) : Token(TokenType.Multiply, Position, "*");