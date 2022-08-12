using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ASTClass;
using Sy;
namespace Final
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class Class1
    {


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //KeyInputUTF8();
            //KeyInputASCII();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void KeyInputUTF8()
        {
            // Keyboard input (UTF8) 
            string exp;
            byte[] input;

            while (true)
            {
                Console.Write("> ");
                exp = Console.ReadLine();
                if (exp.Length == 0) break;

                input = Encoding.UTF8.GetBytes(exp);
                byte[] inputUTF8 = new byte[input.Length + 3];
                inputUTF8[0] = 0xEF;
                inputUTF8[1] = 0xBB;
                inputUTF8[2] = 0xBF;
                input.CopyTo(inputUTF8, 3);

                Scanner scanner = new Scanner(new MemoryStream(inputUTF8));
                Parser parser = new Parser(scanner);
                //parser.tab = new SymbolTable(parser);

                parser.Parse();

            }


        }

        static void KeyInputASCII()
        {
            // Keyboard input 
            string exp;
            byte[] input;

            while (true)
            {
                Console.Write("Enter arithmetic expression > ");

                exp = Console.ReadLine();
                if (exp.Length == 0) break;

                input = Encoding.ASCII.GetBytes(exp);
                Scanner scanner = new Scanner(new MemoryStream(input));
                Parser parser = new Parser(scanner);

                parser.Parse();
                if (parser.errors.count == 0)
                {
                    Console.WriteLine("Input is accepted");

                    // Make graphviz dot file of the AST
                    //parser.root.Simplify();
                    parser.root.Simplify2();
                    //parser.root.Children[0].MakeDotFile("AST.dot", exp);
                    //exp = exp.Replace("\"", "\\\"");
                    parser.root.MakeDotFile("AST.dot", exp);

                    // Run the graph viz batch file to generate JPG file from AST.dot
                    System.Diagnostics.Process proc;
                    proc = System.Diagnostics.Process.Start("chartJPG.bat");
                    proc.WaitForExit();

                    // Use the default JPG viewer (e.g. Windows Photo Viewer)
                    System.Diagnostics.Process.Start("AST.jpg");

                }
                else
                    Console.WriteLine("Input is not accepted");

            }

        }
    }

}

