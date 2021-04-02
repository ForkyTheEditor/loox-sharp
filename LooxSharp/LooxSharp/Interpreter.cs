using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooxSharp
{
    public class Interpreter : Expr.Visitor<object>
    {
        
        public void interpret(Expr expr)
        {
            try
            {
                object value = evaluate(expr);
                Console.WriteLine(stringify(value));
            }
            catch(RuntimeError err)
            {
                LooxSharp.runtimeError(err);
            }
        }

        private string stringify(object val)
        {
            if (val == null) return "nil";

            if(val is double)
            {
                string text = val.ToString();
                if (text.EndsWith(".0"))
                {
                    //Strip the unnecessary .0
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }
            return val.ToString();
        }

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
                    checkNumberOperand(expr.op, left, right);
                    if ((double)right == 0) throw new RuntimeError(expr.op, "Division by 0 not permitted!");
                    return (double)left / (double)right;
                case TokenType.MINUS:
                    checkNumberOperand(expr.op, left, right);
                    return (double)left - (double)right;
                case TokenType.STAR:
                    checkNumberOperand(expr.op, left, right);
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
                    throw new RuntimeError(expr.op, "Operands must be 2 numbers or 2 strings!");
                case TokenType.GREATER:
                    checkNumberOperand(expr.op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperand(expr.op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    checkNumberOperand(expr.op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    checkNumberOperand(expr.op, left, right);
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

        private void checkNumberOperand(Token op, object left, object right)
        {
            if (left is double && right is double) return;

            throw new RuntimeError(op, "Operands must be numbers!");
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
