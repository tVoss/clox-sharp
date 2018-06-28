using System;
using System.Collections.Generic;
using CloxSharp.Exceptions;
using CloxSharp.Expressions;

namespace CloxSharp{
    public class Parser {
        private readonly List<Token> _tokens;
        private int _current;

        public Parser(List<Token> tokens) {
            _tokens = tokens;
            _current = 0;
        }

        public Expr Parse() {
            try {
                return Expression();
            } catch (ParseException e) {
                return null;
            }
        }

        private Expr Expression() {
            return Equality();
        }

        private Expr Equality() {
            var expr = Comparison();

            while (Match(TokenType.BangEqual, TokenType.EqualEqual)) {
                var op = Previous();
                var right = Comparison();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private Expr Comparison() {
            var expr = Addition();

            while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual)) {
                var op = Previous();
                var right = Addition();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private Expr Addition() {
            var expr = Multiplication();

            while (Match(TokenType.Minus, TokenType.Plus)) {
                var op = Previous();
                var right = Multiplication();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private Expr Multiplication() {
            var expr = Unary();

            while (Match(TokenType.Slash, TokenType.Star)) {
                var op = Previous();
                var right = Unary();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private Expr Unary() {
            if (Match(TokenType.Bang, TokenType.Minus)) {
                var op = Previous();
                var right = Unary();
                return new UnaryExpr(op, right);
            }

            return Primary();
        }

        private Expr Primary() {
            if (Match(TokenType.False)) return new LiteralExpr(false);
            if (Match(TokenType.True)) return new LiteralExpr(true);
            if (Match(TokenType.Nil)) return new LiteralExpr(null);

            if (Match(TokenType.Number, TokenType.String)) {
                return new LiteralExpr(Previous().Literal);
            }

            if (Match(TokenType.LeftParen)) {
                var expr = Expression();
                Consume(TokenType.RightParen, "Expect ')' after expression");
                return new GroupingExpr(expr);
            }

            // Err?
            throw Error(Peek(), "Expect expression");
        }

        private Token Consume(TokenType type, string message) {
            if (Check(type)) {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private ParseException Error(Token token, string message) {
            Lox.Error(token, message);
            return new ParseException();
        }

        private void Synchronize() {
            Advance();

            while (!IsAtEnd()) {
                if (Previous().Type == TokenType.Semicolon) return;

                switch (Peek().Type) {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Print:
                    case TokenType.Return:
                        return;
                }

                Advance();
            }
        }

        private bool Match(params TokenType[] types) {
            foreach (var type in types) {
                if (Check(type)) {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type) {
            if (IsAtEnd()) {
                return false;
            }
            return Peek().Type == type;
        }

        private Token Advance() {
            if (!IsAtEnd()) {
                _current++;
            }
            return Previous();
        }

        private bool IsAtEnd() {
            return Peek().Type == TokenType.Eof;
        }

        private Token Peek() {
            return _tokens[_current];
        }

        private Token Previous() {
            return _tokens[_current  -1];
        }

    }
}