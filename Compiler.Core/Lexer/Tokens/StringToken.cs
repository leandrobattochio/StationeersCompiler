namespace Compiler.Lexer.Tokens;

public record StringToken(string Value, int Position) : Token(TokenType.String, Position, Value);