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
    public partial class ChangePassword : Form
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        string[] Member3 = new string[3];
        public ChangePassword(string[] Member2)
        {
            InitializeComponent();
            Member3 = Member2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "Data Source=MJFT;Initial Catalog=Test1;Integrated Security=True";
            conn.Open();
            string opt = "Update Members SET Spassword = '" + textBox1.Text + "' WHERE Sno = '" + Member3[0] + "'";
            SqlCommand cmd = new SqlCommand(opt, conn);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            using (SqlCommandBuilder builder = new SqlCommandBuilder(adapter))
            {
                adapter.Update(dt);
            }
            conn.Close();

            MessageBox.Show("修改成功", "提示");

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
