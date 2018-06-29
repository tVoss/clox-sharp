using CloxSharp.Visitors;

namespace CloxSharp.Expressions {
    public class UnaryExpr : Expr {
        public Token Operator { get; }
        public Expr Right { get; }
        public UnaryExpr(Token op, Expr right) {
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }
}