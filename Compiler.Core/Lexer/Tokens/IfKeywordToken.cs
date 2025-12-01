namespace Compiler.Lexer.Tokens;
public record IfKeywordToken(int Position) : Token(TokenType.Keyword, Position, "if");


