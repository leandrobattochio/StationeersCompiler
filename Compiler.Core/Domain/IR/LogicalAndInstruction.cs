namespace Compiler.Domain.IR;

public sealed record LogicalAndInstruction(string Target, string Left, string Right) : IrInstruction;

