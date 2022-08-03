using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _129_Final_Project__TONACAO_
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public static string userInput;
        private void button1_Click(object sender, EventArgs e)
        {
            userInput = textBox1.Text;
            this.Close();
        }
    }
}
