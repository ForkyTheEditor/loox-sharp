using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooxSharp
{
    class Parser
    {
        private class ParseError : Exception { }


        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr parse()
        {
            try
            {
                return expression();
            }
            catch(ParseError err)
            {
                return null;
            }
        }


        private Expr expression()
        {
            return equality();
        }

        private Expr equality()
        {
            Expr expr = comparison();

            while (match(TokenType.EXCLAM_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token op = previous();
                Expr right = comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr comparison()
        {
            Expr expr = term();

            while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = previous();
                Expr right = term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr term()
        {
            Expr expr = factor();

            while(match(TokenType.PLUS, TokenType.MINUS))
            {
                Token op = previous();
                Expr right = factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr factor()
        {
            Expr expr = unary();

            while(match(TokenType.SLASH, TokenType.STAR))
            {
                Token op = previous();
                Expr right = unary();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr unary()
        {
            if (match(TokenType.EXCLAM, TokenType.MINUS))
            {
                Token op = previous();
                Expr right = unary();
                return new Expr.Unary(op, right);
            }
            return primary();
        }

        private Expr primary()
        {
            if (match(TokenType.FALSE)) return new Expr.Literal(false);
            if (match(TokenType.TRUE)) return new Expr.Literal(true);
            if (match(TokenType.NIL)) return new Expr.Literal(null);

            if(match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expr.Literal(previous().literal);
            }

            if (match(TokenType.LEFT_PARNTH))
            {
                Expr expr = expression();
                consume(TokenType.RIGHT_PARNTH, "Expected ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw error(peek(), "Expected expression.");
        }

        private Token consume(TokenType type, string msg)
        {
            if (check(type))
            {
                return advance();
            }

            throw error(peek(), msg);

        }

        private ParseError error(Token token, string msg)
        {
            LooxSharp.error(token, msg);
            return new ParseError();
        }

        private void synchronize()
        {
            advance();

            while (!isAtEnd())
            {
                if(previous().type == TokenType.SEMICOLON)
                {
                    return;
                }

                switch (peek().type)
                {
                    case TokenType.CLASS:
                    case TokenType.FN:
                    case TokenType.LET:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                advance();
            }
        }

        private bool match(params TokenType[] types)
        {
            foreach(TokenType type in types)
            {
                if (check(type))
                {
                    advance();
                    return true;
                }
            }

            return false;
        }

        private bool check(TokenType type)
        {
            if (isAtEnd())
            {
                return false;
            }
            return peek().type == type;
        }

        private Token advance()
        {
            if (!isAtEnd())
            {
                current++;
            }

            return previous();
        }
        
        private bool isAtEnd()
        {
            return peek().type == TokenType.EOF;
        }

        private Token peek()
        {
            return tokens[current];
        }

        private Token previous()
        {
            return tokens[current - 1];
        }

    }
}
