namespace Compiler.Lexer.Tokens;

public record StationeerConstantToken(string Value, int Position)
    : Token(TokenType.StationeerConstant, Position, Value);