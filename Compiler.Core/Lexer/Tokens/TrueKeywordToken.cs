namespace Compiler.Lexer.Tokens;

public record TrueKeywordToken(int Position) : Token(TokenType.TrueKeyword, Position, "true");