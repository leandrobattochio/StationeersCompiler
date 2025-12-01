using Compiler.Domain.IR;

namespace Compiler.CodeGeneration;

public class IrOptimizer
{
    public IrProgram Optimize(IrProgram program)
    {
        // If one load instruction storing to tX, followed by a binary operation and then a move the result to tX again,
        // remove the move instruction and change the binary operation to store directly to tX.
        for (var i = 0; i < program.Instructions.Count - 2; i++)
        {
            if (program.Instructions[i] is LoadFromDeviceInstruction loadInstr &&
                program.Instructions[i + 1] is BinaryOpInstruction binOpInstr &&
                program.Instructions[i + 2] is MoveInstruction moveInstr)
            {
                if (loadInstr.Target == binOpInstr.Left &&
                    moveInstr.Source == binOpInstr.Target &&
                    moveInstr.Target == loadInstr.Target)
                {
                    program.Instructions[i + 1] = new BinaryOpInstruction(loadInstr.Target, binOpInstr.Left,
                        binOpInstr.Right, binOpInstr.Op);
                    // Remove the move instruction
                    program.Instructions.RemoveAt(i + 2);
                }
            }
        }

        return program;
    }
}