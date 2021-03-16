using System;
using System.Collections.Generic;

namespace LooxSharp
{

    public abstract class Expr
    {
        public interface Visitor<T>
        {
            T visitBinaryExpr(Binary expr);
            T visitUnaryExpr(Unary expr);
            T visitGroupingExpr(Grouping expr);
            T visitLiteralExpr(Literal expr);
        }
        public class Binary : Expr
        {
            Binary(Expr left, Token op, Expr right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public readonly Expr left;
            public readonly Token op;
            public readonly Expr right;

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitBinaryExpr(this);
            }
        }
        public class Unary : Expr
        {
            Unary(Token op, Expr right)
            {
                this.op = op;
                this.right = right;
            }

            public readonly Token op;
            public readonly Expr right;

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitUnaryExpr(this);
            }
        }
        public class Grouping : Expr
        {
            Grouping(Expr expression)
            {
                this.expression = expression;
            }

            public readonly Expr expression;

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitGroupingExpr(this);
            }
        }
        public class Literal : Expr
        {
            Literal(Object value)
            {
                this.value = value;
            }

            public readonly Object value;

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitLiteralExpr(this);
            }
        }

        public abstract T accept<T>(Visitor<T> visitor);
    }
}
