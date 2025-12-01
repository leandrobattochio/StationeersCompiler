namespace Compiler.Domain.IR;

public sealed record BinaryOpInstruction(
    string Target,
    string Left,
    string Right,
    string Op
) : IrInstruction;
