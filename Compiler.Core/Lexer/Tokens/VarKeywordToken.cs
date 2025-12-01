namespace Compiler.Lexer.Tokens;

public record VarKeywordToken(int Position) : Token(TokenType.Keyword, Position, "var");
