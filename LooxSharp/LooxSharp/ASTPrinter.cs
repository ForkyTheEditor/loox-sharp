using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooxSharp
{
    public class ASTPrinter : Expr.Visitor<string>
    {

        public string print(Expr expr)
        {
            return expr.accept(this);
        }

        public string visitBinaryExpr(Expr.Binary expr)
        {
            return paranthesize(expr.op.lexeme, expr.left, expr.right);
        }

        public string visitUnaryExpr(Expr.Unary expr)
        {
            return paranthesize(expr.op.lexeme, expr.right);
        }
        public string visitLiteralExpr(Expr.Literal expr)
        {
            if(expr.value == null)
            {
                return "nil";
            }
            return expr.value.ToString();
        }
        public string visitGroupingExpr(Expr.Grouping expr)
        {
            return paranthesize("group", expr.expression);
        }



        private string paranthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(");
            builder.Append(name);
            foreach (Expr expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.accept(this));
            }
            builder.Append(")");


            return builder.ToString();
        }

    }
}
