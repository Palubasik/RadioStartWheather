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
    public partial class ParameterChooseForm : Form
    {
        public ParameterChooseForm()
        {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                Tag = (GadgetItem)Enum.Parse(typeof(GadgetItem), comboBox1.SelectedItem.ToString()); 
                DialogResult = System.Windows.Forms.DialogResult.OK;
                return;
            }
        }

        private void NewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Tag = (GadgetItem)Enum.Parse(typeof(GadgetItem), comboBox1.SelectedItem.ToString()); 
        }

        private void NewForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (string data in Enum.GetNames(typeof(GadgetItem)))
                comboBox1.Items.Add(data);
            comboBox1.SelectedIndex = 0;
        }
    }
}
