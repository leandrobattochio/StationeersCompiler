namespace Compiler.Lexer.Tokens;

public record FalseKeywordToken(int Position) : Token(TokenType.FalseKeyword, Position, "true");