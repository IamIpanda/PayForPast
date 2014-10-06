using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace 为过去赎罪
{
    public partial class Form1 : Form
    {
        DirectoryInfo dir;
        List<FileInfo> files = new List<FileInfo>();
        List<String> patterns = new List<String> { ".jpg", ".jpeg", ".bmp", ".png" };
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;
            dir = new DirectoryInfo(textBox1.Text);
            button1.Enabled = dir.Exists;
            listBox1.Items.Clear();
            if (dir.Exists)
            {
                files.Clear();
                foreach (var file in dir.GetFiles())
                    if (patterns.Contains(file.Extension))
                    {
                        listBox1.Items.Add(file.Name);
                        files.Add(file);
                    }
            }
            else listBox1.Items.Add("路径不合法。");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = 
            label2.Enabled = checkBox1.Checked;
        }
        
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            if (button1.Enabled == false) return;
            FileInfo file = files[listBox1.SelectedIndex];
            try
            {
                Bitmap bit = new Bitmap(file.FullName);
                pictureBox1.Image = bit;
            }
            catch (Exception ex) { }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null) return;
            Image bit = pictureBox1.Image;
            double zoom, zoom_x, zoom_y, location_x, location_y;
            zoom_x = bit.Width / (pictureBox1.Width + 0.0);
            zoom_y = bit.Height / (pictureBox1.Height + 0.0);
            if (zoom_x > zoom_y)
            {
                zoom = zoom_x;
                location_x = 0;
                location_y = (pictureBox1.Height - bit.Height / zoom_x) / 2;
            }
            else
            {
                zoom = zoom_y;
                location_y = 0;
                location_x = (pictureBox1.Width - bit.Width / zoom_y) / 2;
            }
            location_x = (e.X - location_x) * zoom;
            location_y = (e.Y - location_y) * zoom;
            Color color = ((Bitmap)bit).GetPixel((int)location_x, (int)location_y);
            label2.BackColor = color;
            label1.Text = color.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            new System.Threading.Thread(this.Work).Start();
        }
        public void Work()
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            Bitmap bit, clone;
            for (int i = 0; i < files.Count; i++)
            {
                FileInfo file = files[i];
                this.listBox1.SelectedIndex = i;
                bit = null;
                try
                {
                    bit = new Bitmap(file.FullName);
                }
                catch (Exception ex) { }
                if (bit == null) continue;
                Color back;
                if (checkBox1.Checked) back = label2.BackColor;
                else
                {
                    back = bit.GetPixel(0, 0);
                    label2.BackColor = back;
                    label1.Text = back.ToString();
                }
                clone = bit.Clone() as Bitmap;
                bit.Dispose();
                clone.MakeTransparent(back);
                Bitmap show = new Bitmap(clone.Width, clone.Height);
                Graphics gra = Graphics.FromImage(show);
                gra.DrawImage(clone, 0, 0);
                gra.Save();
                pictureBox1.Image = show;
                try
                {
                    clone.Save(file.FullName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            MessageBox.Show("完成了您的操作");
            Application.Exit();
        }

        private void Invoke(Action<int> action, int i)
        {
            action.Invoke(i);
        }
        public void setIndex(int i)
        {
            this.listBox1.SelectedIndex = i;
        }

    }
}
