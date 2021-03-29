using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooxSharp
{
    class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
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
            return null;
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
