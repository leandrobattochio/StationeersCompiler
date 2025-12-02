using Compiler.Domain.Ast.Expressions;
using Compiler.Domain.Ast.Statements;
using Compiler.Domain.Ast.Visitors;
using Compiler.Domain.IR;

namespace Compiler.CodeGeneration;

public sealed class IrBuilder :
    IExprVisitor<string>,
    IStmtVisitor<object>
{
    private readonly List<IrInstruction> _instructions = new();
    private readonly Dictionary<string, string> _variableRegisters = new();
    private Dictionary<string, string> _deviceReferences = new();
    private int _temp;
    private int _label;

    private string Temp() => $"t{_temp++}";
    private string Label() => $"L{_label++}";

    public IrProgram Build(List<Stmt> statements, Dictionary<string, string>? deviceReferences = null)
    {
        _instructions.Clear();
        _variableRegisters.Clear();
        _deviceReferences = deviceReferences ?? new Dictionary<string, string>();
        _temp = 0;

        string? lastTemp = null;

        foreach (var stmt in statements)
            stmt.Accept(this);

        return new IrProgram(
            new List<IrInstruction>(_instructions),
            lastTemp ?? "r0"
        );
    }

    public object VisitVarDeclaration(VarDeclarationStmt stmt)
    {
        var valueReg = stmt.Initializer.Accept(this);
        _variableRegisters[stmt.Name] = valueReg;
        return new object();
    }

    public object VisitExprStmt(ExprStmt stmt)
    {
        if (stmt.Expression is CallExpr { Callee: IdentifierExpr { Name: "convertToCelsius" } } callExpr)
        {
            if (callExpr.Arguments.Count > 0 && callExpr.Arguments[0] is IdentifierExpr varExpr)
            {
                var resultReg = callExpr.Accept(this);
                _variableRegisters[varExpr.Name] = resultReg;
                return new object();
            }
        }

        stmt.Expression.Accept(this);
        return new object();
    }

    public object VisitIf(IfStmt stmt)
    {
        var condition = stmt.Condition.Accept(this);
        var elseLabel = Label();
        var endLabel = Label();

        _instructions.Add(new BranchIfZeroInstruction(condition, elseLabel));

        stmt.ThenBranch.Accept(this);
        _instructions.Add(new JumpInstruction(endLabel));

        _instructions.Add(new LabelInstruction(elseLabel));
        stmt.ElseBranch?.Accept(this);

        _instructions.Add(new LabelInstruction(endLabel));

        return new object();
    }

    public object VisitBlock(BlockStmt stmt)
    {
        foreach (var s in stmt.Statements)
            s.Accept(this);
        return new object();
    }

    public string VisitNumber(NumberExpr expr)
    {
        var t = Temp();
        _instructions.Add(new LoadConstInstruction(t, expr.Value));
        return t;
    }

    public string VisitFloat(FloatExpr expr)
    {
        var t = Temp();
        _instructions.Add(new LoadConstInstruction(t, expr.Value));
        return t;
    }

    public string VisitBoolean(BooleanExpr expr)
    {
        var t = Temp();
        _instructions.Add(new LoadConstInstruction(t, expr.Value ? 1 : 0));
        return t;
    }

    public string VisitIdentifier(IdentifierExpr expr)
    {
        if (_deviceReferences.TryGetValue(expr.Name, out var devicePin))
            return devicePin;

        return _variableRegisters.TryGetValue(expr.Name, out var reg) ? reg : expr.Name;
    }

    public string VisitBinary(BinaryExpr expr)
    {
        var l = expr.Left.Accept(this);
        var r = expr.Right.Accept(this);
        var t = Temp();

        switch (expr.Op)
        {
            case "&&":
                _instructions.Add(new LogicalAndInstruction(t, l, r));
                break;
            case "||":
                _instructions.Add(new LogicalOrInstruction(t, l, r));
                break;
            case ">" or "<" or ">=" or "<=" or "==" or "!=":
                _instructions.Add(new CompareInstruction(t, l, r, expr.Op));
                break;
            default:
                _instructions.Add(new BinaryOpInstruction(t, l, r, expr.Op));
                break;
        }

        return t;
    }

    public string VisitGroup(GroupExpr expr)
        => expr.Inner.Accept(this);

    public string VisitString(StringExpr expr)
    {
        return expr.Value;
    }

    public string VisitStationeerConstant(StationeerConstantExpr expr)
    {
        return expr.Value;
    }

    public string VisitDevice(DeviceExpr expr)
    {
        return expr.DeviceName;
    }

    public string VisitLoadDevice(LoadDeviceExpr expr)
    {
        var dev = expr.Device.Accept(this);
        var t = Temp();
        _instructions.Add(new LoadFromDeviceInstruction(t, dev, expr.Property));
        return t;
    }

    public string VisitCall(CallExpr expr)
    {
        if (expr.Callee is not IdentifierExpr id)
            throw new Exception("Unknown function");

        switch (id.Name)
        {
            case "sleep":
                _instructions.Add(new YieldInstruction());
                return "0"; // void function, return dummy value
            
            case "referenceDevice":
                return expr.Arguments[0].Accept(this);

            default:
                throw new Exception($"Unknown function: {id.Name}");
        }
    }

    public string VisitDeviceProperty(DevicePropertyExpr expr)
    {
        return expr.PropertyName;
    }

    public string VisitAssignment(AssignmentExpr expr)
    {
        if (_variableRegisters.TryGetValue(expr.Name, out var existingReg))
        {
            if (expr.Value is NumberExpr numExpr)
            {
                _instructions.Add(new LoadConstInstruction(existingReg, numExpr.Value));
                return existingReg;
            }

            var valueReg = expr.Value.Accept(this);
            _instructions.Add(new MoveInstruction(existingReg, valueReg));
            return existingReg;
        }
        else
        {
            var valueReg = expr.Value.Accept(this);
            _variableRegisters[expr.Name] = valueReg;
            return valueReg;
        }
    }

    public string VisitMemberAccess(MemberAccessExpr expr)
    {
        // Get the device register/identifier
        var deviceReg = expr.Object.Accept(this);
        
        // Generate a temp register for the result
        var resultReg = Temp();
        
        // Generate load instruction: l resultReg deviceReg PropertyName
        _instructions.Add(new LoadFromDeviceInstruction(resultReg, deviceReg, expr.MemberName));
        
        return resultReg;
    }

    public string VisitMemberAssignment(MemberAssignmentExpr expr)
    {
        // Get the device register/identifier
        var deviceReg = expr.Object.Accept(this);
        
        // Optimize: If assigning a boolean literal, use the literal value directly
        string valueToStore;
        if (expr.Value is BooleanExpr boolExpr)
        {
            valueToStore = boolExpr.Value ? "1" : "0";
        }
        else if (expr.Value is NumberExpr numExpr)
        {
            valueToStore = numExpr.Value.ToString();
        }
        else if (expr.Value is FloatExpr floatExpr)
        {
            valueToStore = floatExpr.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
            // For complex expressions, evaluate and store the result register
            valueToStore = expr.Value.Accept(this);
        }
        
        // Generate store instruction: s deviceReg PropertyName value
        _instructions.Add(new StoreToDeviceInstruction(deviceReg, expr.MemberName, valueToStore));
        
        return valueToStore;
    }

    public string VisitStaticMethodCall(StaticMethodCallExpr expr)
    {
        // Currently only Math object is supported
        if (expr.ObjectName != "Math")
            throw new Exception($"Unknown global object '{expr.ObjectName}'");

        switch (expr.MethodName)
        {
            case "convertToCelsius":
                var kelvinValue = expr.Arguments[0].Accept(this);
                var celsius = Temp();
                _instructions.Add(new BinaryOpInstruction(celsius, kelvinValue, "273.15", "-"));
                return celsius;

            default:
                throw new Exception($"Unknown method '{expr.MethodName}' on {expr.ObjectName}");
        }
    }

    public string VisitMethodCall(MethodCallExpr expr)
    {
        // Get the device register/identifier
        var deviceReg = expr.Object.Accept(this);

        // Currently no instance methods are defined
        // Methods are handled by the semantic analyzer
        throw new Exception($"Unknown method '{expr.MethodName}'");
    }

    public string VisitCompoundAssignment(CompoundAssignmentExpr expr)
    {
        // Get the current variable register
        if (!_variableRegisters.TryGetValue(expr.Name, out var varReg))
            throw new Exception($"Variable '{expr.Name}' not found");

        // Evaluate the right-hand side value
        var valueReg = expr.Value.Accept(this);

        // Generate the binary operation: varReg = varReg op valueReg
        var resultReg = Temp();
        _instructions.Add(new BinaryOpInstruction(resultReg, varReg, valueReg, expr.Op));

        // Move result back to the variable's register
        _instructions.Add(new MoveInstruction(varReg, resultReg));

        return varReg;
    }

    public string VisitIncrementDecrement(IncrementDecrementExpr expr)
    {
        // Get the current variable register
        if (!_variableRegisters.TryGetValue(expr.Name, out var varReg))
            throw new Exception($"Variable '{expr.Name}' not found");

        // Determine the operation (++ -> +, -- -> -)
        var op = expr.Op == "++" ? "+" : "-";

        // Generate the operation: varReg = varReg op 1
        var resultReg = Temp();
        _instructions.Add(new BinaryOpInstruction(resultReg, varReg, "1", op));

        // Move result back to the variable's register
        _instructions.Add(new MoveInstruction(varReg, resultReg));

        return varReg;
    }
}