using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RadioStart.WheatherGadgetProcess;
using System.Xml.Serialization;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace RadioStart.WheatherGadgetConfigurator
{
    public partial class Form1 : Form
    {

        public delegate void _();
        WheatherGadgetData gadgetData = new WheatherGadgetData();
        GadgetLayerData currentLayer = new GadgetLayerData();

        public Form1()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;
            const int WM_KEYUP = 0x0101;
            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                if((keyData & Keys.Control) == Keys.Control)
                    ctrlPressed = true;
            }

            if (msg.Msg == WM_KEYUP) 
            {
                ctrlPressed = false;
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }

        void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!ctrlPressed)
            {
                return;
            }

            decimal incr = 0;
            if (e.Delta > 0)
            {
                incr = (decimal)((float)numericUpDownZoom.Value + 0.1);

            }
            else
                incr = (decimal)((float)numericUpDownZoom.Value - 0.1);
            if (incr > numericUpDownZoom.Maximum || incr < numericUpDownZoom.Minimum)
                return;

            numericUpDownZoom.Value = incr;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            SaveVisualization();
            currentLayer = new GadgetLayerData();
            currentLayer.Name = "Layer " + listBoxLayers.Items.Count;
            currentLayer.Position = listBoxLayers.Items.Count;
            currentLayer.Type = LayerType.Picture;
            gadgetData.Layers.Add(currentLayer);
            UpdateAll();
            listBoxLayers.SelectedIndex = listBoxLayers.Items.Count - 1;
        }

        void SaveVisualization()
        {
            currentLayer.Name = textBoxLayerName.Text;
            currentLayer.X = (int)numericUpDownX.Value;
            currentLayer.Y = (int)numericUpDownY.Value;
            currentLayer.W = (int)numericUpDownW.Value;
            currentLayer.H = (int)numericUpDownH.Value;
        }
        private void оПрогToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 f = new AboutBox1();
            f.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBoxRequestResult.BeginInvoke(new _(()=> {textBoxRequestResult.Text = 
            WheatherGadgetProcess.GadgetWheatherProcessor.PerformRequest(textBoxRequest.Text);}));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = saveFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            comboBoxRuleParameters.Items.Clear();
            foreach(var par in gadgetData.Parameters)
                comboBoxRuleParameters.Items.Add(par);
            
            comboBoxRuleSign.Items.Clear();
            foreach (var par in Enum.GetNames(typeof(Operation)))
                comboBoxRuleSign.Items.Add(par);
        }

        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gadgetData = new WheatherGadgetData();
            cur_file = null;
            UpdateAll();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Image Files(*.BMP;*.JPG;*.png)|*.BMP;*.JPG;*.PNG";
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
            {
                textBoxPicture.Text = of.FileName;
                Image img = Image.FromFile(textBoxPicture.Text);
                numericUpDownW.Value = img.Width;
                numericUpDownH.Value = img.Height;
                textBoxPicture.Text = of.FileName;
                currentLayer.Sample = textBoxPicture.Text.Replace(AppDomain.CurrentDomain.BaseDirectory, string.Empty);
            }
        }

        private static Image ResizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        private void DrawPreview()
        {
           
            using (Bitmap b = new Bitmap((int)numericUpDown1.Value, (int)numericUpDown2.Value ))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    g.DrawRectangle(new Pen(new SolidBrush(Color.Red)),new Rectangle(0,0,(int)numericUpDown1.Value, (int)numericUpDown2.Value));
                    foreach (var layer in gadgetData.Layers)
                    {
                        switch (layer.Type)
                        {
                            case LayerType.Text:
                                
                                g.DrawString(layer.Name, layer.Font.Font,new SolidBrush(Color.FromArgb(layer.Color)), new PointF(layer.X, layer.Y));
                                break;
                            case LayerType.Picture:
                                if (layer.W <= 0 || layer.H <= 0)
                                    continue;
                                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"test.png");
                                if (layer.Sample != null)
                                  path = layer.Sample.Contains(":") ? layer.Sample : Path.Combine(AppDomain.CurrentDomain.BaseDirectory,layer.Sample);
                                using (Image j = Image.FromFile(File.Exists(path) ? path : "test.png", true))
                                {
                                    Image im = ResizeImage(j, new Size(layer.W, layer.H));
                                    g.DrawImage(j, new Rectangle(layer.X, layer.Y, layer.W, layer.H));
                                }
                                break;
                        }
                    }
                    Image imm = ResizeImage(b, new Size((int)(numericUpDown1.Value * numericUpDownZoom.Value), (int)(numericUpDown2.Value * numericUpDownZoom.Value)));
                    MemoryStream ms = new MemoryStream();
                    imm.Save(ms, ImageFormat.Png);
                    pictureBoxView.Image = Image.FromStream(ms);
                }
            }
            
        }

        private int comprassion(GadgetLayerData d1, GadgetLayerData d2) 
        {
            return d1.Position - d2.Position;
        }
        #region tabpage1

        //private void ClearLayerPage() 
        //{
        //    textBoxLayerX.Text = textBoxLayerY.Text = textBoxLayerW.Text = textBoxLayerH.Text = "0";
        //    listBoxLayers.Items.Clear();
        //    textBoxPicture.Text = string.Empty;
        //    checkBox1.Checked = false;
        //    textBoxLayerName.Enabled = true;
        //}
        
        private void UpdateLayerInfo()
        {
            controlUpdate = true;
            numericUpDownX.Value = currentLayer.X;
            numericUpDownY.Value = currentLayer.Y;
            numericUpDownW.Value = currentLayer.W;
            numericUpDownH.Value = currentLayer.H;
            checkBoxNullable.Checked = currentLayer.IsNullable;
            textBoxPicture.Text = currentLayer.Sample;
            checkBox1.Checked = currentLayer.Type == LayerType.Text;
            textBoxLayerName.Enabled = !(currentLayer.Type == LayerType.Text);
            textBoxLayerName.Text = currentLayer.Name;
            controlUpdate = false;
        }

       

        bool controlUpdate = false;
        #endregion

        #region ControlUpdates

        void UpdateAll()
        {
            
            UpdateVisualization(); UpdateConnection(); UpdateRules();
            
        }

        void UpdateVisualization() 
        {
            controlUpdate = true;
            gadgetData.Layers.Sort(new Comparison<GadgetLayerData>(comprassion));
            listBoxLayers.Items.Clear();
            
            foreach (GadgetLayerData layer in gadgetData.Layers)
            {
                listBoxLayers.Items.Add(layer);
            }
            listBoxLayers.DisplayMember = "ListName";
            groupBoxLayerEdit.Enabled = false;
            controlUpdate = false;
        }

        void UpdateConnection() 
        {
            textBoxRequest.Text = gadgetData.ConnectionString;
            textBoxRequestSubForm.Text = gadgetData.SubForm;
            UpdateTagList(); 
        }

        bool ruleListUpdated = false;
        void UpdateRules()
        {
            ruleListUpdated = true;
            listBoxRules.Items.Clear();
            
            foreach (GadgetRuleData data in gadgetData.Rules)
            {
                listBoxRules.Items.Add(data);
            }
            groupBoxRule.Enabled = false;
            listBoxRules.DisplayMember = "ListName";
            ruleListUpdated = false;
        }

        #endregion
        
        private void button9_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (fontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
            {
                currentLayer.Font = new SerializableFont(fontDialog1.Font);
            }
            timer1.Enabled = true;
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            

            if (checkBox1.Checked)
            {
                buttonFont.Visible = buttonColor.Visible = true;
                buttonPicture.Enabled = false;
                textBoxPicture.Enabled = false;
                if (!controlUpdate)
                {
                    ParameterChooseForm cf = new ParameterChooseForm();
                    cf.ShowDialog();
                    textBoxLayerName.Text = cf.Tag.ToString();
                    textBoxLayerName.Enabled = false;
                    currentLayer.Name = textBoxLayerName.Text;
                    currentLayer.Type = LayerType.Text;
                }
            }
            else
            {
                buttonFont.Visible = buttonColor.Visible = false;
                buttonPicture.Enabled = true;
                textBoxLayerName.Enabled = true;
               
                textBoxPicture.Enabled = true;
                if (!controlUpdate)
                {
                    currentLayer.Name = textBoxLayerName.Text;
                    currentLayer.Type = LayerType.Picture;
                }
            }


        }


        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
            {
                gadgetData = 
                ConfigSerializer.Deserialize<WheatherGadgetData>(openFileDialog1.FileName);
                cur_file = openFileDialog1.FileName;
                UpdateAll();
            }
        }

        string cur_file = null;

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ConfigSerializer.Serialize<WheatherGadgetData>(gadgetData, saveFileDialog1.FileName);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(gadgetData != null){
            if(MessageBox.Show("Сохранить изменения?","Внимание",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                if(cur_file == null)
                    сохранитьToolStripMenuItem_Click(sender,e);
                else
                    ConfigSerializer.Serialize<WheatherGadgetData>(gadgetData, cur_file);
                }
            }
            Application.Exit();
        }

        private void listBoxLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxLayers.SelectedIndex < 0)
            {
                groupBoxLayerEdit.Enabled = false;
                return;
            }
            groupBoxLayerEdit.Enabled = true;
            currentLayer = listBoxLayers.SelectedItem as GadgetLayerData;
            UpdateLayerInfo();
        }

       

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            if (listBoxLayers.SelectedIndex < 0)
                return;
            gadgetData.Layers.Remove(listBoxLayers.SelectedItem as GadgetLayerData);
            int idx = listBoxLayers.SelectedIndex;
            UpdateVisualization();
            if (idx < listBoxLayers.Items.Count)
                listBoxLayers.SelectedIndex = idx;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listBoxLayers.SelectedIndex < 1)
                return;
            (listBoxLayers.SelectedItem as GadgetLayerData).Position--;
            (listBoxLayers.Items[listBoxLayers.SelectedIndex - 1] as GadgetLayerData).Position++;
            int idx = listBoxLayers.SelectedIndex -1;
            
            UpdateVisualization();
            listBoxLayers.SelectedIndex = idx;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listBoxLayers.SelectedIndex < 0 || listBoxLayers.SelectedIndex >= listBoxLayers.Items.Count -1)
                return;
            (listBoxLayers.SelectedItem as GadgetLayerData).Position++;
            (listBoxLayers.Items[listBoxLayers.SelectedIndex + 1] as GadgetLayerData).Position--;
            int idx = listBoxLayers.SelectedIndex + 1;
            UpdateVisualization();
            listBoxLayers.SelectedIndex = idx;
        }

        #region tabpage 2
        bool tagListUpdated = false;
        private void UpdateTagList() 
        {
            tagListUpdated = true;
            listBoxParameters.Items.Clear();
            listBoxParameters.DisplayMember = "VisibleName";
            foreach (GadgetItemTagData data in gadgetData.Tags) 
            {
                listBoxParameters.Items.Add(data);
            }
            tagListUpdated = false;
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
           
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            TagTypeForm pf = new TagTypeForm(gadgetData.Parameters);
            pf.ShowDialog();
            GadgetItemTagData data = new GadgetItemTagData();
            data.Parameter = pf.Tag;
            data.TagName = pf.ResultValue;
            gadgetData.Tags.Add(data);
            UpdateTagList();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (listBoxParameters.SelectedIndex < 0 || tagListUpdated)
                return;
            GadgetItemTagData data = listBoxParameters.SelectedItem as GadgetItemTagData;
            TagTypeForm pf = new TagTypeForm(gadgetData.Parameters);
            pf.Tag = data.Parameter;
            pf.ResultValue = data.TagName;
            pf.ShowDialog();
            data.Parameter = pf.Tag;
            data.TagName = pf.ResultValue;
            UpdateTagList();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (listBoxParameters.SelectedIndex < 0 || tagListUpdated)
                return;
            GadgetItemTagData data = listBoxParameters.SelectedItem as GadgetItemTagData;
            gadgetData.Tags.Remove(data);
            UpdateTagList();
        }
        #endregion

        #region tabpage 3

       

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GadgetRuleData rule = new GadgetRuleData();
            rule.Name = "Rule " + listBoxRules.Items.Count;
            gadgetData.Rules.Add(rule);
            UpdateRules();
            listBoxRules.SelectedItem = rule;
        }


        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            DrawPreview();
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            ColorDialog cd = new ColorDialog();
            
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentLayer.Color = cd.Color.ToArgb();
            }
            timer1.Enabled = true ;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawPreview();
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 0)
                timer1.Enabled = true;
            else
                timer1.Enabled = false;

            if (tabControl2.TabIndex == 3) 
            {
                comboBoxRuleLayer.Items.Clear();
                foreach (var par in gadgetData.Layers)
                    comboBoxRuleLayer.Items.Add(par.Name);
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            toolStripButton5_Click(sender, e);
        }

      
        private void numericUpDownW_ValueChanged(object sender, EventArgs e)
        {
            if (controlUpdate)
                return;
            currentLayer.W = (int)numericUpDownW.Value;
        }

        private void numericUpDownH_ValueChanged(object sender, EventArgs e)
        {
            if (controlUpdate)
                return;
            currentLayer.H = (int)numericUpDownH.Value;
        }

        private void numericUpDownX_ValueChanged(object sender, EventArgs e)
        {
            if (controlUpdate)
                return;
            currentLayer.X = (int)numericUpDownX.Value;
        }

        private void numericUpDownY_ValueChanged(object sender, EventArgs e)
        {
            if (controlUpdate)
                return;
            currentLayer.Y = (int)numericUpDownY.Value;
        }

        private void textBoxLayerName_TextChanged(object sender, EventArgs e)
        {
            if (controlUpdate)
                return;
            currentLayer.Name = textBoxLayerName.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.Cross;
        }

        private void pictureBoxView_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.Default;
          
        }

        private void pictureBoxView_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Cursor != System.Windows.Forms.Cursors.Cross)
                return;
            int canvasW = (int) numericUpDown1.Value;
            int canvasH = (int) numericUpDown2.Value;

            int startX = pictureBoxView.Width / 2 - (int)(canvasW * numericUpDownZoom.Value) / 2;
            int startY = pictureBoxView.Height / 2 - (int)(canvasH * numericUpDownZoom.Value) / 2;
            if (e.X < startX + numericUpDownW.Value/2 || e.Y < startY + numericUpDownW.Value/2)
                return;
            numericUpDownX.Value = e.X - startX - numericUpDownW.Value / 2;
            numericUpDownY.Value = e.Y - startY - numericUpDownH.Value / 2;
        }

        GadgetRuleData selectedRule = null;
        private void listBoxRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ruleListUpdated) 
            {
                return;
            }
            if (listBoxRules.SelectedIndex < 0)
            {
                groupBoxRule.Enabled = false;
                return;
            }
            groupBoxRule.Enabled = true;
            selectedRule = listBoxRules.SelectedItem as GadgetRuleData;
            textBoxRuleName.Text = selectedRule.Name;
            comboBoxRuleSign.SelectedItem = selectedRule.Operation.ToString();
            comboBoxRuleLayer.SelectedItem = selectedRule.Layer;
            comboBoxRuleParameters.SelectedItem = selectedRule.Parameter.ToString();
            textBoxRuleImage.Text = selectedRule.FileName;
            textBoxRuleValue.Text = selectedRule.RegexValue;
        }

        private void textBoxRuleName_TextChanged(object sender, EventArgs e)
        {
            if (ruleListUpdated)
                return;
            selectedRule.Name = textBoxRuleName.Text;
        }

        private void comboBoxRuleParameters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ruleListUpdated || comboBoxRuleParameters.SelectedIndex < 0)
                return;
            selectedRule.Parameter = comboBoxRuleParameters.SelectedItem.ToString();
        }

        private void comboBoxRuleSign_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ruleListUpdated || comboBoxRuleSign.SelectedIndex < 0)
                return;
            selectedRule.Operation = (Operation)Enum.Parse(typeof(Operation), comboBoxRuleSign.SelectedItem.ToString());
        }

        private void textBoxRuleValue_TextChanged(object sender, EventArgs e)
        {
            if (ruleListUpdated)
                return;
            selectedRule.RegexValue = textBoxRuleValue.Text;
        }

        private void comboBoxRuleLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ruleListUpdated || comboBoxRuleLayer.SelectedIndex < 0)
                return;
            selectedRule.Layer = comboBoxRuleLayer.SelectedItem.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Image Files(*.BMP;*.JPG;*.png)|*.BMP;*.JPG;*.PNG";
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxRuleImage.Text = GetCorrectFileName(of.FileName);
            }

        }

        private static string GetCorrectFileName(string fullname)
        {
            string res = fullname.Replace(AppDomain.CurrentDomain.BaseDirectory, string.Empty);
            if (res.Contains(":"))
                return new FileInfo(fullname).Name;
            else return res;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            gadgetData.Rules.Remove(selectedRule);
            UpdateRules();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gadgetData != null && cur_file != null && !AppError.IsError)
            {
                if (MessageBox.Show("Сохранить изменения?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    ConfigSerializer.Serialize<WheatherGadgetData>(gadgetData, cur_file);
                }
            }
        }

        bool ctrlPressed = false;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Control) 
            {
                ctrlPressed = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            ctrlPressed = false;
        }

        private void pictureBoxView_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void pictureBoxView_Click_1(object sender, EventArgs e)
        {
            pictureBoxView_Click(sender, e);
        }

        private void checkBoxNullable_CheckedChanged(object sender, EventArgs e)
        {
            if (controlUpdate)
                return;
            currentLayer.IsNullable = checkBoxNullable.Checked;
        }

        private void pictureBoxView_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            currentLayer.W = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            currentLayer.H = (int)numericUpDown2.Value;
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            string[] strings = textBoxTesting.Text.Split(';');
            listBox1.Items.Clear();
            foreach (string s in strings) 
            {
                GadgetWheatherProcessor wp = new GadgetWheatherProcessor(this.gadgetData);
                wp.OnError = WriteException;
                wp.ConnectionString = s;
                wp.Process(null);
                GadgetTestViewForm tf = new GadgetTestViewForm(s, wp.LastImage,this.gadgetData.Tags, this.gadgetData.Rules);
                tf.Show();
            }
        }
        private void WriteException(Exception e) 
        {
            listBox1.Items.Add(String.Format("{0};{1}",e.Message,e.StackTrace));
        }
        private void TestingThread(object data) 
        {
        
        }

        private void textBoxRequestSubForm_TextChanged(object sender, EventArgs e)
        {
            gadgetData.SubForm = textBoxRequestSubForm.Text;
        }

        private void textBoxRuleImage_TextChanged(object sender, EventArgs e)
        {
            if (ruleListUpdated)
                return;
            selectedRule.FileName = textBoxRuleImage.Text; 
        }

        private void numericUpDownZoom_ValueChanged(object sender, EventArgs e)
        {

        }

    }
}
