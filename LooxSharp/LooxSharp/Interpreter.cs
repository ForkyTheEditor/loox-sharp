using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooxSharp
{
    public class Interpreter : Expr.Visitor<object>
    {
        
        public object visitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            return evaluate(expr.expression);
        }

        public object visitUnaryExpr(Expr.Unary expr)
        {
            object right = evaluate(expr);

            switch (expr.op.type)
            {
                //Return the negated object
                case TokenType.EXCLAM:
                    return !isTruthy(expr);
                case TokenType.MINUS:
                    checkNumberOperand(expr.op, right);
                    return -(double)right;
            }

            return null;
        }

        public object visitBinaryExpr(Expr.Binary expr)
        {
            object left = evaluate(expr.left);
            object right = evaluate(expr.right);

            switch (expr.op.type)
            {
                case TokenType.SLASH:
                    return (double)left / (double)right;
                case TokenType.MINUS:
                    return (double)left - (double)right;
                case TokenType.STAR:
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    if(left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }
                    if(left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    break;
                case TokenType.GREATER:
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    return (double)left <= (double)right;
                case TokenType.EQUAL_EQUAL:
                    return isEqual(left, right);
                case TokenType.EXCLAM_EQUAL:
                    return !isEqual(left, right);
            }




            return null;
        }

        private void checkNumberOperand(Token op, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }




        //False and null are false, every other object is true
        //how cool is it that this is just 1 function
        private bool isTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool isEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        private object evaluate(Expr expr)
        {
            return expr.accept(this);
        }

    }
}
