namespace Compiler.Domain.IR;

public sealed record MoveInstruction(string Target, string Source) : IrInstruction;

