using Compiler.Domain.IR;

namespace Compiler.CodeGeneration.Strategies;

/// <summary>
/// Optimizes binary operations by using constant values directly instead of loading them into temporary registers.
/// Example:
///   Before: mov t2 20
///           sub t3 r0 t2
///           mov r0 t3
///   After:  sub r0 r0 20
/// </summary>
public class ConstantFoldingOptimize : IOptimizationStrategy
{
    public IrProgram Apply(IrProgram program)
    {
        var instructionsToRemove = new HashSet<int>();
        var registerValues = new Dictionary<string, string>();
        var registerUsages = new Dictionary<string, int>();

        // First pass: track all LoadConst instructions and count usages
        for (var i = 0; i < program.Instructions.Count; i++)
        {
            if (program.Instructions[i] is LoadConstInstruction loadConst)
            {
                registerValues[loadConst.Target] = loadConst.Value.ToString() ?? string.Empty;
                registerUsages[loadConst.Target] = 0;
            }
        }

        // Second pass: replace constant register uses with direct values and count usages
        for (var i = 0; i < program.Instructions.Count; i++)
        {
            var instr = program.Instructions[i];

            if (instr is BinaryOpInstruction binOp)
            {
                // Check if the right operand is a temporary register holding a constant
                if (registerValues.TryGetValue(binOp.Right, out var constantValue))
                {
                    // Replace with constant value
                    program.Instructions[i] = new BinaryOpInstruction(
                        binOp.Target,
                        binOp.Left,
                        constantValue,
                        binOp.Op
                    );
                    registerUsages[binOp.Right]++;
                }
                
                // Also check left operand (though less common)
                if (registerValues.TryGetValue(binOp.Left, out var leftConstValue))
                {
                    registerUsages[binOp.Left]++;
                }
            }
            else if (instr is CompareInstruction compare)
            {
                var newLeft = compare.Left;
                var newRight = compare.Right;
                
                if (registerValues.TryGetValue(compare.Right, out var rightConstValue))
                {
                    newRight = rightConstValue;
                    registerUsages[compare.Right]++;
                }
                
                if (registerValues.TryGetValue(compare.Left, out var leftConstValue))
                {
                    newLeft = leftConstValue;
                    registerUsages[compare.Left]++;
                }
                
                if (newLeft != compare.Left || newRight != compare.Right)
                {
                    program.Instructions[i] = new CompareInstruction(
                        compare.Target,
                        newLeft,
                        newRight,
                        compare.Op
                    );
                }
            }
            else if (instr is LogicalAndInstruction logicalAnd)
            {
                var replaced = false;
                var newLeft = logicalAnd.Left;
                var newRight = logicalAnd.Right;
                
                if (registerValues.TryGetValue(logicalAnd.Right, out var rightConstValue))
                {
                    newRight = rightConstValue;
                    registerUsages[logicalAnd.Right]++;
                    replaced = true;
                }
                
                if (registerValues.TryGetValue(logicalAnd.Left, out var leftConstValue))
                {
                    newLeft = leftConstValue;
                    registerUsages[logicalAnd.Left]++;
                    replaced = true;
                }
                
                if (replaced)
                {
                    program.Instructions[i] = new LogicalAndInstruction(logicalAnd.Target, newLeft, newRight);
                }
            }
            else if (instr is LogicalOrInstruction logicalOr)
            {
                var replaced = false;
                var newLeft = logicalOr.Left;
                var newRight = logicalOr.Right;
                
                if (registerValues.TryGetValue(logicalOr.Right, out var rightConstValue))
                {
                    newRight = rightConstValue;
                    registerUsages[logicalOr.Right]++;
                    replaced = true;
                }
                
                if (registerValues.TryGetValue(logicalOr.Left, out var leftConstValue))
                {
                    newLeft = leftConstValue;
                    registerUsages[logicalOr.Left]++;
                    replaced = true;
                }
                
                if (replaced)
                {
                    program.Instructions[i] = new LogicalOrInstruction(logicalOr.Target, newLeft, newRight);
                }
            }
            else if (instr is StoreToDeviceInstruction store)
            {
                if (registerValues.ContainsKey(store.Value))
                {
                    registerUsages[store.Value]++;
                    // Note: StoreToDeviceInstruction already handles constant values directly
                }
            }
        }

        // Third pass: remove LoadConst instructions that were fully replaced
        for (var i = 0; i < program.Instructions.Count; i++)
        {
            if (program.Instructions[i] is LoadConstInstruction loadConst)
            {
                // If this register was used and all uses were replaced with constants, remove it
                if (registerUsages.ContainsKey(loadConst.Target) && 
                    !IsRegisterUsedDirectly(program.Instructions, loadConst.Target, i + 1))
                {
                    instructionsToRemove.Add(i);
                }
            }
        }

        // Remove marked instructions in reverse order to maintain correct indices
        foreach (var index in instructionsToRemove.OrderByDescending(x => x))
        {
            program.Instructions.RemoveAt(index);
        }

        return program;
    }

    /// <summary>
    /// Checks if a register is used directly (not as a constant replacement candidate) after a given position
    /// </summary>
    private bool IsRegisterUsedDirectly(List<IrInstruction> instructions, string register, int afterIndex)
    {
        for (int i = afterIndex; i < instructions.Count; i++)
        {
            var instr = instructions[i];

            // Check if register is used in any instruction where we haven't replaced it
            if (instr is MoveInstruction move)
            {
                if (move.Source == register)
                    return true;
                if (move.Target == register)
                    return false; // Register reassigned
            }
            else if (instr is BranchIfZeroInstruction branch)
            {
                if (branch.Condition == register)
                    return true;
            }
            else if (instr is LoadConstInstruction loadConst)
            {
                if (loadConst.Target == register)
                    return false; // Register reassigned
            }
            // If it's in a BinaryOp, Compare, etc., we've already replaced it with constant
            // so it doesn't count as "direct use"
        }

        return false;
    }


}

