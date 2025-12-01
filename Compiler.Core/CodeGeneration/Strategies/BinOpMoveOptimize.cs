using Compiler.Domain.IR;

namespace Compiler.CodeGeneration.Strategies;

public class BinOpMoveOptimize : IOptimizationStrategy
{
    public IrProgram Apply(IrProgram program)
    {
        for (var i = 0; i < program.Instructions.Count - 1; i++)
        {
            if (program.Instructions[i] is BinaryOpInstruction binOpInstr &&
                program.Instructions[i + 1] is MoveInstruction moveInstr)
            {
                // cmd target left right
                // sub r2 r0 2733.15

                // cmd 
                // mov r0 r2

                if (binOpInstr.Target == moveInstr.Source && binOpInstr.Left == moveInstr.Target)
                {
                    program.Instructions[i] = new BinaryOpInstruction(binOpInstr.Left, binOpInstr.Left,
                        binOpInstr.Right, binOpInstr.Op);

                    // Remove the move instruction
                    program.Instructions.RemoveAt(i + 1);
                }
            }
        }

        return program;
    }
}