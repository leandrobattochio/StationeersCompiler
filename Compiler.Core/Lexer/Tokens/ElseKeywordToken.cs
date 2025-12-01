namespace Compiler.Lexer.Tokens;

public record ElseKeywordToken(int Position) : Token(TokenType.Keyword, Position, "else");

