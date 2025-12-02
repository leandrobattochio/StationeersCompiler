namespace Compiler.Lexer.Tokens;

public sealed record PlusPlusToken(int Position) : Token(TokenType.PlusPlus, Position, "++");

