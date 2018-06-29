using CloxSharp.Visitors;

namespace CloxSharp.Expressions {
    public abstract class Expr {
        public abstract T Accept<T>(IVisitor<T> visitor);
    }
}