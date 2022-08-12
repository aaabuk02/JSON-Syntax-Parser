using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ASTClass;

namespace Final
{
    public partial class Form1 : Form
    {
        bool isASTDisplay = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            string parse = "";
            byte[] input;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog1.FileName);
                    using (sr)
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            parse += line;
                        }
                    }
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                }
                Regex.Replace(parse, @"[^\u0020-\u007E]", string.Empty);

                Console.WriteLine(parse);
                this.textBoxInput.Text = parse;

                if (parse.Length == 0) return;

                input = Encoding.UTF8.GetBytes(parse);
                byte[] inputUTF8 = new byte[input.Length + 3];
                inputUTF8[0] = 0xEF;
                inputUTF8[1] = 0xBB;
                inputUTF8[2] = 0xBF;
                input.CopyTo(inputUTF8, 3);

                Scanner scanner = new Scanner(new MemoryStream(inputUTF8));
                Parser parser = new Parser(scanner);
                parser.Parse();

                if (parser.errors.count == 0)
                {
                    Console.WriteLine("Input is accepted");
                    textBoxResult.Text = ("Input is accepted");
                    if (isASTDisplay)
                    {
                        // Make graphviz dot file of the AST
                        parser.root.Simplify2();
                        parse = parse.Replace("\"", "\\\"");
                        if (parse.Length > 20)
                            parser.root.MakeDotFile("AST.dot", "JSON");
                        else
                            parser.root.MakeDotFile("AST.dot", parse);
                        System.Diagnostics.Process proc;
                        proc = System.Diagnostics.Process.Start("chartJPG.bat");
                        proc.WaitForExit();
                        System.Diagnostics.Process.Start("AST.jpg");
                    }
                }
                else
                {
                    Console.WriteLine("Input is not accepted");
                    textBoxResult.Text = parse + " ==> Invalid syntax input";


                }

            }

        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {

            string input;

            input = textBoxInput.Text;

            Regex.Replace(input, @"[^\u0020-\u007E]", string.Empty);
            Console.WriteLine(input);
            byte[] input2, input3;
            UTF8Encoding en = new UTF8Encoding();
            input2 = en.GetBytes(input);

            // Convert to UTF8 input
            input3 = new byte[input2.Length + 3];
            input3[0] = 0xEF;
            input3[1] = 0xBB;
            input3[2] = 0xBF;
            for (int i = 0; i < input2.Length; ++i)
                input3[i + 3] = input2[i];

            //Console.WriteLine(input);
            Scanner scanner = new Scanner(new MemoryStream(input3));
            Parser parser = new Parser(scanner);
            parser.Parse();



            if (parser.errors.count == 0)
            {
                Console.WriteLine("Input is accepted");
                textBoxResult.Text = ("Input is accepted");
                if (isASTDisplay)
                {

                    parser.root.Simplify2();
                    input = input.Replace("\"", "\\\"");


                    if (input.Length > 20)
                        parser.root.MakeDotFile("AST.dot", "JSON");
                    else
                        parser.root.MakeDotFile("AST.dot", input);

                    System.Diagnostics.Process proc;
                    proc = System.Diagnostics.Process.Start("chartJPG.bat");
                    proc.WaitForExit();

                    System.Diagnostics.Process.Start("AST.jpg");
                }
            }
            else
            {
                Console.WriteLine("Input is not accepted");
                textBoxResult.Text = input + " ==> Invalid syntax input";
            }
        }
        
        private void buttonATS_Click(object sender, EventArgs e)
        {
            if (isASTDisplay)
            {
                isASTDisplay = false;
                buttonATS.Text = "AST Display Off";
            }
            else
            {
                isASTDisplay = true;
                buttonATS.Text = "AST Display On";
            }
        }
    }
}





