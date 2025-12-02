using Compiler.Domain.Ast.Expressions;
using Compiler.Domain.Ast.Statements;
using Compiler.Lexer;
using Compiler.Lexer.Tokens;

namespace Compiler.Parser;

public sealed class StationeerParser
{
    private readonly StationeerLexer _lexer;
    private Token _currentToken;
    private Token? _nextToken;

    public StationeerParser(StationeerLexer lexer)
    {
        _lexer = lexer;
        _currentToken = _lexer.NextToken();
    }

    private Token CurrentToken => _currentToken;

    private Token Peek()
    {
        if (_nextToken == null)
            _nextToken = _lexer.NextToken();
        return _nextToken;
    }

    private void Eat(TokenType type)
    {
        if (CurrentToken.Type == type)
        {
            if (_nextToken != null)
            {
                _currentToken = _nextToken;
                _nextToken = null;
            }
            else
            {
                _currentToken = _lexer.NextToken();
            }
        }
        else
            throw new Exception($"Expected token {type} but got {CurrentToken.Type} on pos {CurrentToken.Position}");
    }

    public List<Stmt> Parse()
    {
        List<Stmt> statements = new();

        while (CurrentToken.Type != TokenType.EndOfFile)
            statements.Add(ParseStatement());

        return statements;
    }

    private Stmt ParseStatement()
    {
        return CurrentToken switch
        {
            VarKeywordToken => ParseVarDeclaration(),
            IfKeywordToken => ParseIfStatement(),
            LeftBraceToken => ParseBlockStatement(),
            IdentifierToken => LooksLikeVarDeclaration() ? ParseVarDeclaration() : ParseExprStatement(),
            _ => ParseExprStatement()
        };
    }

    private bool LooksLikeVarDeclaration()
    {
        // Lookahead: If we see "Identifier Identifier", it's likely a variable declaration
        // Pattern: TypeName variableName = ...
        // This catches both valid types (Int, Float, StationeersDevice) and invalid types (Fdd)
        // Invalid types will be caught by semantic analyzer
        if (CurrentToken.Type != TokenType.Identifier)
            return false;

        // Peek at next token
        var nextToken = Peek();

        // If next is also an identifier, this is likely a type declaration
        // e.g., "StationeersDevice sensor" or "Float temp" or "Int value"
        return nextToken.Type == TokenType.Identifier;
    }

    private bool IsGlobalObject(string name)
    {
        // List of known global objects (static namespaces)
        return name is "Math";
    }

    private Stmt ParseIfStatement()
    {
        Eat(TokenType.Keyword);
        Eat(TokenType.LParen);
        var condition = ParseExpression();
        Eat(TokenType.RParen);

        var thenBranch = ParseStatement();

        Stmt? elseBranch = null;
        if (CurrentToken is ElseKeywordToken)
        {
            Eat(TokenType.Keyword);
            elseBranch = ParseStatement();
        }

        return new IfStmt(condition, thenBranch, elseBranch);
    }

    private Stmt ParseBlockStatement()
    {
        Eat(TokenType.LBrace);
        var statements = new List<Stmt>();

        while (CurrentToken.Type != TokenType.RBrace && CurrentToken.Type != TokenType.EndOfFile)
        {
            statements.Add(ParseStatement());
        }

        Eat(TokenType.RBrace);
        return new BlockStmt(statements);
    }

    private Stmt ParseVarDeclaration()
    {
        // Can be either "var name = ..." or "TypeName name = ..."
        string? explicitType = null;

        if (CurrentToken is VarKeywordToken)
        {
            Eat(TokenType.Keyword);
        }
        else if (CurrentToken.Type == TokenType.Identifier)
        {
            // This is an explicit type declaration
            explicitType = CurrentToken.Value;
            Eat(TokenType.Identifier);
        }
        else
        {
            throw new Exception("Expected 'var' or type name in variable declaration");
        }

        var name = CurrentToken.Value;
        Eat(TokenType.Identifier);
        Eat(TokenType.Equals);

        var initializer = ParseExpression();
        Eat(TokenType.Semicolon);

        return new VarDeclarationStmt(name, initializer, explicitType);
    }

