namespace Compiler.Lexer.Tokens;

public record NumberToken(string Value, int Position) : Token(TokenType.Number, Position, Value);