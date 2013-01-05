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
    public partial class GadgetTestViewForm : Form
    {
      
        public GadgetTestViewForm(string Text, Image image, List<GadgetItemTagData> tags, List<GadgetRuleData> rules)
        {
            InitializeComponent();
            this.Text = Text;
            pictureBoxView.Image = image;

            listBox1.Items.Clear();
            foreach (GadgetItemTagData tag in tags) 
            {
                listBox1.Items.Add(String.Format("{0} = {1}",tag.Parameter,tag.Value));
            }

            listBox2.Items.Clear();
            foreach (GadgetRuleData rule in rules) 
            {
                listBox2.Items.Add(String.Format("{0} - {1}",rule.Name, rule.Correct ? "Корректно" : "Некорректно"));
            }
        }

        private void GadgetTestViewForm_Load(object sender, EventArgs e)
        {

        }
    }
}
