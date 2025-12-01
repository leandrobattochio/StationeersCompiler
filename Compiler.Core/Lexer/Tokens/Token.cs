namespace Compiler.Lexer.Tokens;

public abstract record Token(TokenType Type, int Position, string Value);