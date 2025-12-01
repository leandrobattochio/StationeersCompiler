namespace Compiler.Lexer.Tokens;

public record FloatToken(string Value, int Position) : Token(TokenType.Float, Position, Value);

