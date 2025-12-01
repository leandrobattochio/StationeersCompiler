namespace Compiler.Domain.IR;

public sealed record LoadFromDeviceInstruction(
    string Target,
    string Device,
    string Parameter
) : IrInstruction;
