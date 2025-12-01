namespace Compiler.Domain.Types;

/// <summary>
/// Represents information about a member (property or method) of a type
/// </summary>
public abstract class MemberInfo
{
    public string Name { get; }

    protected MemberInfo(string name)
    {
        Name = name;
    }
}

/// <summary>
/// Represents a property member (a value that can be read)
/// </summary>
public sealed class PropertyInfo : MemberInfo
{
    public TypeInfo PropertyType { get; }

    public PropertyInfo(string name, TypeInfo propertyType) : base(name)
    {
        PropertyType = propertyType;
    }
}

/// <summary>
/// Represents a method member (a callable function)
/// </summary>
public sealed class MethodInfo : MemberInfo
{
    public TypeInfo ReturnType { get; }
    public List<TypeInfo> ParameterTypes { get; }

    public MethodInfo(string name, TypeInfo returnType, List<TypeInfo>? parameterTypes = null) : base(name)
    {
        ReturnType = returnType;
        ParameterTypes = parameterTypes ?? new List<TypeInfo>();
    }
}

