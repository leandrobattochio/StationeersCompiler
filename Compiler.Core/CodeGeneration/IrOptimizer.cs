using Compiler.CodeGeneration.Strategies;
using Compiler.Domain.IR;

namespace Compiler.CodeGeneration;

public class IrOptimizer
{
    private readonly List<IOptimizationStrategy> _strategies =
    [
        new ConstantFoldingOptimize(),
        new BinOpMoveOptimize(),
    ];


    public IrProgram Optimize(IrProgram program)
    {
        return _strategies.Aggregate(program, (current, strategy) => strategy.Apply(current));
    }
}