    private Stmt ParseExprStatement()
    {
        var expr = ParseExpression();

        // Handle postfix increment/decrement
        if (expr is IdentifierExpr identifier &&
            CurrentToken.Type is TokenType.PlusPlus or TokenType.MinusMinus)
        {
            var op = CurrentToken.Value; // "++" or "--"
            Eat(CurrentToken.Type);
            expr = new IncrementDecrementExpr(identifier.Name, op, false);
        }

        Eat(TokenType.Semicolon);
        return new ExprStmt(expr);
    }

    public Expr ParseExpression() => ParseAssignment();

    private Expr ParseAssignment()
    {
        var expr = ParseLogicalOr();

        if (CurrentToken.Type == TokenType.Equals)
        {
            // Support both variable assignment and member assignment
            if (expr is IdentifierExpr identifier)
            {
                Eat(TokenType.Equals);
                var value = ParseAssignment();
                return new AssignmentExpr(identifier.Name, value);
            }
            else if (expr is MemberAccessExpr memberAccess)
            {
                Eat(TokenType.Equals);
                var value = ParseAssignment();
                return new MemberAssignmentExpr(memberAccess.Object, memberAccess.MemberName, value);
            }
            else
            {
                throw new Exception("Assignment target must be an identifier or member access");
            }
        }
        else if (CurrentToken.Type is TokenType.PlusEquals or TokenType.MinusEquals
                 or TokenType.MultiplyEquals or TokenType.DivideEquals)
        {
            if (expr is IdentifierExpr identifier)
            {
                var op = CurrentToken.Value[0].ToString(); // Extract '+', '-', '*', or '/'
                Eat(CurrentToken.Type);
                var value = ParseAssignment();
                return new CompoundAssignmentExpr(identifier.Name, op, value);
            }
            else
            {
                throw new Exception("Compound assignment target must be an identifier");
            }
        }

        return expr;
    }

