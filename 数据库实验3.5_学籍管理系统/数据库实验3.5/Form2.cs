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
using System.IO;

namespace 数据库实验3._5
{
    public partial class Form2 : Form
    {
        private void Form2_Load(object sender, EventArgs e)
        {
            DO("select * from Student", dataGridView4);
        }

        //从父窗传递用户等级
        string[] Member2 = new string[3];
        public Form2(string[] Member)
        {
            InitializeComponent();
            Member2 = Member;
        }

        private void DO (string opt,DataGridView X)//执行语句
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "Data Source=MJFT;Initial Catalog=Test1;Integrated Security=True";
            conn.Open();
            SqlCommand cmd = new SqlCommand(opt, conn);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            X.DataSource = dt;
            using (SqlCommandBuilder builder = new SqlCommandBuilder(adapter))
            {
                adapter.Update(dt);
            }
            conn.Close();
        }

        private int FindLevel()//获得选中行的用户等级
        {
            int Level;
            int index = dataGridView4.CurrentRow.Index;    //取得选中行的索引  
            string Sno = dataGridView4.Rows[index].Cells["Sno"].Value.ToString().Trim();
            string connectionString = "Data Source=MJFT;Initial Catalog=Test1;Integrated Security=True";
            SqlConnection SqlCon = new SqlConnection(connectionString); //数据库连接
            SqlCon.Open(); //打开数据库
            string sql = "Select * from Members where Sno='" + Sno + "'";//查找用户Sno语句
            SqlCommand cmd = new SqlCommand(sql, SqlCon);
            cmd.CommandType = CommandType.Text;
            SqlDataReader sdr;
            sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {
                Level = sdr.GetInt32(2);
                sdr.Close();
                SqlCon.Close();
                return Level;
            }
            else
            {
                sdr.Close();
                SqlCon.Close();
                return 0;
            }
        }


        /////////////////////////////////////////////////////////以下为主程序//////////////////////////////////////////////////////////////

        private void button2_Click_1(object sender, EventArgs e)//添加记录
        {
            if (textBox_Sno.Text == "")//主键不为空
            {
                MessageBox.Show("请输入学号", "提示");
            }
            else
            {
                SqlConnection conn = new SqlConnection("Data Source=MJFT;Initial Catalog=Test1;Integrated Security=True");
                conn.Open();
                string opt = "select * from Student Where Sno = '" + textBox_Sno.Text + "'";
                SqlCommand cmd = new SqlCommand(opt, conn);
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();
                if (dt != null && dt.Rows.Count > 0)//判断查询结果是否为空，不为空则主键不唯一
                {
                    MessageBox.Show("学号已存在", "提示");
                }
                else
                {
                    //添加学籍信息
                    string cstring = "Data Source=MJFT;Initial Catalog=Test1;Integrated Security=True";
                    string sqlstring = "select * from Student";
                    SqlConnection myconn = new SqlConnection(cstring);
                    myconn.Open();
                    SqlDataAdapter myAdapter;
                    myAdapter = new SqlDataAdapter(sqlstring, myconn);
                    DataSet myDataSet = new DataSet();
                    myAdapter.Fill(myDataSet, "Student");
                    DataRow myrow = myDataSet.Tables["Student"].NewRow();
                    myrow[0] = textBox_Sno.Text;
                    myrow[1] = textBox_Sname.Text;
                    if (textBox_Ssex.Text == "男" || textBox_Ssex.Text == "女" || textBox_Ssex.Text.Trim().Length == 0)
                        myrow[2] = textBox_Ssex.Text;
                    else MessageBox.Show("请输入男或女", "提示");
                    if (textBox_Sbirthday.Text.Trim().Length != 0)
                        myrow[3] = textBox_Sbirthday.Text.ToString();
                    myrow[4] = textBox_Sacademy.Text;
                    myrow[5] = textBox_Sdept.Text;
                    myrow[6] = textBox_Syear.Text;
                    if (textBox_Scet4.Text.Trim().Length != 0)
                        myrow[7] = System.Int32.Parse(textBox_Scet4.Text);//cet4转为int类型
                    myrow[8] = textBox_Sclass.Text;
                    myrow[9] = textBox_Sduty.Text;
                    myrow[10] = textBox_Sorigin.Text;
                    myrow[11] = textBox_Sqq.Text;   
                    myDataSet.Tables["Student"].Rows.Add(myrow);
                    SqlCommandBuilder mycndbuilder = new SqlCommandBuilder(myAdapter);
                    myAdapter.Update(myDataSet, "Student");

                    //添加账号信息
                    string sqlstring2 = "select * from Members";
                    SqlDataAdapter myAdapter2;
                    myAdapter2 = new SqlDataAdapter(sqlstring2, myconn);
                    DataSet myDataSet2 = new DataSet();
                    myAdapter2.Fill(myDataSet2, "Members");
                    DataRow myrow2 = myDataSet2.Tables["Members"].NewRow();
                    myrow2[0] = textBox_Sno.Text;
                    myrow2[1] = "123456";
                    myrow2[2] = "0";
                    myDataSet2.Tables["Members"].Rows.Add(myrow2);
                    SqlCommandBuilder mycndbuilder2 = new SqlCommandBuilder(myAdapter2);
                    myAdapter2.Update(myDataSet2, "Members");

                    myconn.Close();

                    MessageBox.Show("录入成功", "提示");
                }
                DO("select * from Student", dataGridView4);//执行录入
            }
        }

