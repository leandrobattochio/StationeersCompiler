namespace Compiler.Lexer.Tokens;

public record IdentifierToken(string Value, int Position) : Token(TokenType.Identifier, Position, Value);