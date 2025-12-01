namespace Compiler.Domain.IR;

public sealed record JumpInstruction(string Label) : IrInstruction;

