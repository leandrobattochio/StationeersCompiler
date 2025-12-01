using Compiler;
using Spectre.Console;

var compiler = new StationeersCompiler(File.ReadAllText("example.st"));

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