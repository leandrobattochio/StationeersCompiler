namespace Compiler.Lexer.Tokens;

public record EndOfFileToken(int Position) : Token(TokenType.EndOfFile, Position, "");