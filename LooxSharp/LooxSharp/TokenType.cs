using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooxSharp
{
    enum TokenType
    {
        //Single characters
        LEFT_PARNTH, RIGHT_PARNTH, LEFT_BRACE, RIGHT_BRACE, COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR, 

        //1-2 characters
        EXCLAM, EXCLAM_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,
        
        //Literals
        IDENTIFIER, STRING, NUMBER,

        //Keywords
        AND, OR, CLASS, IF, ELSE, FN, FOR, NIL, PRINT, RETURN, PARENT, THIS, TRUE, FALSE, LET, WHILE,

        EOF

    }
}
