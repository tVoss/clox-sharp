using CloxSharp.Visitors;

namespace CloxSharp.Expressions {
    public class GroupingExpr : Expr {
        public Expr Expression { get; }
        public GroupingExpr(Expr expression) {
            Expression = expression;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }
}