using Compiler.Domain.IR;

namespace Compiler.CodeGeneration;

public static class CodeGenerator
{
    private static string ResolveOperand(string operand, int instrIndex, RegisterAllocator regAlloc)
    {
        if (operand.StartsWith('t') && char.IsDigit(operand[1]))
            return regAlloc.GetRegister(operand, instrIndex);
        
        return operand;
    }

    public static IReadOnlyList<string> Generate(IrProgram program)
    {
        var lines = new List<string>();
        var regAlloc = new RegisterAllocator(program);
        var instructions = program.Instructions;

        lines.Add("start:");
        for (var i = 0; i < instructions.Count; i++)
        {
            var instr = instructions[i];

            switch (instr)
            {
                case LoadConstInstruction lc:
                    EmitLoadConst(lc, i, regAlloc, lines);
                    break;

                case BinaryOpInstruction bo:
                    EmitBinary(bo, i, regAlloc, lines);
                    break;

                case LoadFromDeviceInstruction lfd:
                    EmitLoadFromDevice(lfd, i, regAlloc, lines);
                    break;

                case CompareInstruction cmp:
                    EmitCompare(cmp, i, regAlloc, lines);
                    break;

                case LogicalAndInstruction and:
                    EmitLogicalAnd(and, i, regAlloc, lines);
                    break;

                case LogicalOrInstruction or:
                    EmitLogicalOr(or, i, regAlloc, lines);
                    break;
                
                case YieldInstruction y:
                    lines.Add("yield");
                    break;

                case BranchIfZeroInstruction bz:
                    EmitBranchIfZero(bz, i, regAlloc, lines);
                    break;

                case JumpInstruction jmp:
                    EmitJump(jmp, lines);
                    break;

                case LabelInstruction lbl:
                    EmitLabel(lbl, lines);
                    break;

                case StoreToDeviceInstruction std:
                    EmitStoreToDevice(std, lines);
                    break;

                case MoveInstruction mov:
                    EmitMove(mov, i, regAlloc, lines);
                    break;

                default:
                    throw new Exception("Unknown IR instruction type.");
            }

            regAlloc.ReleaseForInstruction(i);
        }
        
        lines.Add("j start");

        return lines;
    }

    private static void EmitLoadConst(
        LoadConstInstruction lc,
        int instrIndex,
        RegisterAllocator regAlloc,
        List<string> lines)
    {
        var reg = regAlloc.GetRegister(lc.Target, instrIndex);
        var value = lc.Value is double d 
            ? d.ToString("G", System.Globalization.CultureInfo.InvariantCulture)
            : lc.Value.ToString();
        lines.Add($"mov {reg} {value}");
    }

    private static void EmitBinary(
        BinaryOpInstruction bo,
        int instrIndex,
        RegisterAllocator regAlloc,
        List<string> lines)
    {
        var target = regAlloc.GetRegister(bo.Target, instrIndex);
        var left = ResolveOperand(bo.Left, instrIndex, regAlloc);
        var right = ResolveOperand(bo.Right, instrIndex, regAlloc);

        var op = bo.Op switch
        {
            "+" => "add",
            "-" => "sub",
            "*" => "mul",
            "/" => "div",
            _ => throw new Exception($"Unknown binary operator {bo.Op}")
        };

        lines.Add($"{op} {target} {left} {right}");
    }

    private static void EmitLoadFromDevice(
        LoadFromDeviceInstruction lfd,
        int instrIndex,
        RegisterAllocator regAlloc,
        List<string> lines)
    {
        var target = regAlloc.GetRegister(lfd.Target, instrIndex);
        var device = ResolveOperand(lfd.Device, instrIndex, regAlloc);
        var parameter = ResolveOperand(lfd.Parameter, instrIndex, regAlloc);
        lines.Add($"l {target} {device} {parameter}");
    }

    private static void EmitCompare(
        CompareInstruction cmp,
        int instrIndex,
        RegisterAllocator regAlloc,
        List<string> lines)
    {
        var target = regAlloc.GetRegister(cmp.Target, instrIndex);
        var left = ResolveOperand(cmp.Left, instrIndex, regAlloc);
        var right = ResolveOperand(cmp.Right, instrIndex, regAlloc);

        var op = cmp.Op switch
        {
            ">" => "sgt",
            "<" => "slt",
            ">=" => "sge",
            "<=" => "sle",
            "==" => "seq",
            "!=" => "sne",
            _ => throw new Exception($"Unknown comparison operator {cmp.Op}")
        };

        lines.Add($"{op} {target} {left} {right}");
    }

    private static void EmitLogicalAnd(
        LogicalAndInstruction and,
        int instrIndex,
        RegisterAllocator regAlloc,
        List<string> lines)
    {
        var target = regAlloc.GetRegister(and.Target, instrIndex);
        var left = ResolveOperand(and.Left, instrIndex, regAlloc);
        var right = ResolveOperand(and.Right, instrIndex, regAlloc);

        lines.Add($"mul {target} {left} {right}");
    }

    private static void EmitLogicalOr(
        LogicalOrInstruction or,
        int instrIndex,
        RegisterAllocator regAlloc,
        List<string> lines)
    {
        var target = regAlloc.GetRegister(or.Target, instrIndex);
        var left = ResolveOperand(or.Left, instrIndex, regAlloc);
        var right = ResolveOperand(or.Right, instrIndex, regAlloc);

        lines.Add($"add {target} {left} {right}");
        lines.Add($"sgt {target} {target} 0");
    }

    private static void EmitBranchIfZero(
        BranchIfZeroInstruction bz,
        int instrIndex,
        RegisterAllocator regAlloc,
        List<string> lines)
    {
        var cond = ResolveOperand(bz.Condition, instrIndex, regAlloc);
        lines.Add($"beqz {cond} {bz.Label}");
    }

    private static void EmitJump(JumpInstruction jmp, List<string> lines)
    {
        lines.Add($"j {jmp.Label}");
    }

    private static void EmitLabel(LabelInstruction lbl, List<string> lines)
    {
        lines.Add($"{lbl.Label}:");
    }

    private static void EmitStoreToDevice(StoreToDeviceInstruction std, List<string> lines)
    {
        lines.Add($"s {std.Device} {std.Property} {std.Value}");
    }

    private static void EmitMove(
        MoveInstruction mov,
        int instrIndex,
        RegisterAllocator regAlloc,
        List<string> lines)
    {
        var target = ResolveOperand(mov.Target, instrIndex, regAlloc);
        var source = ResolveOperand(mov.Source, instrIndex, regAlloc);
        lines.Add($"mov {target} {source}");
    }
}
