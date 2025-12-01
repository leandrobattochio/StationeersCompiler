namespace Compiler.Lexer.Tokens;

public record PlusToken(int Position) : Token(TokenType.Plus, Position, "+");