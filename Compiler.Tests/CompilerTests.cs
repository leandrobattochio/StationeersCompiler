using Compiler.CodeGeneration;
using Compiler.Lexer;
using Compiler.Parser;
using Compiler.SemanticAnalysis;

namespace Compiler.Tests;

public class CompilerTests
{
    [Fact]
    public void Should_Compile_Code()
    {
        var code = """
                   StationeersDevice airConditioner = referenceDevice(d0);
                   StationeersDevice airSensor = referenceDevice(d1);

                   Float temp = airConditioner.Temperature;
                   Float pressure = airSensor.Pressure;

                   temp = Math.convertToCelsius(temp);
                   if (temp >= 25 && pressure < 101325.0) {
                       airConditioner.On = true;
                   } else {
                       airConditioner.On = false;
                       airSensor.On = true;
                   }
                   """;
        
        var lexer = new StationeerLexer(code);
        var parser = new StationeerParser(lexer);
        var ast = parser.Parse();
        var semantic = new SemanticAnalyzer();
        semantic.Analyze(ast);
        var irBuilder = new IrBuilder();
        var ir = irBuilder.Build(ast, semantic.DeviceReferences);
        var asm = CodeGenerator.Generate(ir);


        var expectedAssembly = """
                               start:
                               l r0 d0 Temperature
                               l r1 d1 Pressure
                               sub r2 r0 273.15
                               mov r0 r2
                               mov r3 25
                               sge r4 r0 r3
                               mov r3 101325
                               slt r0 r1 r3
                               mul r3 r4 r0
                               beqz r3 L0
                               s d0 On 1
                               j L1
                               L0:
                               s d0 On 0
                               s d1 On 1
                               L1:
                               j start
                               """;
        
        var compiledAsmLines = string.Join(Environment.NewLine, asm);
        
        Assert.Equal(expectedAssembly, compiledAsmLines);
    }
}