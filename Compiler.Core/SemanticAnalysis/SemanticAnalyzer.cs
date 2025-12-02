using Compiler.Domain.Ast.Expressions;
using Compiler.Domain.Ast.Statements;
using Compiler.Domain.Ast.Visitors;
using Compiler.Domain.Types;

namespace Compiler.SemanticAnalysis;

public sealed class SemanticAnalyzer :
    IExprVisitor<TypeInfo>,
    IStmtVisitor<TypeInfo>
{
    private readonly SymbolTable _symbols = new();
    private readonly Dictionary<string, string> _deviceReferences = new();

    public Dictionary<string, string> DeviceReferences => _deviceReferences;

    public SemanticAnalyzer()
    {
        _symbols.DeclareFunction("sleep", [], TypeInfo.Void);
        _symbols.DeclareFunction("referenceDevice", [TypeInfo.Device], TypeInfo.StationeersDevice);
        _symbols.DeclareFunction("convertToCelsius", [TypeInfo.Int], TypeInfo.Int);
    }

    public void Analyze(List<Stmt> statements)
    {
        foreach (var stmt in statements)
            stmt.Accept(this);
    }

    public TypeInfo VisitVarDeclaration(VarDeclarationStmt stmt)
    {
        var initializerType = stmt.Initializer.Accept(this);

        if (initializerType.Kind == TypeKind.Void)
            throw new Exception($"Cannot assign void function result to variable '{stmt.Name}'");

        // If explicit type is provided, validate it
        TypeInfo declaredType;
        if (stmt.ExplicitType != null)
        {
            declaredType = ParseTypeName(stmt.ExplicitType);

            // Check if initializer type matches declared type
            if (declaredType.Kind != initializerType.Kind)
            {
                throw new Exception(
                    $"Type mismatch in variable '{stmt.Name}': declared as '{stmt.ExplicitType}' but initialized with '{initializerType.Kind}'");
            }
        }
        else
        {
            // Type inference from initializer
            declaredType = initializerType;
        }

        if (stmt.Initializer is CallExpr { Callee: IdentifierExpr { Name: "referenceDevice" } } callExpr)
        {
            if (callExpr.Arguments[0] is IdentifierExpr deviceId)
            {
                _deviceReferences[stmt.Name] = deviceId.Name;
            }
        }

        _symbols.Declare(stmt.Name, declaredType);
        return declaredType;
    }

    private TypeInfo ParseTypeName(string typeName)
    {
        return typeName switch
        {
            "Int" => TypeInfo.Int,
            "Float" => TypeInfo.Float,
            "Boolean" => TypeInfo.Boolean,
            "StationeersDevice" => TypeInfo.StationeersDevice,
            "Device" => TypeInfo.Device,
            _ => throw new Exception($"Unknown type name '{typeName}'")
        };
    }

    public TypeInfo VisitExprStmt(ExprStmt stmt)
    {
        var resultType = stmt.Expression.Accept(this);

        return stmt.Expression switch
        {
            // Check if a function with a return value is being called without using the result
            StaticMethodCallExpr staticCall when resultType.Kind != TypeKind.Void => throw new Exception(
                $"Function '{staticCall.ObjectName}.{staticCall.MethodName}' returns a value of type '{resultType.Kind}' " +
                $"but the result is not being used. Assign it to a variable or use it in an expression."),

            MethodCallExpr methodCall when resultType.Kind != TypeKind.Void => throw new Exception(
                $"Method '{methodCall.MethodName}' returns a value of type '{resultType.Kind}' " +
                $"but the result is not being used. Assign it to a variable or use it in an expression."),

            CallExpr call when call.Callee is IdentifierExpr calleeId && resultType.Kind != TypeKind.Void => throw
                new Exception($"Function '{calleeId.Name}' returns a value of type '{resultType.Kind}' " +
                              $"but the result is not being used. Assign it to a variable or use it in an expression."),
            _ => resultType
        };
    }

    public TypeInfo VisitIf(IfStmt stmt)
    {
        stmt.Condition.Accept(this);
        stmt.ThenBranch.Accept(this);
        stmt.ElseBranch?.Accept(this);
        return TypeInfo.Int;
    }

    public TypeInfo VisitBlock(BlockStmt stmt)
    {
        foreach (var s in stmt.Statements)
            s.Accept(this);
        return TypeInfo.Int;
    }

    public TypeInfo VisitNumber(NumberExpr expr)
        => TypeInfo.Int;

    public TypeInfo VisitFloat(FloatExpr expr)
        => TypeInfo.Float;

    public TypeInfo VisitBoolean(BooleanExpr expr)
        => TypeInfo.Boolean;

    public TypeInfo VisitIdentifier(IdentifierExpr expr)
    {
        if (IsDeviceIdentifier(expr.Name))
            return TypeInfo.Device;

        if (_deviceReferences.ContainsKey(expr.Name))
            return TypeInfo.StationeersDevice;

        return _symbols.Lookup(expr.Name);
    }

    public TypeInfo VisitBinary(BinaryExpr expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);

        var isComparison = expr.Op is ">" or "<" or ">=" or "<=" or "==" or "!=";

        if (isComparison)
        {
            if ((left.Kind != TypeKind.Int && left.Kind != TypeKind.Float) ||
                (right.Kind != TypeKind.Int && right.Kind != TypeKind.Float))
            {
                throw new Exception($"Comparison operators require numeric types. Got {left.Kind} and {right.Kind}");
            }

            return left.Kind != right.Kind
                ? throw new Exception(
                    $"Comparison requires both sides to have the same type. Got {left.Kind} and {right.Kind}")
                : TypeInfo.Boolean;
        }

        // Both operands must be numeric (Int or Float)
        if ((left.Kind != TypeKind.Int && left.Kind != TypeKind.Float) ||
            (right.Kind != TypeKind.Int && right.Kind != TypeKind.Float))
            return TypeInfo.Error;

        // If either operand is Float, result is Float
        if (left.Kind == TypeKind.Float || right.Kind == TypeKind.Float)
            return TypeInfo.Float;

        // Both Int, result is Int
        return TypeInfo.Int;
    }

    public TypeInfo VisitGroup(GroupExpr expr)
        => expr.Inner.Accept(this);

    public TypeInfo VisitDevice(DeviceExpr expr)
        => TypeInfo.Int;

    public TypeInfo VisitStationeerConstant(StationeerConstantExpr expr)
        => TypeInfo.StationeerConstant;

    public TypeInfo VisitString(StringExpr expr)
        => TypeInfo.String;

    public TypeInfo VisitLoadDevice(LoadDeviceExpr expr)
    {
        expr.Device.Accept(this);
        return TypeInfo.Int;
    }

    public TypeInfo VisitCall(CallExpr expr)
    {
        if (expr.Callee is not IdentifierExpr id)
            return TypeInfo.Error;

        var functionType = _symbols.LookupFunction(id.Name);

        if (expr.Arguments.Count != functionType.ParameterTypes!.Count)
            throw new Exception(
                $"Function '{id.Name}' expects {functionType.ParameterTypes.Count} arguments, but got {expr.Arguments.Count}");

        switch (id.Name)
        {
            case "referenceDevice":
                if (expr.Arguments[0] is not IdentifierExpr deviceId)
                    throw new Exception("referenceDevice must take a device identifier (e.g., d0, d1, db)");

                if (!IsDeviceIdentifier(deviceId.Name))
                    throw new Exception(
                        $"'{deviceId.Name}' is not a valid device identifier. Use d0, d1, d2, d3, d4, d5, or db");
                break;
        }

        for (var i = 0; i < expr.Arguments.Count; i++)
        {
            var expectedType = functionType.ParameterTypes[i];

            if (expectedType.Kind == TypeKind.Device)
            {
                if (expr.Arguments[i] is IdentifierExpr deviceIdent)
                {
                    if (IsDeviceIdentifier(deviceIdent.Name) || _deviceReferences.ContainsKey(deviceIdent.Name))
                    {
                        continue;
                    }
                }

                throw new Exception(
                    $"Argument {i + 1} of function '{id.Name}' must be a device identifier or device reference");
            }

            var argType = expr.Arguments[i].Accept(this);

            // Allow StationeersDevice where Device or StationeersDevice is expected
            if (expectedType.Kind == TypeKind.StationeersDevice && argType.Kind == TypeKind.StationeersDevice)
                continue;

            if (argType.Kind != expectedType.Kind)
                throw new Exception(
                    $"Argument {i + 1} of function '{id.Name}' has incorrect type. Expected {expectedType.Kind}, got {argType.Kind}");
        }

        return functionType.ReturnType!;
    }

    private bool IsDeviceIdentifier(string name)
    {
        if (name == "db") return true;
        if (name is not ['d', _] || !char.IsDigit(name[1])) return false;
        var digit = name[1] - '0';
        return digit is >= 0 and <= 5;
    }

    public TypeInfo VisitDeviceProperty(DevicePropertyExpr expr)
        => TypeInfo.Int;

    public TypeInfo VisitAssignment(AssignmentExpr expr)
    {
        var varType = _symbols.Lookup(expr.Name);

        var valueType = expr.Value.Accept(this);

        if (valueType.Kind == TypeKind.Void)
            throw new Exception($"Cannot assign void function result to variable '{expr.Name}'");

        return varType.Kind != valueType.Kind
            ? throw new Exception($"Cannot assign {valueType.Kind} to variable '{expr.Name}' of type {varType.Kind}")
            : valueType;
    }

    public TypeInfo VisitMemberAccess(MemberAccessExpr expr)
    {
        var objectType = expr.Object.Accept(this);

        // Check if this type has members
        if (objectType.Members == null)
            throw new Exception($"Type '{objectType.Kind}' does not have accessible members");

        // Check if the specific member exists
        if (!objectType.HasMember(expr.MemberName))
        {
            var availableMembers = string.Join(", ", objectType.Members.Keys);
            throw new Exception(
                $"Type '{objectType.Kind}' does not have member '{expr.MemberName}'. Available members: {availableMembers}");
        }

        // Get member info
        var member = objectType.GetMember(expr.MemberName);

        return member switch
        {
            // Check if it's a property (member access is only for properties)
            PropertyInfo property => property.PropertyType,
            MethodInfo => throw new Exception(
                $"'{expr.MemberName}' is a method and must be called with parentheses: {expr.MemberName}()"),
            _ => throw new Exception($"Unknown member type for '{expr.MemberName}'")
        };
    }

    public TypeInfo VisitMemberAssignment(MemberAssignmentExpr expr)
    {
        var objectType = expr.Object.Accept(this);

        // Check if this type has members
        if (objectType.Members == null)
            throw new Exception($"Type '{objectType.Kind}' does not have accessible members");

        // Check if the specific member exists
        if (!objectType.HasMember(expr.MemberName))
        {
            var availableMembers = string.Join(", ", objectType.Members.Keys);
            throw new Exception(
                $"Type '{objectType.Kind}' does not have member '{expr.MemberName}'. Available members: {availableMembers}");
        }

        // Get member info
        var member = objectType.GetMember(expr.MemberName);

        // Check if it's a property (only properties can be assigned)
        if (member is not PropertyInfo property)
        {
            throw new Exception($"'{expr.MemberName}' is not a property and cannot be assigned");
        }

        // Check the value type matches the property type
        var valueType = expr.Value.Accept(this);

        if (valueType.Kind != property.PropertyType.Kind)
        {
            throw new Exception(
                $"Cannot assign {valueType.Kind} to property '{expr.MemberName}' of type {property.PropertyType.Kind}");
        }

        return valueType;
    }

    public TypeInfo VisitStaticMethodCall(StaticMethodCallExpr expr)
    {
        // Currently only "Math" object is supported
        if (expr.ObjectName != "Math")
            throw new Exception($"Unknown global object '{expr.ObjectName}'. Available: Math");

        // Validate method exists on Math object
        switch (expr.MethodName)
        {
            case "convertToCelsius":
                // Validate arguments
                if (expr.Arguments.Count != 1)
                    throw new Exception($"Math.convertToCelsius expects 1 argument, but got {expr.Arguments.Count}");

                var argType = expr.Arguments[0].Accept(this);

                // Accept Int or Float
                if (argType.Kind != TypeKind.Int && argType.Kind != TypeKind.Float)
                    throw new Exception($"Math.convertToCelsius expects Int or Float argument, but got {argType.Kind}");

                // Return same type as input (Int -> Int, Float -> Float)
                return argType;

            default:
                throw new Exception(
                    $"Unknown method '{expr.MethodName}' on Math object. Available methods: convertToCelsius");
        }
    }

    public TypeInfo VisitMethodCall(MethodCallExpr expr)
    {
        // Get the type of the object
        var objectType = expr.Object.Accept(this);

        // Check if type has members/methods
        if (objectType.Members == null)
            throw new Exception($"Type '{objectType.Kind}' does not have methods");

        // Check if method exists on the type
        if (!objectType.HasMember(expr.MethodName))
        {
            var availableMembers = string.Join(", ", objectType.Members.Keys);
            throw new Exception(
                $"Type '{objectType.Kind}' does not have method '{expr.MethodName}'. Available members: {availableMembers}");
        }

        // Get member info
        var member = objectType.GetMember(expr.MethodName);

        // Validate it's a method, not a property
        if (member is PropertyInfo)
        {
            throw new Exception($"'{expr.MethodName}' is a property, not a method. Use it without parentheses.");
        }

        if (member is not MethodInfo method)
        {
            throw new Exception($"Unknown member type for '{expr.MethodName}'");
        }

        // Validate arguments count
        if (expr.Arguments.Count != method.ParameterTypes.Count)
        {
            throw new Exception(
                $"Method '{expr.MethodName}' expects {method.ParameterTypes.Count} argument(s), but got {expr.Arguments.Count}");
        }

        // Validate argument types
        for (var i = 0; i < expr.Arguments.Count; i++)
        {
            var argType = expr.Arguments[i].Accept(this);
            var expectedType = method.ParameterTypes[i];

            if (argType.Kind != expectedType.Kind)
            {
                throw new Exception(
                    $"Method '{expr.MethodName}' parameter {i + 1} expects {expectedType.Kind}, but got {argType.Kind}");
            }
        }

        // Return the method's return type
        return method.ReturnType;
    }

    public TypeInfo VisitCompoundAssignment(CompoundAssignmentExpr expr)
    {
        var varType = _symbols.Lookup(expr.Name);
        var valueType = expr.Value.Accept(this);

        if ((varType.Kind != TypeKind.Int && varType.Kind != TypeKind.Float) ||
            (valueType.Kind != TypeKind.Int && valueType.Kind != TypeKind.Float))
        {
            throw new Exception(
                $"Compound assignment requires numeric types. Variable '{expr.Name}' is {varType.Kind.ToString()}, value is {valueType.Kind.ToString()}");
        }

        // Result type matches the variable type
        return varType;
    }

    public TypeInfo VisitIncrementDecrement(IncrementDecrementExpr expr)
    {
        var varType = _symbols.Lookup(expr.Name);

        // Variable must be a numeric type (Int or Float)
        if (varType.Kind != TypeKind.Int && varType.Kind != TypeKind.Float)
        {
            throw new Exception(
                $"Increment/decrement operator requires a numeric type. Variable '{expr.Name}' is {varType.Kind}");
        }

        return varType;
    }
}