    private Expr ParseLogicalOr()
    {
        var expr = ParseLogicalAnd();

        while (CurrentToken.Type == TokenType.LogicalOr)
        {
            var op = CurrentToken.Value;
            Eat(TokenType.LogicalOr);
            var right = ParseLogicalAnd();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr ParseLogicalAnd()
    {
        var expr = ParseComparison();

        while (CurrentToken.Type == TokenType.LogicalAnd)
        {
            var op = CurrentToken.Value;
            Eat(TokenType.LogicalAnd);
            var right = ParseComparison();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr ParseComparison()
    {
        var expr = ParseTerm();

        while (CurrentToken.Type is TokenType.GreaterThan or TokenType.LessThan
               or TokenType.GreaterThanOrEqual or TokenType.LessThanOrEqual
               or TokenType.EqualsEquals or TokenType.NotEquals)
        {
            var op = CurrentToken.Value;
            Eat(CurrentToken.Type);
            var right = ParseTerm();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr ParseUnary()
    {
        if (CurrentToken.Type is TokenType.Minus)
        {
            Eat(TokenType.Minus);
            var right = ParseUnary();
            return new BinaryExpr(new NumberExpr(0), "-", right);
        }

        // Handle prefix increment/decrement
        if (CurrentToken.Type is TokenType.PlusPlus or TokenType.MinusMinus)
        {
            var op = CurrentToken.Value;
            Eat(CurrentToken.Type);

            if (CurrentToken.Type != TokenType.Identifier)
                throw new Exception("Increment/decrement operator must be followed by an identifier");

            var name = CurrentToken.Value;
            Eat(TokenType.Identifier);

            return new IncrementDecrementExpr(name, op, true);
        }

        return ParsePrimary();
    }

    private Expr ParseTerm()
    {
        var expr = ParseFactor();

        while (CurrentToken.Type is TokenType.Plus or TokenType.Minus)
        {
            var op = CurrentToken.Value;
            Eat(CurrentToken.Type);

            var right = ParseFactor();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr ParseFactor()
    {
        var expr = ParseUnary();

        while (CurrentToken.Type is TokenType.Multiply or TokenType.Divide)
        {
            var op = CurrentToken.Value;
            Eat(CurrentToken.Type);

            var right = ParseUnary();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr ParseNumber()
    {
        var tok = (NumberToken)CurrentToken;
        Eat(TokenType.Number);
        return new NumberExpr(int.Parse(tok.Value));
    }

    private Expr ParseFloat()
    {
        var tok = (FloatToken)CurrentToken;
        Eat(TokenType.Float);
        return new FloatExpr(double.Parse(tok.Value, System.Globalization.CultureInfo.InvariantCulture));
    }

    private Expr ParseIdentifierOrCall()
    {
        var name = CurrentToken.Value;
        Eat(TokenType.Identifier);

        Expr expr = new IdentifierExpr(name);

        // Handle member access and method calls (device.Property or device.method())
        while (CurrentToken.Type == TokenType.Dot)
        {
            Eat(TokenType.Dot);
            var memberName = CurrentToken.Value;
            Eat(TokenType.Identifier);

            // Check if this is a method call (has parentheses)
            if (CurrentToken.Type == TokenType.LParen)
            {
                Eat(TokenType.LParen);
                var args = new List<Expr>();

                if (CurrentToken.Type != TokenType.RParen)
                {
                    do
                    {
                        args.Add(ParseExpression());
                        if (CurrentToken.Type != TokenType.Comma)
                            break;
                        Eat(TokenType.Comma);
                    } while (true);
                }

                Eat(TokenType.RParen);

                // Distinguish between static method call (Math.method) and instance method call (device.method)
                // Check if the base is a known global object
                var isStatic = false;
                string? globalObjectName = null;

                if (expr is IdentifierExpr identExpr)
                {
                    if (IsGlobalObject(identExpr.Name))
                    {
                        isStatic = true;
                        globalObjectName = identExpr.Name;
                    }
                }

                if (isStatic && globalObjectName != null)
                {
                    // This is static method call like Math.convertToCelsius()
                    return new StaticMethodCallExpr(globalObjectName, memberName, args);
                }
                else
                {
                    // This is instance method call like device.setOn()
                    return new MethodCallExpr(expr, memberName, args);
                }
            }

            // This is property access
            expr = new MemberAccessExpr(expr, memberName);
        }

        // Handle function calls
        if (CurrentToken.Type == TokenType.LParen)
        {
            Eat(TokenType.LParen);

            var args = new List<Expr>();

            if (CurrentToken.Type != TokenType.RParen)
            {
                do
                {
                    args.Add(ParseExpression());
                    if (CurrentToken.Type != TokenType.Comma)
                        break;
                    Eat(TokenType.Comma);
                } while (true);
            }

            Eat(TokenType.RParen);
            return new CallExpr(expr, args);
        }

        return expr;
    }

    private Expr ParseGrouping()
    {
        Eat(TokenType.LParen);
        var expr = ParseExpression();
        Eat(TokenType.RParen);
        return new GroupExpr(expr);
    }

    private Expr ParsePrimary()
    {
        return CurrentToken.Type switch
        {
            TokenType.StationeerConstant => ParseStationeerConstant(),
            TokenType.String => ParseString(),
            TokenType.Number => ParseNumber(),
            TokenType.Float => ParseFloat(),
            TokenType.TrueKeyword => ParseBoolean(true),
            TokenType.FalseKeyword => ParseBoolean(false),
            TokenType.Identifier => ParseIdentifierOrCall(),
            TokenType.LParen => ParseGrouping(),
            TokenType.DeviceProperty => ParseDeviceProperty(),
            _ => throw new Exception($"Unexpected token {CurrentToken.Type}")
        };
    }

    private Expr ParseStationeerConstant()
    {
        var tok = (StationeerConstantToken)CurrentToken;
        Eat(TokenType.StationeerConstant);
        return new StationeerConstantExpr(tok.Value);
    }

    private Expr ParseString()
    {
        var tok = (StringToken)CurrentToken;
        Eat(TokenType.String);
        return new StringExpr(tok.Value);
    }

    private Expr ParseBoolean(bool value)
    {
        Eat(value ? TokenType.TrueKeyword : TokenType.FalseKeyword);
        return new BooleanExpr(value);
    }

    private Expr ParseDeviceProperty()
    {
        var propertyName = CurrentToken.Value;
        Eat(TokenType.DeviceProperty);
        return new DevicePropertyExpr(propertyName);
    }
}