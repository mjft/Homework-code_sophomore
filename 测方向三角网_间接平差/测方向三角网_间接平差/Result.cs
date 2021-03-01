using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 测方向三角网_间接平差
{
    public partial class Result : Form
    {
        public Result()
        {
            InitializeComponent();
        }
        string ResultText;
        public Result(string Text)
        {
            InitializeComponent();
            ResultText = Text;
        }//从父窗传递结果

        private void Result_Load(object sender, EventArgs e)
        {
            richTextBox1_Result.Text = ResultText;
        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string SavePath;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "文本文件 (*.txt)|*.txt";
            saveFileDialog1.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)//点了保存按钮进入 
            {
                //获得文件路径 
                SavePath = saveFileDialog1.FileName.ToString();
                File.CreateTxtWithText(SavePath, richTextBox1_Result.Text);
            }
        }

        private void 帮助HToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
