using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;

namespace RadioStart.WheatherGadgetProcess
{
    public delegate void OnError(Exception e);
    public class GadgetWheatherProcessor
    {
        public OnError OnError;
        WheatherGadgetData data;
        Thread workThread = null;
        Image image;
        public Image LastImage { get { return image; } }
        public string ConnectionString;

        static object picture_locker = new object();
        private void WriteError(Exception e) 
        {
            if (OnError != null)
                OnError(e);
        }
        public GadgetWheatherProcessor(WheatherGadgetData data) 
        {
            this.data = data;
            ConnectionString = data.ConnectionString;
        }

        public string MakeRequest() 
        {
            return PerformRequest(ConnectionString);
        }

        public void StartProcess() 
        {
            if (workThread != null) 
            {
                StopProcess();
            };
            workThread = new Thread(new ParameterizedThreadStart(ProcessThread));
            workThread.Start(null);
        }

        public void StopProcess() 
        {
            workThread.Abort();
            workThread.Join();
            workThread = null;
        }

        public void ProcessThread(object data) 
        {
            while (true) 
            {
                Process(data);
                Thread.Sleep(this.data.Timeout);
            }
        }
        public void Process(object data) 
        {
            try
            {
                string request = MakeRequest();
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(request);
                XmlNode form = xd.GetElementsByTagName(this.data.SubForm)[0];
                foreach (XmlNode node in form.ChildNodes) 
                {
                    foreach(GadgetItemTagData tag in this.data.Tags)
                    {
                        if (string.Compare(tag.TagName, node.Name, true) == 0) 
                        {
                            tag.Value = node.Attributes["data"].Value;
                            break;
                        }
                    }
                }

                ProcessRules();

            }
            catch (Exception e) 
            {
                WriteError(e);
                Logger.WriteEntry(String.Format("{0} ; {1}",e.Message,e.StackTrace));
            }
        }

        private void ProcessRules()
        {
            foreach (GadgetRuleData rule in data.Rules)
            {
                if (ProcessRule(rule))
                    rule.Correct = true;
                else
                    rule.Correct = false;
            }
            DrawImage();
        }

        private void DrawImage()
        {

            using (Bitmap b = new Bitmap(this.data.W, this.data.H))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    foreach (GadgetLayerData layer in this.data.Layers)
                    {
                        GadgetRuleData founded = data.Rules.FindLast(x => x.Layer == layer.Name && x.Correct == true);
                        if (founded == null)
                        {
                            if (layer.IsNullable)
                                continue;
                            layer.Value = layer.Type == LayerType.Picture ? layer.Sample : data.Tags.Find(x => x.Parameter.ToString() == layer.Name).Value;
                            DrawLayer(g, layer);
                        }
                        else
                        {
                            layer.Value = founded.FileName;
                            DrawLayer(g, layer);
                        }
                    }
                }

                MemoryStream ms = new MemoryStream();
                b.Save(ms, ImageFormat.Png);
                
                image = Image.FromStream(ms);
                lock (picture_locker)
                {
                    image.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"wheather.png"));
                }
            }
        }

        public static byte[] GetWheatherImageStream(Size size) 
        {
            byte[] res = null;
            lock(picture_locker)
            {
                 Image im = ResizeImage(Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wheather.png"), true), size);
                 MemoryStream ms = new MemoryStream();
                 im.Save(ms, ImageFormat.Png);
                 res = ms.ToArray();
            }
            return res;
        }

        public static byte[] GetWheatherImageStream()
        {
            byte[] res = null;
            lock (picture_locker)
            {
                res = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wheather.png"));
            }
            return res;
        }

        private int comprassion(GadgetLayerData d1, GadgetLayerData d2)
        {
            return d1.Position - d2.Position;
        }

        private bool ProcessRule(GadgetRuleData rule)
        {

            string value = this.data.Tags.Find(x => x.Parameter == rule.Parameter).Value;
            if (value == null)
                return false;
            int int_value = 0;
            int i_res = 0;
            bool is_int = int.TryParse(rule.RegexValue, out int_value);
            Match mc = Regex.Match(value,@"\d+");
            switch (rule.Operation)
            {

                case Operation.Equal:
                     if
                       (is_int && int.TryParse(value, out i_res))
                    {
                        return i_res == int_value;
                    }else if(mc.Success)
                     { return int.Parse(mc.Value) == int_value;
                    }
                    else return String.Compare(rule.RegexValue, value, true) == 0;

                case Operation.Great:
                     if (is_int && int.TryParse(value, out i_res))
                         return i_res > int_value;else
                    if (mc.Success)
                        return int.Parse(mc.Value) > int_value;
                    break;
                case Operation.GreatEqual:
                    if (is_int && int.TryParse(value, out i_res))
                        return i_res >= int_value;
                    else
                        if (mc.Success)
                            return int.Parse(mc.Value) >= int_value;
                    break;
                case Operation.Less:
                    if (is_int && int.TryParse(value, out i_res))
                        return i_res < int_value;
                    else
                        if (mc.Success)
                            return int.Parse(mc.Value) < int_value;
                    break;
                case Operation.LessEqual:
                    if (is_int && int.TryParse(value, out i_res))
                        return i_res <= int_value;
                    else
                        if (mc.Success)
                            return int.Parse(mc.Value) <= int_value;
                    break;
            }
            return false;
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

        //private void DrawPreview()
        //{

        //    using (Bitmap b = new Bitmap(this.data.W, this.data.H))
        //    {
        //        using (Graphics g = Graphics.FromImage(b))
        //        {
        //            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //            g.SmoothingMode = SmoothingMode.AntiAlias;
        //            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //            g.CompositingQuality = CompositingQuality.HighQuality;
        //            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        //             foreach (var layer in data.Layers)
        //            {
        //                DrawLayer(g, layer);
        //                return;
        //            }
        //            Image imm = ResizeImage(b, new Size((int)(numericUpDown1.Value * numericUpDownZoom.Value), (int)(numericUpDown2.Value * numericUpDownZoom.Value)));
        //            MemoryStream ms = new MemoryStream();
        //            imm.Save(ms, ImageFormat.Png);
        //            pictureBoxView.Image = Image.FromStream(ms);
        //        }
        //    }

        //}

        private static void DrawLayer(Graphics g, GadgetLayerData layer)
        {
            switch (layer.Type)
            {
                case LayerType.Text:
                    g.DrawString(layer.Value, layer.Font.Font, new SolidBrush(Color.FromArgb(layer.Color)), new PointF(layer.X, layer.Y));
                    break;
                case LayerType.Picture:
                    if (layer.W <= 0 || layer.H <= 0)
                        return;
                    
                    if (layer.Value == null)
                            return;
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,layer.Value);
                    if (!File.Exists(path))
                        return;
                    using (Image j = Image.FromFile(path, true))
                    {
                        Image im = ResizeImage(j, new Size(layer.W, layer.H));
                        g.DrawImage(j, new Rectangle(layer.X, layer.Y, layer.W, layer.H));
                    }
                    break;
            }
            return;
        }

        public static string PerformRequest(string url)
        {
            //Initialization
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url);

            WebReq.AllowAutoRedirect = false;

            //This time, our method is GET.
            //WebReq.CookieContainer = new CookieContainer();
            WebReq.Method = "GET";
            //From here on, it's all the same as above.
            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
            //Now, we read the response (the string), and output it.
            Stream Answer = WebResp.GetResponseStream();
            StreamReader _Answer = new StreamReader(Answer);

            return _Answer.ReadToEnd();
        }

    }
}
