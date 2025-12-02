using System.Diagnostics;
using Compiler.Lexer.Tokens;

namespace Compiler.Lexer;

public class StationeerLexer
{
    private readonly string _src;
    private int _pos;
    private Token? _lastToken;

    public StationeerLexer(string src) => _src = src;

    private char Current => _pos < _src.Length ? _src[_pos] : '\0';
    private char Peek() => _pos + 1 < _src.Length ? _src[_pos + 1] : '\0';

    private void Advance() => _pos++;

    private void AdvanceUntilNextLine()
    {
        while (Current != '\n' && Current != '\0')
            Advance();

        while (char.IsWhiteSpace(Current))
            Advance();
    }

    public Token NextToken()
    {
        while (char.IsWhiteSpace(Current))
            Advance();

        while (Current == '#')
            AdvanceUntilNextLine();
        
        if (Current == '\0')
        {
            var token = new EndOfFileToken(_pos);
            _lastToken = token;
            return token;
        }

        Token result;

        // numbers (int or float)
        if (char.IsDigit(Current))
        {
            var start = _pos;
            while (char.IsDigit(Current))
                Advance();

            // Check for decimal point (float)
            if (Current == '.' && _pos + 1 < _src.Length && char.IsDigit(_src[_pos + 1]))
            {
                Advance(); // consume '.'
                while (char.IsDigit(Current))
                    Advance();

                var floatText = _src[start.._pos];
                result = new FloatToken(floatText, start);
            }
            else
            {
                var intText = _src[start.._pos];
                result = new NumberToken(intText, start);
            }

            _lastToken = result;
            return result;
        }

        // identifiers / keywords
        if (!char.IsLetter(Current))
        {
            result = Current switch
            {
                '"' => HandleQuote(),
                '=' => HandleEquals(),
                '+' => HandlePlus(),
                '-' => HandleMinus(),
                '*' => HandleMultiply(),
                '/' => HandleDivide(),
                '(' => ConsumeAnd(new LeftParentToken(_pos)),
                ')' => ConsumeAnd(new RightParentToken(_pos)),
                ',' => ConsumeAnd(new CommaToken(_pos)),
                ';' => ConsumeAnd(new SemicolonToken(_pos)),
                '{' => ConsumeAnd(new LeftBraceToken(_pos)),
                '}' => ConsumeAnd(new RightBraceToken(_pos)),
                '.' => ConsumeAnd(new DotToken(_pos)),
                '>' => HandleGreaterThan(),
                '<' => HandleLessThan(),
                '!' => HandleExclamation(),
                '&' => HandleAmpersand(),
                '|' => HandlePipe(),
                _ => throw new Exception($"Unexpected char '{Current}' on pos {_pos}")
            };

            _lastToken = result;
            return result;
        }

        var start2 = _pos;
        while (char.IsLetterOrDigit(Current))
            Advance();

        var text = _src[start2.._pos];

        result = text switch
        {
            "var" => new VarKeywordToken(start2),
            "true" => new TrueKeywordToken(start2),
            "false" => new FalseKeywordToken(start2),
            "if" => new IfKeywordToken(start2),
            "else" => new ElseKeywordToken(start2),
            _ => new IdentifierToken(text, start2)
        };

        _lastToken = result;
        return result;
    }


    private Token HandleQuote()
    {
        var start = _pos;
        Advance(); // consume opening quote

        var strContentStart = _pos;
        while (Current != '"' && Current != '\0')
            Advance();

        if (Current == '\0')
            return new LexerErrorToken("Unterminated string literal", start);

        var strContent = _src[strContentStart.._pos];
        Advance(); // consume closing quote

        return new StringToken(strContent, start);
    }

    private Token ConsumeAnd(Token t)
    {
        Advance();
        return t;
    }

    private Token HandleEquals()
    {
        var start = _pos;
        Advance();
        if (Current != '=') return new EqualsToken(start);
        Advance();
        return new EqualsEqualsToken(start);
    }

