using Compiler.Domain.Types;

namespace Compiler.SemanticAnalysis;

public sealed class SymbolTable
{
    private readonly Dictionary<string, TypeInfo> _symbols = new();
    private readonly Dictionary<string, TypeInfo> _functions = new();

    public void Declare(string name, TypeInfo type)
    {
        if (_symbols.ContainsKey(name))
            throw new Exception($"Symbol '{name}' already declared.");

        _symbols[name] = type;
    }

    public TypeInfo Lookup(string name)
    {
        return _symbols.TryGetValue(name, out var type)
            ? type
            : throw new Exception($"Undefined symbol '{name}'.");
    }

    public void DeclareFunction(string name, List<TypeInfo> parameterTypes, TypeInfo returnType)
    {
        if (_functions.ContainsKey(name))
            throw new Exception($"Function '{name}' already declared.");

        _functions[name] = new TypeInfo(TypeKind.Function, returnType, parameterTypes);
    }

    public TypeInfo LookupFunction(string name)
    {
        return _functions.TryGetValue(name, out var type)
            ? type
            : throw new Exception($"Undefined function '{name}'.");
    }
}
