using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RadioStart.WheatherGadgetConfigurator
{
    public partial class ErrorForm : Form
    {
        
        public string ErrorText { set { textBox1.Text = value; } }
        public ErrorForm()
        {
            InitializeComponent();
        }

        private void ErrorForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.DialogResult = System.Windows.Forms.DialogResult.Yes;
            Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.DialogResult = System.Windows.Forms.DialogResult.No;
            Close();

        }
    }
}