    private Token HandleGreaterThan()
    {
        var start = _pos;
        Advance();
        if (Current != '=') return new GreaterThanToken(start);
        Advance();
        return new GreaterThanOrEqualToken(start);
    }

    private Token HandleLessThan()
    {
        var start = _pos;
        Advance();
        if (Current != '=') return new LessThanToken(start);
        Advance();
        return new LessThanOrEqualToken(start);
    }

    private Token HandleExclamation()
    {
        var start = _pos;
        Advance();
        if (Current != '=') return new LexerErrorToken("Unexpected '!'", start);
        Advance();
        return new NotEqualsToken(start);
    }

    private Token HandleAmpersand()
    {
        var start = _pos;
        Advance();
        if (Current != '&') return new LexerErrorToken("Unexpected '&'", start);
        Advance();
        return new LogicalAndToken(start);
    }

    private Token HandlePipe()
    {
        var start = _pos;
        Advance();
        if (Current != '|') return new LexerErrorToken("Unexpected '|'", start);
        Advance();
        return new LogicalOrToken(start);
    }

    private Token HandlePlus()
    {
        var start = _pos;
        Advance();
        if (Current == '=')
        {
            Advance();
            return new PlusEqualsToken(start);
        }

        if (Current == '+')
        {
            Advance();
            return new PlusPlusToken(start);
        }

        return new PlusToken(start);
    }

    private Token HandleMinus()
    {
        var start = _pos;

        // Check if this could be a negative number
        // A minus is part of a negative number if:
        // 1. Next character is a digit
        // 2. Previous token suggests this is not a subtraction operator
        if (char.IsDigit(Peek()) && ShouldTreatAsNegativeNumber())
        {
            Advance(); // consume '-'
            return ParseNumber(start);
        }

        Advance();
        if (Current == '=')
        {
            Advance();
            return new MinusEqualsToken(start);
        }

        if (Current == '-')
        {
            Advance();
            return new MinusMinusToken(start);
        }

        return new MinusToken(start);
    }

    private bool ShouldTreatAsNegativeNumber()
    {
        // If there's no previous token, this is the start of the input - treat as negative number
        if (_lastToken == null)
            return true;

        // After these tokens, a minus should be treated as part of a negative number
        return _lastToken.Type is
            TokenType.LParen or
            TokenType.Comma or
            TokenType.Equals or
            TokenType.PlusEquals or
            TokenType.MinusEquals or
            TokenType.MultiplyEquals or
            TokenType.DivideEquals or
            TokenType.EqualsEquals or
            TokenType.NotEquals or
            TokenType.LessThan or
            TokenType.LessThanOrEqual or
            TokenType.GreaterThan or
            TokenType.GreaterThanOrEqual or
            TokenType.LogicalAnd or
            TokenType.LogicalOr or
            TokenType.LBrace;
    }

    private Token ParseNumber(int start)
    {
        while (char.IsDigit(Current))
            Advance();

        // Check for decimal point (float)
        if (Current == '.' && _pos + 1 < _src.Length && char.IsDigit(_src[_pos + 1]))
        {
            Advance(); // consume '.'
            while (char.IsDigit(Current))
                Advance();

            var floatText = _src[start.._pos];
            return new FloatToken(floatText, start);
        }

        var intText = _src[start.._pos];
        return new NumberToken(intText, start);
    }

    private Token HandleMultiply()
    {
        var start = _pos;
        Advance();
        if (Current == '=')
        {
            Advance();
            return new MultiplyEqualsToken(start);
        }

        return new MultiplyToken(start);
    }

    private Token HandleDivide()
    {
        var start = _pos;
        Advance();
        if (Current == '=')
        {
            Advance();
            return new DivideEqualsToken(start);
        }

        return new DivideToken(start);
    }

    public IReadOnlyList<Token> Tokenize()
    {
        var tokens = new List<Token>();
        while (true)
        {
            var token = NextToken();
            tokens.Add(token);

            if (token.Type is TokenType.EndOfFile or TokenType.CompileError)
                break;
        }

        _pos = 0;
        return tokens;
    }
}