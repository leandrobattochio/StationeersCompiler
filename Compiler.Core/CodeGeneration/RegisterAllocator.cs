using Compiler.Domain.IR;

namespace Compiler.CodeGeneration;

public sealed class RegisterAllocator
{
    private readonly Dictionary<string, string> _tempToReg = new();
    private readonly Stack<string> _freeRegs = new();
    private readonly Dictionary<int, List<string>> _tempsEndingAt = new();

    public RegisterAllocator(IrProgram program)
    {
        for (var i = 15; i >= 0; i--)
        {
            _freeRegs.Push($"r{i}");
        }

        var lastUse = ComputeLastUse(program);

        foreach (var (temp, index) in lastUse)
        {
            if (!_tempsEndingAt.TryGetValue(index, out var list))
            {
                list = new List<string>();
                _tempsEndingAt[index] = list;
            }

            list.Add(temp);
        }
    }

    private static Dictionary<string, int> ComputeLastUse(IrProgram program)
    {
        var lastUse = new Dictionary<string, int>();
        var instructions = program.Instructions;

        for (var i = 0; i < instructions.Count; i++)
        {
            UpdateLastUseForInstruction(instructions[i], i, lastUse);
        }

        return lastUse;
    }

    private static void UpdateLastUseForInstruction(IrInstruction instruction, int index, Dictionary<string, int> lastUse)
    {
        switch (instruction)
        {
            case LoadConstInstruction lc:
                UpdateLastUse(lc.Target, index, lastUse);
                break;

            case BinaryOpInstruction bo:
                UpdateLastUseForBinaryOperands(bo.Left, bo.Right, bo.Target, index, lastUse);
                break;

            case LoadFromDeviceInstruction lfd:
                UpdateLastUseIfTemp(lfd.Device, index, lastUse);
                UpdateLastUseIfTemp(lfd.Parameter, index, lastUse);
                UpdateLastUse(lfd.Target, index, lastUse);
                break;

            case CompareInstruction cmp:
                UpdateLastUseForBinaryOperands(cmp.Left, cmp.Right, cmp.Target, index, lastUse);
                break;

            case LogicalAndInstruction and:
                UpdateLastUseForBinaryOperands(and.Left, and.Right, and.Target, index, lastUse);
                break;

            case LogicalOrInstruction or:
                UpdateLastUseForBinaryOperands(or.Left, or.Right, or.Target, index, lastUse);
                break;

            case BranchIfZeroInstruction bz:
                UpdateLastUseIfTemp(bz.Condition, index, lastUse);
                break;

            case StoreToDeviceInstruction std:
                UpdateLastUseIfTemp(std.Device, index, lastUse);
                UpdateLastUseIfTemp(std.Value, index, lastUse);
                break;
        }
    }

    private static void UpdateLastUseForBinaryOperands(string left, string right, string target, int index, Dictionary<string, int> lastUse)
    {
        UpdateLastUseIfTemp(left, index, lastUse);
        UpdateLastUseIfTemp(right, index, lastUse);
        UpdateLastUse(target, index, lastUse);
    }

    private static void UpdateLastUseIfTemp(string operand, int index, Dictionary<string, int> lastUse)
    {
        if (IsTemp(operand))
        {
            UpdateLastUse(operand, index, lastUse);
        }
    }

    private static void UpdateLastUse(string temp, int index, Dictionary<string, int> lastUse)
    {
        lastUse[temp] = index;
    }

    private static bool IsTemp(string name)
    {
        return name.StartsWith('t');
    }

    public string GetRegister(string temp, int instrIndex)
    {
        if (_tempToReg.TryGetValue(temp, out var reg))
            return reg;

        if (_freeRegs.Count == 0)
            throw new Exception("No more registers available.");

        reg = _freeRegs.Pop();
        _tempToReg[temp] = reg;
        return reg;
    }

    public void ReleaseForInstruction(int instrIndex)
    {
        if (!_tempsEndingAt.TryGetValue(instrIndex, out var temps))
            return;

        foreach (var temp in temps)
        {
            if (_tempToReg.Remove(temp, out var reg))
            {
                _freeRegs.Push(reg);
            }
        }
    }
}
