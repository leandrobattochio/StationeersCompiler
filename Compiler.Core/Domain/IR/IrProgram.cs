namespace Compiler.Domain.IR;

public sealed class IrProgram
{
    public List<IrInstruction> Instructions { get; }
    public string ResultTemp { get; }

    public IrProgram(List<IrInstruction> instructions, string resultTemp)
    {
        Instructions = instructions;
        ResultTemp = resultTemp;
    }
}
