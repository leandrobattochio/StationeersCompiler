namespace Compiler.Domain.IR;

public sealed record LabelInstruction(string Label) : IrInstruction;

