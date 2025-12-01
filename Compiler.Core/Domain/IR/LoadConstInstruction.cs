namespace Compiler.Domain.IR;

public sealed record LoadConstInstruction(string Target, object Value) : IrInstruction;
