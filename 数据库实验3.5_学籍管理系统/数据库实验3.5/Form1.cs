using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace 数据库实验3._5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //public static int a;
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.BackColor = Color.Transparent;
            label2.BackColor = Color.Transparent;
            label3.BackColor = Color.Transparent;
        }

        private void button2_Click(object sender, EventArgs e)//登录
        {
            string Sno = textBox1.Text, Password = textBox2.Text;
            string connectionString = "Data Source=MJFT;Initial Catalog=Test1;Integrated Security=True";
            SqlConnection SqlCon = new SqlConnection(connectionString); //数据库连接
            SqlCon.Open(); //打开数据库
            string sql = "Select * from Members where Sno='" + Sno + "'";//查找用户sql语句
            SqlCommand cmd = new SqlCommand(sql, SqlCon);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            SqlDataReader sdr;
            sdr = cmd.ExecuteReader();
            if (Sno.Equals("") || Password.Equals(""))//账号或密码为空
            {
                MessageBox.Show("账号或密码不能为空", "提示");
            }
            else//账号或密码不为空
            {
                sdr.Read();
                if (dt.Rows.Count < 1)
                {
                    MessageBox.Show("账号不存在", "提示");
                }
                else
                {
                    if (Password == sdr.GetString(1).Trim())//密码正确
                    {
                        MessageBox.Show("登陆成功", "提示");
                        string[] Member = new string[3] { sdr.GetString(0).Trim(), sdr.GetString(1).Trim(), sdr.GetInt32(2).ToString().Trim() };
                        Form2 c = new Form2(Member);//关闭登录界面，开启主界面
                        this.Visible = false;
                        c.ShowDialog();//此处不可用Show()
                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("密码错误", "提示");
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)//退出
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
