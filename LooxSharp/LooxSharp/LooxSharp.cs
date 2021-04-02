using System;
using System.IO;
using System.Collections.Generic;

namespace LooxSharp
{
    class LooxSharp
    {
        private static readonly Interpreter interpreter = new Interpreter();
        static bool hadError = false;
        static bool hadRuntimeError = false;
        
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Only 1 argument is taken in! E.g. looxsh [yourFile] ");
                //incorrect number of arguments was used (refer to https://www.freebsd.org/cgi/man.cgi?query=sysexits&apropos=0&sektion=0&manpath=FreeBSD+4.3-RELEASE&format=html)
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                //Interpret the file
                runFile(args[0]);
            }
            else
            {
                //Fire up the prompt 
                runCmdPrompt();
            }
        }
        /// <summary>
        /// Runs the specified looxsh file.
        /// </summary>
        /// <param name="file"></param>
        private static void runFile(string filePath)
        {
            byte[] bytesInFile = File.ReadAllBytes(filePath);
            run(System.Text.Encoding.Default.GetString(bytesInFile));

            //Check if the script had any errors
            if(hadError == true)
            {
                Environment.Exit(65);
            }
            if(hadRuntimeError == true)
            {
                Environment.Exit(70);
            }

        }

        private static void runCmdPrompt()
        {
            Console.WriteLine("LooxSharp Interpreter");

            while (true)
            {
                Console.Write(">>> ");

                string input = Console.ReadLine();
                if (input == null)
                {
                    break;
                }

                run(input);
                //Reset the error flag
                hadError = false;
            }
        }

        private static void run(String code)
        {
            Scanner scanner = new Scanner(code);
            List<Token> tokens = scanner.scanTokens();
            Parser parser = new Parser(tokens);
            Expr expression = parser.parse();

            if (hadError)
            {
                return;
            }

            interpreter.interpret(expression);
         

        }

        public static void runtimeError(RuntimeError err)
        {
            Console.Error.WriteLine(err.Message + "\n[line " + err.token.line + "]");
            hadRuntimeError = true;
        }

        public static void error(int line, string message)
        {
            report(line, "", message);
        }

        public static void error(Token token, string message)
        {
            if(token.type == TokenType.EOF)
            {
                report(token.line, " at end", message);
            }
            else
            {
                report(token.line, " at '" + token.lexeme + "'", message);
            }
        }

        private static void report(int line, string where, string message)
        {
            Console.WriteLine("[line " + line + "] Error " +  where + ": " + message);
            hadError = true;
        }


    }
}
