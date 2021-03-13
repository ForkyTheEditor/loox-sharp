using System;
using System.IO;
using System.Collections.Generic;

namespace LooxSharp
{
    class LooxSharp
    {
        static bool hadError = false;

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

        }

        private static void runCmdPrompt()
        {
            Console.WriteLine("LooxSharp Interpreter");

            while (true)
            {
                Console.Write("> ");

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

            foreach (Token tkn in tokens)
            {
                Console.WriteLine(tkn);
            }

        }

        public static void error(int line, string message)
        {
            report(line, "", message);
        }

        private static void report(int line, string where, string message)
        {
            Console.WriteLine("[line " + line + "] Error " +  where + ": " + message);
            hadError = true;
        }


    }
}
