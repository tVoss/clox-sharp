using CloxSharp.Expressions;

namespace CloxSharp.Visitors {
    public class Interpreter : IVisitor<object>
    {
        public object VisitBinaryExpr(BinaryExpr expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.Operator.Type) {
                // Arithmetic
                case TokenType.Minus:
                    return (double) left - (double) right;
                case TokenType.Slash:
                    return (double) left / (double) right;
                case TokenType.Star:
                    return (double) left * (double) right;
                case TokenType.Plus:
                    if (left is double && right is double) {
                        return (double) left + (double) right;
                    }
                    if (left is string && right is string) {
                        return (string) left + (string) right;
                    }
                    break;
                
                // Comparison
                case TokenType.Greater:
                    return (double) left > (double) right;
                case TokenType.GreaterEqual:
                    return (double) left >= (double) right;
                case TokenType.Less:
                    return (double) left < (double) right;
                case TokenType.LessEqual:
                    return (double) left <= (double) right;

                case TokenType.BangEqual: return !IsEqual(left, right);
                case TokenType.EqualEqual: return IsEqual(left, right);
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
                    return -(double)right;
            }

            return null;
        }

        private bool IsTruthy(object value) {
            if (value == null) return false;
            if (value is bool) return (bool) value;
            return true;
        }

        private bool IsEqual(object left, object right) {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }
        
        private object Evaluate(Expr expr) {
            return expr.Accept(this);
        }
    }
}