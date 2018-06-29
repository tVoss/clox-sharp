using System.Text;
using CloxSharp.Expressions;

namespace CloxSharp.Visitors {
    public class AstPrinter : IVisitor<string>
    {
        public string Print(Expr expr) {
            return expr.Accept(this);
        }

        private string Parenthesize(string name, params Expr[] exprs) {
            var builder = new StringBuilder();

            builder.Append('(').Append(name);
            foreach (var expr in exprs) {
                builder.Append(' ');
                builder.Append(expr.Accept(this));
            }
            builder.Append(')');

            return builder.ToString();
        }

        public string VisitBinaryExpr(BinaryExpr expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(GroupingExpr expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(LiteralExpr expr)
        {
            return expr.Value == null
                ? "nil"
                : expr.Value.ToString();
        }

        public string VisitUnaryExpr(UnaryExpr expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }
    }
}