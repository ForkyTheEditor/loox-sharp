using System;
using System.Collections.Generic;
using System.IO;

namespace LooxSharpTools
{
    /// <summary>
    /// Generates the AST classes. Tool for automation purposes.
    /// </summary>
    class GenerateAST
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                
                Console.Error.WriteLine("Incorrect no. of arguments. Usage: generateast <output dir>");
                Environment.Exit(64);
            }
            
            string outputDir = args[0];

            defineAst(outputDir, "Expr", new List<string> {
                "Binary : Expr left, Token op, Expr right", 
                "Unary : Token op, Expr right",
                "Grouping : Expr expression",
                "Literal : Object value"});

        }

        private static void defineAst(string output, string baseName, List<string> types)
        {
            string path = output + "\\" + baseName + ".cs";

            StreamWriter writer = new StreamWriter(path);

            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine();
            writer.WriteLine("namespace LooxSharp {");
            writer.WriteLine();
            writer.WriteLine("public abstract class " + baseName + " {");

            defineVisitor(writer, baseName, types);

            foreach(string type in types)
            {
                string className = type.Split(':')[0].Trim();
                string fields = type.Split(':')[1].Trim();

                defineType(writer, baseName, className, fields);
            }
            

            writer.WriteLine();
            writer.WriteLine("  public abstract T accept<T>(Visitor<T> visitor);");

            //Close class
            writer.WriteLine("}");
            //Close namespace
            writer.WriteLine("}");

            writer.Close();

        }

        private static void defineType(StreamWriter writer, string baseName, string className, string fields)
        {
            writer.WriteLine("    public class " + className + " : " + baseName + " {");

            //print the constructor
            writer.WriteLine("      public " + className + "(" + fields + ") { " );

            string[] separateFields = fields.Split(", ");
            //Fill constructor
            foreach(string field in separateFields)
            {
                string name = field.Split(" ")[1];
                writer.WriteLine("          this." + name + " = " + name + ";");
            }

            writer.WriteLine("      }");

            //Print fields
            writer.WriteLine();

            foreach(string field in separateFields)
            {
                writer.WriteLine("      public readonly " + field + ";");
            }
            
            writer.WriteLine();
            writer.WriteLine("      public override T accept<T>(Visitor<T> visitor) {");
            writer.WriteLine("      return visitor.visit" + className + baseName + "(this);");
            writer.WriteLine("}");

            //Close class
            writer.WriteLine("}");
        }

        private static void defineVisitor(StreamWriter writer, string baseName, List<string> types )
        {
            writer.WriteLine("      public interface Visitor<T> {");

            foreach(string type in types)
            {
                string typename = type.Split(":")[0].Trim();
                writer.WriteLine("      T visit" + typename + baseName + "(" + typename + " " + baseName.ToLower() + ");");
            }

            writer.WriteLine("      }");
        }


    }

}
