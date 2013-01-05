using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RadioStart.WheatherGadgetProcess;

namespace RadioStart.WheatherGadgetConfigurator
{
    public partial class TagTypeForm : Form
    {
        private string tag = GadgetItem.Temperature.ToString();
        public new string Tag { get { return tag; } set { tag = value; } }
        public string ResultValue { get; set; }
        List<string> parameters;
        public TagTypeForm(List<string> parameters)
        {
            InitializeComponent();
            this.parameters = parameters;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                ResultValue = textBox1.Text;
                Tag = comboBox1.Text;
                if (!parameters.Contains(comboBox1.Text))
                    parameters.Add(comboBox1.Text);
                DialogResult = System.Windows.Forms.DialogResult.OK;
                return;
            }
        }

        private void NewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ResultValue = textBox1.Text;
            Tag = comboBox1.Text;
            if (!parameters.Contains(comboBox1.Text))
                parameters.Add(comboBox1.Text);
        }

        private void TagTypeForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (string data in parameters)
                comboBox1.Items.Add(data);
            comboBox1.SelectedIndex = 0;
            textBox1.Text = ResultValue;
            comboBox1.Text = tag.ToString();
        }
    }
}