        private void button3_Click_1(object sender, EventArgs e)//删除选中
        {
            int index = dataGridView4.CurrentRow.Index;    //取得选中行的索引  
            if (int.Parse(Member2[2]) > FindLevel())    //权限比较
            {
                DO(    "DELETE FROM Student where Sno = '" + dataGridView4.Rows[index].Cells["Sno"].Value.ToString() + "'" 
                    + " DELETE FROM Members where Sno = '" + dataGridView4.Rows[index].Cells["Sno"].Value.ToString() + "'" 
                    + " select * from Student", dataGridView4);
                MessageBox.Show("删除成功", "提示");
            }
            else MessageBox.Show("权限不足", "提示");
        }

        private void button1_Click_1(object sender, EventArgs e)//一键查询
        {
            //编写查询语句
            string opt = "select * from Student where ";
            if (textBox_Sno.Text == "" && textBox_Sname.Text == "" && textBox_Ssex.Text == "" 
                && textBox_Sbirthday.Text == "" && textBox_Sacademy.Text == "" && textBox_Sdept.Text == ""
                 && textBox_Syear.Text == "" && textBox_Scet4.Text == "" && textBox_Sclass.Text == "" 
                 && textBox_Sduty.Text == "" && textBox_Sorigin.Text == "" && textBox_Sqq.Text == "")
                MessageBox.Show("请输入查询条件","提示");
            else
            {
                if (textBox_Sno.Text != "")
                    opt = opt + "Sno = '" + textBox_Sno.Text + "' AND ";
                if (textBox_Sname.Text != "")
                    opt = opt + "Sname like '%" + textBox_Sname.Text + "%' AND ";
                if (textBox_Ssex.Text != "")
                    opt = opt + "Ssex = '" + textBox_Ssex.Text + "' AND ";
                if (textBox_Sbirthday.Text != "")
                    opt = opt + "Sbirthday = '" + textBox_Sbirthday.Text + "' AND ";
                if (textBox_Sacademy.Text != "")
                    opt = opt + "Sacademy = '" + textBox_Sacademy.Text + "' AND ";
                if (textBox_Sdept.Text != "")
                    opt = opt + "Sdept = '" + textBox_Sdept.Text + "' AND ";
                if (textBox_Syear.Text != "")
                    opt = opt + "Syear = '" + textBox_Syear.Text + "' AND ";
                if (textBox_Scet4.Text != "")
                    opt = opt + "Scet4 = " + textBox_Scet4.Text + " AND ";
                if (textBox_Sclass.Text != "")
                    opt = opt + "Sclass = '" + textBox_Sclass.Text + "' AND ";
                if (textBox_Sduty.Text != "")
                    opt = opt + "Sduty = '" + textBox_Sduty.Text + "' AND ";
                if (textBox_Sorigin.Text != "")
                    opt = opt + "Sorigin = '" + textBox_Sorigin.Text + "' AND ";
                if (textBox_Sqq.Text != "")
                    opt = opt + "Sqq = '" + textBox_Sqq.Text + "' AND ";
                opt = opt.Substring(0, opt.Length - 4);
                DO(opt,dataGridView4);//执行查询语句
            }
        }
        
