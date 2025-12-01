namespace Compiler.Lexer.Tokens;

public record LexerErrorToken(string Message, int Position)
    : Token(TokenType.CompileError, Position, Message);