using System;
using System.Collections.Generic;

namespace CloxSharp
{
    internal class Scanner {

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
            }
        }

        private char Advance() {
            _current++;
            return _source[_current - 1];
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