        private void button8_Click(object sender, EventArgs e)//清空
        {
            textBox_Sno.Text = "";
            textBox_Sname.Text = "";
            textBox_Ssex.Text = "";
            textBox_Sbirthday.Text = "";
            textBox_Sacademy.Text = "";
            textBox_Sdept.Text = "";
            textBox_Syear.Text = "";
            textBox_Scet4.Text = "";
            textBox_Sclass.Text = "";
            textBox_Sduty.Text = "";
            textBox_Sorigin.Text = "";
            textBox_Sqq.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)//查询所有记录
        {
            DO("select * from Student", dataGridView4);
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)//打开关于界面
        {
            Attention frmAttention = new Attention();
            frmAttention.Show();
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)//退出
        {
            Environment.Exit(0);
        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)//保存到文件
        {
            SaveFileDialog pdlg = new SaveFileDialog(); //构造保存对话框
            pdlg.Filter = "查询结果(*.txt)|*.txt|所有文件(*.*)|(*.*)";
            DialogResult rt = pdlg.ShowDialog(); //显示对话框
            if (rt == DialogResult.OK) //判断对话框的返回是否有效
            {
                string sfile = pdlg.FileName; //获得对话框选择文件名
                //构造写文件流
                StreamWriter sw = new StreamWriter(sfile, true, Encoding.Default);
                //遍历datagridview数据
                int row = dataGridView4.Rows.Count;//得到总行数  
                int cell = dataGridView4.Rows[1].Cells.Count;//得到总列数
                try
                {
                    for (int i = 0; i < row - 1; i++)//得到总行数并在之内循环  
                    {
                        string sline = "";
                        for (int j = 0; j < cell; j++)
                            sline += dataGridView4.Rows[i].Cells[j].Value.ToString().Trim() + " ";
                        /*
                        //分别获得列表框数据内容
                        string Sno      = dataGridView4.Rows[i].Cells[0].Value.ToString().Trim();
                        string Sname    = dataGridView4.Rows[i].Cells[1].Value.ToString().Trim();
                        string Ssex     = dataGridView4.Rows[i].Cells[2].Value.ToString().Trim();
                        string Sacademy = dataGridView4.Rows[i].Cells[3].Value.ToString().Trim();
                        string Sdept    = dataGridView4.Rows[i].Cells[4].Value.ToString().Trim();
                        string Syear    = dataGridView4.Rows[i].Cells[5].Value.ToString().Trim();
                        string Scet4    = dataGridView4.Rows[i].Cells[6].Value.ToString().Trim();
                        string Sclass   = dataGridView4.Rows[i].Cells[7].Value.ToString().Trim();
                        string Sduty    = dataGridView4.Rows[i].Cells[8].Value.ToString().Trim();
                        string Sorigin  = dataGridView4.Rows[i].Cells[9].Value.ToString().Trim();
                        string Sqq      = dataGridView4.Rows[i].Cells[10].Value.ToString().Trim();
                        //格式化字符串
                        string sline = String.Format("{0,0} {1,4} {2,4} {3,4} {4,4} {5,4} {6,4} {7,4} {8,4} {9,4} {10,4}"
                            , Sno, Sname, Ssex, Sacademy, Sdept, Syear, Scet4, Sclass, Sduty, Sorigin, Sqq);*/
                        sw.WriteLine(sline); //写一行数据到文件
                    }
                    sw.Close(); //关闭写文件流
                    MessageBox.Show("保存成功", "提示");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }
            }
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)//打开查询结果文件
        {
            MessageBox.Show("待实现", "提示");
            /*
            string path;
            openFileDialog1.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
            openFileDialog1.InitialDirectory = "C:";//初始加载路径
            openFileDialog1.Filter = "查询结果文件 (*.txt)|*.txt";//过滤txt型文本文件
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //清空datagridview数据
                //while (this.dataGridView4.Rows.Count != 0)
                //    this.dataGridView4.Rows.RemoveAt(0);
                DataTable dt = (DataTable)dataGridView4.DataSource;
                dt.Rows.Clear();
                dataGridView4.DataSource = dt;
                //录入文件数据
                path = openFileDialog1.FileName.ToString();
                StreamReader data = new StreamReader(path, System.Text.Encoding.Default);
                //dataGridView4.Rows.Add();
                int i = 0;
                while (data.Peek() != -1)
                {
                    string str = data.ReadLine();  //读取文件中的一行字符
                    string[] strs = str.Split(new char[] { ' ' }, //以空格符分割字符串
                                                                  StringSplitOptions.RemoveEmptyEntries);
                    dataGridView4.Rows[i].Cells[0].Value = strs[0];
                    dataGridView4.Rows[i].Cells[1].Value = strs[1];
                    dataGridView4.Rows[i].Cells[2].Value = strs[2];
                    dataGridView4.Rows[i].Cells[3].Value = strs[3];
                    dataGridView4.Rows[i].Cells[4].Value = strs[5];
                    dataGridView4.Rows[i].Cells[5].Value = strs[6];
                    dataGridView4.Rows[i].Cells[6].Value = strs[7];
                    dataGridView4.Rows[i].Cells[7].Value = strs[8];
                    dataGridView4.Rows[i].Cells[8].Value = strs[9];
                    dataGridView4.Rows[i].Cells[9].Value = strs[10];
                    dataGridView4.Rows[i].Cells[10].Value = strs[11];
                    i++;
                }
                data.Close();  //关闭文件流
            }
            else
                return;*/
        }

        private void 密码修改ToolStripMenuItem_Click(object sender, EventArgs e)//密码修改
        {
            ChangePassword c = new ChangePassword(Member2);
            c.ShowDialog();
        }

        private void 权限修改ToolStripMenuItem_Click(object sender, EventArgs e)//权限修改
        {
            ChangeLevel c = new ChangeLevel(Member2);
            c.ShowDialog();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "Data Source=MJFT;Initial Catalog=Test1;Integrated Security=True";
            conn.Open();
            string opt2 = "select * from Members where Sno = '" + Member2[0] + "'";
            SqlCommand cmd2 = new SqlCommand(opt2, conn);
            SqlDataReader sdr;
            sdr = cmd2.ExecuteReader();
            sdr.Read();
            Member2[2]=sdr.GetInt32(2).ToString();
        }

        private void 内容CToolStripMenuItem_Click(object sender, EventArgs e)//操作指南
        {
            MessageBox.Show("待实现", "提示");
        }
    }
}
