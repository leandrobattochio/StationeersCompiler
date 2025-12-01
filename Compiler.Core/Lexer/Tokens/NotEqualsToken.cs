namespace Compiler.Lexer.Tokens;

public record NotEqualsToken(int Position) : Token(TokenType.NotEquals, Position, "!=");

