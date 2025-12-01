namespace Compiler.Domain.IR;

public sealed record BranchIfZeroInstruction(string Condition, string Label) : IrInstruction;

