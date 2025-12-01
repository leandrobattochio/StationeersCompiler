using Compiler.CodeGeneration;
using Compiler.Lexer;
using Compiler.Parser;
using Compiler.SemanticAnalysis;
using Spectre.Console;

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

AnsiConsole.MarkupLine("[red]Tokens[/]");
lexer.DebugTokens();

var parser = new StationeerParser(lexer);

var ast = parser.Parse();

AnsiConsole.MarkupLine("[yellow]AST[/]");
foreach (var stmt in ast)
    Console.WriteLine(stmt);

var semantic = new SemanticAnalyzer();

AnsiConsole.MarkupLine("[green]Semantics[/]");

try
{
    semantic.Analyze(ast);
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]Exception: {ex.Message}[/]");
    Console.Read();
    Environment.Exit(1);
}

Console.WriteLine("Semantic analysis completed successfully");

var irBuilder = new IrBuilder();
var ir = irBuilder.Build(ast, semantic.DeviceReferences);

var irOptimizer = new IrOptimizer();
var optimizedIr = irOptimizer.Optimize(ir);

var asm = CodeGenerator.Generate(optimizedIr);

AnsiConsole.MarkupLine("[blue]Stationeers Assembly[/]");
foreach (var line in asm)
    Console.WriteLine(line);


Console.ReadLine();