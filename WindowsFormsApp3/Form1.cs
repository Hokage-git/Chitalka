using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Globalization;
using System.Diagnostics;
using System.Threading;


namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public List<string> pages = new List<string>();
        RichTextBox rich = new RichTextBox();
        OpenFileDialog dialog = new OpenFileDialog();
        int current=0;
        string str;
        string strUtf8;
        string strANSI;

        void parametrs()
        {
            rich.ReadOnly = true;
            rich.Dock = DockStyle.Fill;
            rich.ScrollBars = RichTextBoxScrollBars.None;
            Controls.Add(rich);
            rich.BringToFront();
        }

        ProcessStartInfo Conv_Parametrs(string PandocCommand)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.FileName = "pandoc.exe";
            proc.Arguments = $"{PandocCommand}";
            return proc;
        }

        void SliceOnPages(string str)
        {
            TextFormatFlags flags = TextFormatFlags.Top | TextFormatFlags.Left |
                            TextFormatFlags.WordBreak | TextFormatFlags.NoPadding |
                            TextFormatFlags.TextBoxControl;
            Size textSize = TextRenderer.MeasureText(str, rich.Font, rich.ClientSize, flags);
            int NumberOfpages = textSize.Height / rich.ClientSize.Height;
            if (textSize.Height % rich.ClientSize.Height != 0)
            {
                NumberOfpages += 1;
            }
            if (textSize.Height > rich.Height)
            {
                rich.Text = str;
                rich.Update();

                int FirstCharOfLastShownLine = rich.GetCharIndexFromPosition(new Point(0, rich.ClientSize.Height));
                int ShownLines = rich.GetLineFromCharIndex(FirstCharOfLastShownLine - 1);
                int TotalLines = rich.GetLineFromCharIndex(rich.Text.Length - 1);

                for (int p = 0; p < NumberOfpages; p++)
                {
                    int FirstLineOfPage = (p * ShownLines);
                    int FirstCharOfPage = rich.GetFirstCharIndexFromLine(FirstLineOfPage);
                    if (FirstCharOfPage < 0)
                    {
                        break;
                    }
                    int FirstLineOfNextPage = (p + 1) * ShownLines;
                    FirstLineOfNextPage = (FirstLineOfNextPage > TotalLines) ? TotalLines : FirstLineOfNextPage;
                    int LastCharOfPage = (FirstLineOfNextPage < TotalLines)
                                       ? rich.GetFirstCharIndexFromLine(FirstLineOfNextPage) - 1
                                       : rich.Text.Length;
                    pages.Add(rich.Text.Substring(FirstCharOfPage, LastCharOfPage - FirstCharOfPage));
                }
            }
            else
            {
                pages.Add(str);
            }
            rich.Text = pages.First();
            label1.Text = $"{current + 1}/{(pages.Count)}";
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            rich = new RichTextBox();
            pages.Clear();
            dialog.Filter = ".txt(*.txt)|*.txt|All files(*.*)|*.*";
            dialog.FilterIndex = 2;

            dialog.CheckFileExists = true;
            DialogResult res = dialog.ShowDialog();
            bool Converted = false;
            string extens = Path.GetExtension(dialog.FileName);
            switch (extens)
            {
                case ".docx":
                    string FromDocxToTxT = $"-o file.txt -f docx -t markdown -V lang=russian {dialog.FileName}";
                    Process convDocx = Process.Start(Conv_Parametrs(FromDocxToTxT));
                    convDocx.WaitForExit(3000);
                    dialog.FileName = "file.txt";
                    Converted = true;
                    break;
                case ".rtf":
                    //string FromRtfToTxT = $"-o file.txt -f rtf -t markdown {dialog.FileName}";
                    //Process convRtf = Process.Start(Conv_Parametrs(FromRtfToTxT));
                    //convRtf.WaitForExit(3000);
                    //dialog.FileName = "C:\\Users\\Soft\\Desktop\\WindowsFormsApp3\\WindowsFormsApp3\\bin\\Debug\\file.txt";
                   // Converted = true;

                    break;
                case ".epub":
                    string FromEpubToTxT = $"{dialog.FileName} -t plain -o file.txt";
                    Process convEpub = Process.Start(Conv_Parametrs(FromEpubToTxT));
                    convEpub.WaitForExit(3000);
                    dialog.FileName = "file.txt";
                    Converted = true;
                    break;
            }
            if (!(res == DialogResult.Cancel))
            {
                parametrs();
                str = File.ReadAllText(dialog.FileName);
                strUtf8 = File.ReadAllText(dialog.FileName, Encoding.UTF8);
                strANSI = File.ReadAllText(dialog.FileName, Encoding.Default);
                if (Converted)
                    File.Delete("file.txt");
                SliceOnPages(str);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (current+1 < pages.Count)
            {
                current++;
                rich.Text = pages[current];
                label1.Text = $"{current+1}/{(pages.Count)}";
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (current+1 > 1)
            {
                current--;
                rich.Text = pages[current];
                label1.Text = $"{current+1}/{(pages.Count)}";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string cur = current.ToString();
            string max = pages.Count.ToString();
            if (!textBox1.Text.All(Char.IsDigit))
            {
                textBox1.Text = new string(textBox1.Text.Where(c => Char.IsDigit(c)).ToArray());
                if (textBox1.Text == "")
                    {

                    }
                    else if (Convert.ToInt64(textBox1.Text) > Int32.MaxValue)
                    {
                        textBox1.Text = 1.ToString();
                    }
                    else
                    {
                        if (Convert.ToInt64(textBox1.Text) <= pages.Count)
                        {
                            current = Convert.ToInt32(textBox1.Text)-1;
                            label1.Text = $"{current+1}/{(pages.Count)}";
                            rich.Text = pages[current];
                        }
                    }
                }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MinimumSize = new Size((int)(System.Windows.SystemParameters.PrimaryScreenHeight / 1.5), (int)(System.Windows.SystemParameters.PrimaryScreenHeight - 60));
            MaximumSize = new Size((int)(System.Windows.SystemParameters.PrimaryScreenHeight / 1.5), (int)(System.Windows.SystemParameters.PrimaryScreenHeight - 60));
            Size = new Size((int)(System.Windows.SystemParameters.PrimaryScreenHeight / 1.5), (int)(System.Windows.SystemParameters.PrimaryScreenHeight - 60));
        }

        private void uTF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rich = new RichTextBox();
            pages.Clear();
            parametrs();
            SliceOnPages(strUtf8);
        }

        private void aNSIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rich = new RichTextBox();
            pages.Clear();
            parametrs();
            SliceOnPages(strANSI);
        }
    }
}
