using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RadioStart.WheatherGadgetConfigurator
{
    public partial class NewForm : Form
    {
        public string ResultData { get; set; }
        public NewForm()
        {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                ResultData = textBox1.Text;
                DialogResult = System.Windows.Forms.DialogResult.OK;
                return;
            }
        }

        private void NewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ResultData = textBox1.Text;
        }

        private void NewForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = ResultData;
        }
    }
}
