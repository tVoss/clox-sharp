using System;
using CloxSharp.Exceptions;
using CloxSharp.Expressions;

namespace CloxSharp.Visitors {
    public class Interpreter : IVisitor<object> {

        public void Interpret(Expr expression) {
            try {
                var value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            } catch (RuntimeException e) {
                Lox.RuntimeError(e);
            }
        }

        private string Stringify(object value) {
            if (value == null) {
                return "nil";
            }
            if (value is double) {
                var text = value.ToString();
                if (text.EndsWith(".0")) {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }
            return value.ToString();
        }

        public object VisitBinaryExpr(BinaryExpr expr) {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.Operator.Type) {
                // Arithmetic
                case TokenType.Minus:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double) left - (double) right;

                case TokenType.Slash:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double) left / (double) right;

                case TokenType.Star:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double) left * (double) right;

                case TokenType.Plus:
                    if (left is double && right is double) {
                        return (double) left + (double) right;
                    }
                    if (left is string && right is string) {
                        return (string) left + (string) right;
                    }
                    throw new RuntimeException(expr.Operator, "Operands must be numbers or strings");
                
                // Comparison
                case TokenType.Greater:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double) left > (double) right;

                case TokenType.GreaterEqual:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double) left >= (double) right;

                case TokenType.Less:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double) left < (double) right;

                case TokenType.LessEqual:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double) left <= (double) right;
                    
                case TokenType.BangEqual: 
                    return !IsEqual(left, right);

                case TokenType.EqualEqual:
                    return IsEqual(left, right);
            }

            return null;
        }

        public object VisitGroupingExpr(GroupingExpr expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(LiteralExpr expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(UnaryExpr expr)
        {
            var right = Evaluate(expr.Right);

            switch (expr.Operator.Type) {
                case TokenType.Bang:
                    return !IsTruthy(right);
                case TokenType.Minus:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
            }

            return null;
        }

        private void CheckNumberOperand(Token op, object operand) {
            if (operand is double) {
                return;
            }
            throw new RuntimeException(op, "Operand must be a number");
        }

        private void CheckNumberOperands(Token op, object left, object right) {
            if (left is double && right is double) {
                return;
            }
            throw new RuntimeException(op, "Operands must be numbers");
        }

        private bool IsTruthy(object value) {
            if (value == null) {
                return false;
            }
            if (value is bool) {
                return (bool) value;
            }
            return true;
        }

        private bool IsEqual(object left, object right) {
            if (left == null && right == null) {
                return true;
            }
            if (left == null) {
                return false;
            }
            return left.Equals(right);
        }
        
        private object Evaluate(Expr expr) {
            return expr.Accept(this);
        }
    }
}