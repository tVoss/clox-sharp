using System;
using System.Collections.Generic;

namespace CloxSharp
{
    internal class Scanner {

        private static readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType> {
            {"and",    TokenType.And},
            {"class",  TokenType.Class},
            {"else",   TokenType.Else},
            {"false",  TokenType.False},
            {"for",    TokenType.For},
            {"fun",    TokenType.Fun},
            {"if",     TokenType.If},
            {"nil",    TokenType.Nil},
            {"or",     TokenType.Or},
            {"print",  TokenType.Print},
            {"return", TokenType.Return},
            {"super",  TokenType.Super},
            {"this",   TokenType.This},
            {"true",   TokenType.True},
            {"var",    TokenType.Var},
            {"while",  TokenType.While},
        };

        private readonly string _source;
        private readonly List<Token> _tokens;

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        public Scanner(string source) {
            _source = source;
            _tokens = new List<Token>();
        }

        public List<Token> ScanTokens() {
            while (!IsAtEnd()) {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.Eof, string.Empty, null, _line));
            return _tokens;
        }

        private bool IsAtEnd() {
            return _current >= _source.Length;
        }

        private void ScanToken() {
            var c = Advance();
            switch (c) {

                // One character
                case '(': AddToken(TokenType.LeftParen); break;
                case ')': AddToken(TokenType.RightParen); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case '-': AddToken(TokenType.Minus); break;
                case '+': AddToken(TokenType.Plus); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '*': AddToken(TokenType.Star); break;
                
                // One or two character
                case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
                case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
                case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
                case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;

                // Slash or comment?
                case '/':
                    if (Match('/')) {
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    } else {
                        AddToken(TokenType.Slash);
                    }
                    break;

                // Ignore whitespace
                case ' ':
                case '\r':
                case '\t':
                    break;

                case '\n':
                    _line++;
                    break;
                
                // Handle strings
                case '"': AddString(); break;

                default:
                    if (IsDigit(c)) {
                        AddNumber();
                    } else if (IsAlpha(c)) {
                        AddIdentitfier();
                    } else {
                        Lox.Error(_line, "Unexpected character");
                    }
                    break;
            }
        }

        private bool Match(char expected) {
            if (IsAtEnd()) {
                return false;
            }
            if (_source[_current] != expected) {
                return false;
            }

            _current++;
            return true;
        }

        private char Peek() {
            return IsAtEnd()
                ? '\0'
                : _source[_current];
        }

        private char PeekNext() {
            return _current + 1 >= _source.Length
                ? '\0'
                : _source[_current + 1];
        }

        private char Advance() {
            _current++;
            return _source[_current - 1];
        }

        private bool IsDigit(char c) {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c) {
            return (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private bool IsAplhaNumeric(char c) {
            return IsAlpha(c) || IsDigit(c);
        }

        private void AddString() {
            // Go through string
            while (Peek() != '"' && !IsAtEnd()) {
                if (Peek() == '\n') {
                    _line++;
                }
                Advance();
            }

            // String is done but wasn't closed
            if (IsAtEnd()) {
                Lox.Error(_line, "Unterminated string");
                return;
            }

            // Grab the closing "
            Advance();

            var stringValue = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(TokenType.String, stringValue);
        }

        private void AddNumber() {
            // Go through digits
            while (IsDigit(Peek())) {
                Advance();
            }

            // See if there's a fractional portion
            if (Peek() == '.' && IsDigit(PeekNext())) {
                Advance();

                // Keep going
                while (IsDigit(Peek())) {
                    Advance();
                }
            }

            AddToken(TokenType.Number, double.Parse(_source.Substring(_start, _current - _start)));
        }
        
        private void AddIdentitfier() {
            while (IsAplhaNumeric(Peek())) {
                Advance();
            }

            var text = _source.Substring(_start, _current - _start);
            if (!_keywords.TryGetValue(text, out var type)) {
                type = TokenType.Identifier;
            }

            AddToken(type);
        }

        private void AddToken(TokenType type) {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal) {
            var text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }
    }
}