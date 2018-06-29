using CloxSharp.Visitors;

namespace CloxSharp.Expressions {
    public class LiteralExpr : Expr {
        public object Value { get; }
        public LiteralExpr(object value) {
            Value = value;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
}