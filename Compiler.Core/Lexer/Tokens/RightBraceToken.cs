namespace Compiler.Lexer.Tokens;

public record RightBraceToken(int Position) : Token(TokenType.RBrace, Position, "}");

