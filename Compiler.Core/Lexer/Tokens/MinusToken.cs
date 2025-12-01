namespace Compiler.Lexer.Tokens;

public record MinusToken(int Position) : Token(TokenType.Minus, Position, "-");