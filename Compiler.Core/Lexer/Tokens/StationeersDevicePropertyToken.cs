namespace Compiler.Lexer.Tokens;

public record StationeersDevicePropertyToken(string Value, int Position) : Token(TokenType.DeviceProperty, Position, Value);

