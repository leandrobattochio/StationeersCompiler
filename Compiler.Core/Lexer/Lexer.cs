using Compiler.Lexer.Tokens;

namespace Compiler.Lexer;

public class StationeerLexer
{
    private readonly string _src;
    private int _pos;

    public StationeerLexer(string src) => _src = src;

    private char Current => _pos < _src.Length ? _src[_pos] : '\0';
    private void Advance() => _pos++;

    public Token NextToken()
    {
        while (char.IsWhiteSpace(Current))
            Advance();

        if (Current == '\0')
            return new EndOfFileToken(_pos);

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
                return new FloatToken(floatText, start);
            }

            var intText = _src[start.._pos];
            return new NumberToken(intText, start);
        }

        // identifiers / keywords
        if (!char.IsLetter(Current))
            return Current switch
            {
                '=' => HandleEquals(),
                '+' => ConsumeAnd(new PlusToken(_pos)),
                '-' => ConsumeAnd(new MinusToken(_pos)),
                '*' => ConsumeAnd(new MultiplyToken(_pos)),
                '/' => ConsumeAnd(new DivideToken(_pos)),
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
                _ => new LexerErrorToken($"Unexpected char '{Current}'", _pos)
            };
        {
            var start = _pos;
            while (char.IsLetterOrDigit(Current))
                Advance();

            var text = _src[start.._pos];

            return text switch
            {
                "var" => new VarKeywordToken(start),
                "true" => new TrueKeywordToken(start),
                "false" => new FalseKeywordToken(start),
                "if" => new IfKeywordToken(start),
                "else" => new ElseKeywordToken(start),
                _ => new IdentifierToken(text, start)
            };
        }

        // symbols
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

    public void DebugTokens()
    {
        Token token;
        while (true)
        {
            token = NextToken();
            Console.WriteLine(token);

            if (token.Type is TokenType.EndOfFile or TokenType.CompileError)
                break;
        }

        // Reset
        _pos = 0;
    }
}