using Compiler.CodeGeneration;
using Compiler.Domain.Ast.Statements;
using Compiler.Lexer;
using Compiler.Lexer.Tokens;
using Compiler.Parser;
using Compiler.SemanticAnalysis;

namespace Compiler;

public class StationeersCompiler(string code)
{
    private readonly List<string> _compileErrors = [];

    public List<string> CompileErrors => _compileErrors;

    public IReadOnlyList<Token> DebugTokens()
    {
        var lexer = new StationeerLexer(code);
        return lexer.Tokenize();
    }

    public string DebugSemantics()
    {
        var ast = DebugAst();
        var semantics = new SemanticAnalyzer();

        try
        {
            semantics.Analyze(ast.ToList());
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return string.Empty;
    }

    public IReadOnlyList<Stmt> DebugAst()
    {
        var lexer = new StationeerLexer(code);
        var parser = new StationeerParser(lexer);
        return parser.Parse();
    }

    public IReadOnlyList<string> Compile(bool optimize = false)
    {
        var lexer = new StationeerLexer(code);
        var parser = new StationeerParser(lexer);
        var ast = parser.Parse();
        var semantic = new SemanticAnalyzer();

        try
        {
            semantic.Analyze(ast);
        }
        catch (Exception ex)
        {
            CompileErrors.Add(ex.Message);
            return [];
        }

        var irBuilder = new IrBuilder();
        var ir = irBuilder.Build(ast, semantic.DeviceReferences);

        if (!optimize) return CodeGenerator.Generate(ir);

        var irOptimizer = new IrOptimizer();
        var optimizedIr = irOptimizer.Optimize(ir);
        return CodeGenerator.Generate(optimizedIr);
    }
}