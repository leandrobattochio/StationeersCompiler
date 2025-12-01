namespace Compiler.Domain.IR;

public sealed record LoadDeviceInstruction(
    string Target,
    string Device,
    string Property
) : IrInstruction;
