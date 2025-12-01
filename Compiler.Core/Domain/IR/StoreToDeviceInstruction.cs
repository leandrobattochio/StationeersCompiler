namespace Compiler.Domain.IR;

public sealed record StoreToDeviceInstruction(string Device, string Property, string Value) : IrInstruction;