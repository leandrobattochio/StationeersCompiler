namespace Compiler.Lexer.Tokens;

public record DivideToken(int Position) : Token(TokenType.Divide, Position, "/");