using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooxSharp
{
    class Scanner
    {
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            { "and", TokenType.AND},
            { "or", TokenType.OR},
            { "class", TokenType.CLASS},
            { "if", TokenType.IF},
            { "else", TokenType.ELSE},
            { "fn", TokenType.FN},
            { "for", TokenType.FOR},
            { "nil", TokenType.NIL},
            { "print", TokenType.PRINT},
            { "return", TokenType.RETURN},
            { "parent", TokenType.PARENT},
            { "this", TokenType.THIS},
            { "true", TokenType.TRUE},
            { "false", TokenType.FALSE},
            { "let", TokenType.LET},
            { "while", TokenType.WHILE}
        };

        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();

        //Fields tracking where the scanner is in the file
        private int start = 0;
        private int current = 0;
        private int line = 1;
        
        private bool isAtEnd { get { return current >= source.Length; } } //Are we at the end of the file?

        

        public Scanner(string src)
        {
            this.source = src;
        }

        public List<Token> scanTokens()
        {
            //The scan loop
            while (!isAtEnd)
            {
                //Beginning of next lexeme
                start = current;
                scanToken();
            }

            //Add the end of file token
            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;

        }

        private void scanToken()
        {
            char c = advanceScanner();

            switch (c)
            {
                //Single characters
                case '(': addToken(TokenType.LEFT_PARNTH); break;
                case ')': addToken(TokenType.RIGHT_PARNTH); break;
                case '{': addToken(TokenType.LEFT_BRACE); break;
                case '}': addToken(TokenType.RIGHT_BRACE); break;
                case ',': addToken(TokenType.COMMA); break;
                case '.': addToken(TokenType.DOT); break;
                case '-': addToken(TokenType.MINUS); break;
                case '+': addToken(TokenType.PLUS); break;
                case ';': addToken(TokenType.SEMICOLON); break;
                case '*': addToken(TokenType.STAR); break;

                //Double characters
                case '!': addToken(match('=') ? TokenType.EXCLAM_EQUAL : TokenType.EXCLAM); break;
                case '=': addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;

                //Divide and comment
                case '/':
                    if (match('/'))
                    {
                        while (peek() != '\n' && !isAtEnd) { advanceScanner(); }
                    }
                    else { addToken(TokenType.SLASH); }
                    break;

                //Whitespace
                case ' ':
                    break;
                case '\t':
                    break;
                case '\r':
                    break;
                case '\n':
                    line++;
                    break;

                //Handle strings
                case '"': handleString(); break;

                default:
                    //Check for digits (for number literals)
                    if (basicIsDigit(c))
                    {
                        handleNumber();
                    }
                    else if (basicIsAlpha(c))
                    {
                        handleIdentifier();
                    }
                    else
                    {
                        LooxSharp.error(line, "Unexpected character.");
                    }
                    break;

            }
        }

        /// <summary>
        /// Checks if the next character is the expected one
        /// </summary>
        /// <param name="expectedChar"></param>
        /// <returns></returns>
        private bool match(char expectedChar)
        {
            if (isAtEnd) { return false; }
            if (source[current] != expectedChar) { return false; }

            current++;
            return true;
        }

        //Peeks at the next character, doesn't advance the scanner
        private char peek()
        {
            if (isAtEnd)
            {
                return '\0';
            }
            return source[current];
        }

        private char peekNext()
        {
            if (current + 1 >= source.Length)
            {
                return '\0';
            }
            return source[current + 1];
        }

        private void handleString()
        {
            while (peek() != '"' && !isAtEnd)
            {
                if (peek() == '\n')
                {
                    line++;
                }
                advanceScanner();
            }

            if (isAtEnd)
            {
                LooxSharp.error(line, "Unterminated string.");
                return;
            }

            //The closing "
            advanceScanner();

            //Remove the " "
            string value = source.Substring(start + 1, current - start - 2);

            addToken(TokenType.STRING, value);
        }


        private void handleNumber()
        {
            while (basicIsDigit(peek()))
            {
                advanceScanner();
            }

            //Check if it has fractional part (and read it entirely)
            if (peek() == '.' && basicIsDigit(peekNext()))
            {
                advanceScanner();

                while (basicIsDigit(peek())) { advanceScanner(); }
            }

            addToken(TokenType.NUMBER, double.Parse(source.Substring(start, current - start)));
        }

        private void handleIdentifier()
        {
            while (basicIsAlphaNumeric(peek()))
            {
                advanceScanner();
            }

            string text = source.Substring(start, current - start);
            TokenType type;
            if(!keywords.TryGetValue(text,out type))
            {
                type = TokenType.IDENTIFIER;
            }
            
            addToken(type);
        }

        private bool basicIsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
        private bool basicIsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c<= 'Z') || c == '_';
        }
        private bool basicIsAlphaNumeric(char c)
        {
            return basicIsDigit(c) || basicIsAlpha(c);
        }
        

        private char advanceScanner()
        {
            //Read current, then increment
            return source[current++];
        }

        private void addToken(TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(TokenType type, object literal)
        {
            //Get the lexeme 
            string lexeme = source.Substring(start, current - start);

            tokens.Add(new Token(type, lexeme, literal, line));
        }

    }
}
