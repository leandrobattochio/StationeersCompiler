namespace Compiler.Domain.IR;
public sealed record CompareInstruction(string Target, string Left, string Right, string Op) : IrInstruction;


