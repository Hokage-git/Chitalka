using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reader
{
    public partial class Reader: UserControl
    {
        public Reader()
        {
            InitializeComponent();
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Enabled = false;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
