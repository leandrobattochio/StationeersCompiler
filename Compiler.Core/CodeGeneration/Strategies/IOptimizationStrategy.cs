using Compiler.Domain.IR;

namespace Compiler.CodeGeneration.Strategies;

public interface IOptimizationStrategy
{
    IrProgram Apply(IrProgram program);
}