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

namespace 数据库实验3._5
{
    public partial class ChangeLevel : Form
    {
        public ChangeLevel()
        {
            InitializeComponent();
        }

        string[] Member4 = new string[3];
        public ChangeLevel(string[] Member2)
        {
            InitializeComponent();
            Member4 = Member2;
        }

        private void ChangeLevel_Load(object sender, EventArgs e)
        {
            label1.Text = "当前账号的权限：" + Member4[2];
        }

        private void button2_Click(object sender, EventArgs e)//升级
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "Data Source=MJFT;Initial Catalog=Test1;Integrated Security=True";
            conn.Open();
            string opt = "Update Members SET Slevel = Slevel + 1 WHERE Sno = '" + Member4[0] + "'";
            SqlCommand cmd = new SqlCommand(opt, conn);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            using (SqlCommandBuilder builder = new SqlCommandBuilder(adapter))
            {
                adapter.Update(dt);
            }

            MessageBox.Show("修改成功", "提示");

            string opt2 = "select * from Members where Sno = '" + Member4[0] + "'";
            SqlCommand cmd2 = new SqlCommand(opt2, conn);
            //cmd2.CommandType = CommandType.Text;
            SqlDataReader sdr;
            sdr = cmd2.ExecuteReader();
            sdr.Read();
            label1.Text = "当前账号的权限：" + sdr.GetInt32(2);

            conn.Close();
        }

        private void button3_Click(object sender, EventArgs e)//降级
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "Data Source=MJFT;Initial Catalog=Test1;Integrated Security=True";
            conn.Open();
            string opt = "Update Members SET Slevel = Slevel - 1 WHERE Sno = '" + Member4[0] + "'";
            SqlCommand cmd = new SqlCommand(opt, conn);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            using (SqlCommandBuilder builder = new SqlCommandBuilder(adapter))
            {
                adapter.Update(dt);
            }

            MessageBox.Show("修改成功", "提示");

            string opt2 = "select * from Members where Sno = '" + Member4[0] + "'";
            SqlCommand cmd2 = new SqlCommand(opt2, conn);
            //cmd2.CommandType = CommandType.Text;
            SqlDataReader sdr;
            sdr = cmd2.ExecuteReader();
            sdr.Read();
            label1.Text = "当前账号的权限：" + sdr.GetInt32(2);

            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
