namespace Compiler.Domain.Types;

public sealed class TypeInfo
{
    public TypeKind Kind { get; }
    public List<TypeInfo>? ParameterTypes { get; }
    public TypeInfo? ReturnType { get; }
    public Dictionary<string, MemberInfo>? Members { get; }

    public TypeInfo(TypeKind kind, TypeInfo? returnType = null, List<TypeInfo>? parameterTypes = null,
        Dictionary<string, MemberInfo>? members = null)
    {
        Kind = kind;
        ParameterTypes = parameterTypes;
        ReturnType = returnType;
        Members = members;
    }

    public bool HasMember(string memberName) => Members?.ContainsKey(memberName) ?? false;

    public MemberInfo? GetMember(string memberName) => Members?.GetValueOrDefault(memberName);

    public PropertyInfo? GetProperty(string memberName) => GetMember(memberName) as PropertyInfo;

    public MethodInfo? GetMethod(string memberName) => GetMember(memberName) as MethodInfo;

    public static readonly TypeInfo Void = new(TypeKind.Void);
    public static readonly TypeInfo Int = new(TypeKind.Int);
    public static readonly TypeInfo Boolean = new(TypeKind.Boolean);
    public static readonly TypeInfo Float = new(TypeKind.Float);
    public static readonly TypeInfo Error = new(TypeKind.Error);
    public static readonly TypeInfo Device = new(TypeKind.Device);

    // StationeersDevice with Temperature and Pressure properties and methods
    public static readonly TypeInfo StationeersDevice = new(
        TypeKind.StationeersDevice,
        members: new Dictionary<string, MemberInfo>
        {
            // Properties
            { "Activate", new PropertyInfo("Activate", Float) },
            { "AirRelease", new PropertyInfo("AirRelease", Float) },
            { "CompletionRatio", new PropertyInfo("CompletionRatio", Float) },
            { "ElevatorLevel", new PropertyInfo("ElevatorLevel", Float) },
            { "ElevatorSpeed", new PropertyInfo("ElevatorSpeed", Float) },
            { "Error", new PropertyInfo("Error", Boolean) },
            { "ExportCount", new PropertyInfo("ExportCount", Float) },
            { "Filtration", new PropertyInfo("Filtration", Boolean) },
            { "Harvest", new PropertyInfo("Harvest", Boolean) },
            { "Horizontal", new PropertyInfo("Horizontal", Float) },
            { "HorizontalRatio", new PropertyInfo("HorizontalRatio", Float) },
            { "Idle", new PropertyInfo("Idle", Boolean) },
            { "ImportCount", new PropertyInfo("ImportCount", Float) },
            { "Lock", new PropertyInfo("Lock", Boolean) },
            { "Maximum", new PropertyInfo("Maximum", Float) },
            { "Mode", new PropertyInfo("Mode", Float) },
            { "On", new PropertyInfo("On", Boolean) },
            { "Open", new PropertyInfo("Open", Float) },
            { "Output", new PropertyInfo("Output", Float) },
            { "Plant", new PropertyInfo("Plant", Float) },
            { "PositionX", new PropertyInfo("PositionX", Float) },
            { "PositionY", new PropertyInfo("PositionY", Float) },
            { "PositionZ", new PropertyInfo("PositionZ", Float) },
            { "Power", new PropertyInfo("Power", Float) },
            { "PowerActual", new PropertyInfo("PowerActual", Float) },
            { "PowerPotential", new PropertyInfo("PowerPotential", Float) },
            { "PowerRequired", new PropertyInfo("PowerRequired", Float) },
            { "Pressure", new PropertyInfo("Pressure", Float) },
            { "PressureExternal", new PropertyInfo("PressureExternal", Float) },
            { "PressureInteral", new PropertyInfo("PressureInteral", Float) },
            { "PressureSetting", new PropertyInfo("PressureSetting", Float) },
            { "Quantity", new PropertyInfo("Quantity", Float) },
            { "Ratio", new PropertyInfo("Ratio", Float) },
            { "RatioCarbonDioxide", new PropertyInfo("RatioCarbonDioxide", Float) },
            { "RatioNitrogen", new PropertyInfo("RatioNitrogen", Float) },
            { "RatioOxygen", new PropertyInfo("RatioOxygen", Float) },
            { "RatioPollutant", new PropertyInfo("RatioPollutant", Float) },
            { "RatioVolatiles", new PropertyInfo("RatioVolatiles", Float) },
            { "RatioWater", new PropertyInfo("RatioWater", Float) },
            { "Reagents", new PropertyInfo("Reagents", Float) },
            { "RecipeHash", new PropertyInfo("RecipeHash", Float) },
            { "ReferenceId", new PropertyInfo("ReferenceId", Float) },
            { "RequestHash", new PropertyInfo("RequestHash", Float) },
            { "RequiredPower", new PropertyInfo("RequiredPower", Float) },
            { "Setting", new PropertyInfo("Setting", Float) },
            { "SolarAngle", new PropertyInfo("SolarAngle", Float) },
            { "Temperature", new PropertyInfo("Temperature", Float) },
            { "TemperatureSettings", new PropertyInfo("TemperatureSettings", Float) },
            { "TotalMoles", new PropertyInfo("TotalMoles", Float) },
            { "VelocityMagnitude", new PropertyInfo("VelocityMagnitude", Float) },
            { "VelocityRelativeX", new PropertyInfo("VelocityRelativeX", Float) },
            { "VelocityRelativeY", new PropertyInfo("VelocityRelativeY", Float) },
            { "VelocityRelativeZ", new PropertyInfo("VelocityRelativeZ", Float) },
            { "Vertical", new PropertyInfo("Vertical", Float) },
            { "VerticalRatio", new PropertyInfo("VerticalRatio", Float) },
            { "Volume", new PropertyInfo("Volume", Float) },
        });
}