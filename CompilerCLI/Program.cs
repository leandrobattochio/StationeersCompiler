using Compiler;
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

var compiler = new StationeersCompiler(code);

AnsiConsole.MarkupLine("[red]Tokens[/]");

foreach (var token in compiler.DebugTokens())
    Console.WriteLine(token);


AnsiConsole.MarkupLine("[yellow]AST[/]");
foreach (var stmt in compiler.DebugAst())
    Console.WriteLine(stmt);

AnsiConsole.MarkupLine("[green]Semantics[/]");
AnsiConsole.MarkupLine($"[red]{compiler.DebugSemantics()}[/]");

var asm = compiler.Compile(true);

AnsiConsole.MarkupLine("[blue]Stationeers Assembly[/]");
foreach (var line in asm)
    Console.WriteLine(line);

Console.ReadLine();