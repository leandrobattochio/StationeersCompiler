namespace Compiler.Domain.IR;

public sealed record LogicalOrInstruction(string Target, string Left, string Right) : IrInstruction;

