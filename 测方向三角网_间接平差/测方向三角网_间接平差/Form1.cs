using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;//文件浏览
using System.Runtime.InteropServices.ComTypes;

namespace 测方向三角网_间接平差
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        string path = "0";

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)//文件-打开
        {
            openFileDialog1.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
            openFileDialog1.InitialDirectory = "C:";//初始加载路径
            openFileDialog1.Filter = "文本文件 (*.txt)|*.txt";//过滤txt型文本文件
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName.ToString();
                //path = Path.GetFileName(openFileDialog1.FileName);//显示文件的名字
            }
            if (path != "0")
            {
                StreamReader data = new StreamReader(path);
                while (data.Peek() != -1)
                {
                    string str = data.ReadLine();  //读取文件中的一行字符
                    richTextBox1_Show.Text = richTextBox1_Show.Text + str + '\n'; //输出一行
                }
                data.Close();  //关闭文件流
            }
        }

        private void 文件FToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)//计算
        {
            int GivenPointNum = 0, UnknowPointNum = 0, ValueNum = 0;
            int i = 0, j = 0, k = 0, n = 0, m = 0;
            string[] PointName = new string[100];//已知点及待求点
            double[,] PointData = new double[2, 100];//已知点及待求点
            string[,] ValueName = new string[3, 100];//观测值    0_station   1_观测方向    2_观测类型
            double[,] ValueData = new double[9, 100];//观测值    0_L   1_ΔY0   2_ΔX0   3_S0   4_A0   5_a   6_b   7_l

            ////////数据录入////////
            if (path == "0")
            {
                MessageBox.Show("请选择文件", "提示");
                return;
            }
            StreamReader data = new StreamReader(path);
            ////////读取第一行
            if (data.Peek() != -1)
                data.ReadLine();
            ////////读取已知点////////
            string str;
            for (i = 0, str = data.ReadLine(); str.IndexOf(',') != -1; i++)
            {
                PointName[i] = str.Substring(0, str.IndexOf(','));
                PointData[0, i] = double.Parse(str.Substring(str.IndexOf(',') + 1, str.LastIndexOf(',') - str.IndexOf(',') - 1));
                PointData[1, i] = double.Parse(str.Substring(str.LastIndexOf(',') + 1, str.Length - str.LastIndexOf(',') - 1));
                GivenPointNum++;
                str = data.ReadLine();
            }
            ////////读取观测数据////////
            string station = "empty";
            int stationNum = 0;
            while (str != null)
            {
                bool Unknow = true;
                if (str.IndexOf(',') == -1)//本行无,
                {
                    station = str;
                    stationNum++;
                    for (j = 0; j < GivenPointNum; j++)
                        if (str == PointName[j])
                            Unknow = false;
                    if (Unknow)
                    {
                        PointName[GivenPointNum + UnknowPointNum] = str;
                        UnknowPointNum++;
                    }
                }
                else
                {
                    ValueName[0, ValueNum] = station;
                    ValueName[1, ValueNum] = str.Substring(0, str.IndexOf(','));
                    ValueName[2, ValueNum] = str.Substring(str.IndexOf(',') + 1, 1);
                    ValueData[0, ValueNum] = double.Parse(str.Substring(str.LastIndexOf(',') + 1, str.Length - str.LastIndexOf(',') - 1));
                    ValueNum++;
                }
                str = data.ReadLine();
            }
            data.Close();  //关闭文件流

            ////////计算待求点近似值////////
            PointData[0, GivenPointNum + UnknowPointNum - 1] = 10122.12;//X0
            PointData[1, GivenPointNum + UnknowPointNum - 1] = 10312.47;//Y0

            ////////计算ΔY0、ΔX0、S0、A0、a、b////////
            for (i = 0; i < ValueNum; i++)
            {
                //读取两点名字
                for (n = 0; n < GivenPointNum; n++)
                    if (ValueName[0, i] == PointName[n])
                        break;
                for (m = 0; m < GivenPointNum; m++)
                    if (ValueName[1, i] == PointName[m])
                        break;
                //ΔY0
                ValueData[1, i] = PointData[1, n] - PointData[1, m];
                //ΔX0
                ValueData[2, i] = PointData[0, n] - PointData[0, m];
                //S0
                ValueData[3, i] = BaseCal.distance(PointData[0, n], PointData[1, n], PointData[0, m], PointData[1, m]);
                //A0
                ValueData[4, i] = BaseCal.angel(PointData[0, n], PointData[1, n], PointData[0, m], PointData[1, m]);
                if (ValueName[0, i] == "D" || ValueName[1, i] == "D")
                {
                    //a
                    ValueData[5, i] = (206265 * ValueData[1, i]) / (ValueData[3, i] * ValueData[3, i] * 10);
                    //b
                    ValueData[6, i] = -1 * (206265 * ValueData[2, i]) / (ValueData[3, i] * ValueData[3, i] * 10);
                }
            }

            ////////计算Z0////////
            double[] Z0 = new double[stationNum];
            i = 0;
            j = 0;
            k = 0;
            while (i < ValueNum)
            {
                station = ValueName[0, i];
                if (ValueData[4, i] - ValueData[0, i] >= 0)
                    Z0[j] = Z0[j] + ValueData[4, i] - ValueData[0, i];//A0-L
                else
                    Z0[j] = Z0[j] + ValueData[4, i] - ValueData[0, i] + 360;//A0-L
                k++;
                i++;
                if (i == ValueNum || ValueName[0, i] != station)
                {
                    Z0[j] = Z0[j] / k;
                    j++;
                    k = 0;
                }
            }

            ////////计算-l////////
            for (i = 0; i < ValueNum; i++)
            {
                /*for (j = 0; j < GivenPointNum; j++)
                    if (ValueName[0, i] == PointName[j])
                        break;*/
                switch (ValueName[0, i])
                {
                    case "D": j = 0; break;
                    case "C": j = 1; break;
                    case "A": j = 2; break;
                    case "B": j = 3; break;
                }
                if (ValueData[4, i] - ValueData[0, i] >= 0)
                    ValueData[7, i] = ValueData[4, i] - ValueData[0, i] - Z0[j];//A0-L-Z0
                else
                    ValueData[7, i] = ValueData[4, i] - ValueData[0, i] + 360 - Z0[j];//A0-L-Z0
            }

            ////////建立矩阵：参数值X0、参数改正值x、参数平差值X、观测值改正数v、观测值ValueL、观测值平差值L、误差方程系数B、 权阵P、自由项l////////
            string X0 = "";
            X0 += Z0[0] + "\n" + Z0[1] + "\n" + Z0[2] + "\n" + Z0[3] + "\n";
            for (i = 0; i < UnknowPointNum; i++)
                X0 += PointData[0, GivenPointNum + i] + "\n" + PointData[1, GivenPointNum + i] + "\n";
            //X0 += Z0[0] + "\n" + Z0[1] + "\n" + Z0[2] + "\n" + Z0[3] + "\n" + PointData[0, GivenPointNum + i] + "\n" + PointData[1, GivenPointNum + i] + "\n";
            string ValueL = "";
            for (i = 0; i < ValueNum; i++)
                ValueL += ValueData[0, i] + "\n";
            string B = "";
            for (i = 0; i < ValueNum; i++)
            {
                switch (ValueName[0, i])
                {
                    case "D": B += "-1,0,0,0,"; break;
                    case "C": B += "0,-1,0,0,"; break;
                    case "A": B += "0,0,-1,0,"; break;
                    case "B": B += "0,0,0,-1,"; break;
                }
                if (ValueName[0, i] == "D")
                    B += (-1) * ValueData[5, i] + "," + (-1) * ValueData[6, i] + '\n';
                else
                    if (ValueName[1, i] == "D")
                        B += ValueData[5, i] + "," + ValueData[6, i] + '\n';
                    else
                        B += "0,0" + '\n';
            }
            string P = "";
            for (i = 0; i < ValueNum; i++)
            {
                for(j=0;j<ValueNum;j++)
                {
                    if (j == i % 10 && ValueName[2, j] == "L")//若观测类型为等精度的角观测，权为1
                        P += "1,";
                    else
                        P += "0,";
                }
                P = P.Substring(0, P.Length - 1);
                P += "\n";
            }
            string l = "";
            for(i=0;i<ValueNum;i++)
                l = l + (-1) * ValueData[7, i] * 3600 + '\n';
            string x;
            Matrix Matrix_X0 = new Matrix();
            Matrix Matrix_x = new Matrix();
            Matrix Matrix_X = new Matrix();
            Matrix Matrix_v = new Matrix();
            Matrix Matrix_ValueL = new Matrix();
            Matrix Matrix_L = new Matrix();
            Matrix Matrix_B = new Matrix();
            Matrix Matrix_P = new Matrix();
            Matrix Matrix_l = new Matrix();
            Matrix Matrix_N_BB_ni = new Matrix();
            Matrix Matrix_N_BB = new Matrix();
            Matrix Matrix_D_xx = new Matrix();
            double sigma0;
            double sigmaP;
            using (StreamWriter obi = new StreamWriter("X0"))
            {
                obi.Write(X0);
            }
            using (StreamWriter obi = new StreamWriter("ValueL"))
            {
                obi.Write(ValueL);
            }
            using (StreamWriter obi = new StreamWriter("B"))
            {
                obi.Write(B);
            }
            using (StreamWriter obi = new StreamWriter("P"))
            {
                obi.Write(P);
            }
            using (StreamWriter obi = new StreamWriter("l"))
            {
                obi.Write(l);
            }
            Matrix_X0 = Matrix.LoadFromTextFile("X0");
            Matrix_ValueL =Matrix.LoadFromTextFile("ValueL");
            Matrix_B = Matrix.LoadFromTextFile("B");
            Matrix_P = Matrix.LoadFromTextFile("P");
            Matrix_l = Matrix.LoadFromTextFile("l");
            ////////计算结果////////
            //x=((BT*P*B)^-1)*BT*P*l
            Matrix_N_BB = Matrix_B.Transpose().Multiply(Matrix_P).Multiply(Matrix_B);
            Matrix_N_BB_ni = Matrix_B.Transpose().Multiply(Matrix_P).Multiply(Matrix_B).InvertGaussJordan();
            Matrix_x = Matrix_N_BB_ni.Multiply(Matrix_B.Transpose()).Multiply(Matrix_P).Multiply(Matrix_l);
            //v=B*x-l
            Matrix_v = Matrix_B.Multiply(Matrix_x).Subtract(Matrix_l);
            //L=ValueL+v
            Matrix_L = Matrix_ValueL.Add(Matrix_v.Multiply(Math.PI / 180));
            //X=X0+x/10
            Matrix_X = Matrix_X0.Add(Matrix_x.Multiply(0.1));
            //验后单位权中误差sigma0σ=VT*P*V/(c-u)
            sigma0 = Math.Sqrt(double.Parse(Matrix_v.Transpose().Multiply(Matrix_P).Multiply(Matrix_v).ToString()) / (ValueNum - 4 - UnknowPointNum * 2));
            //方差-协方差阵D_xx=σ^2*(N_BB)^-1
            Matrix_D_xx = Matrix_N_BB_ni.Multiply(sigma0 * sigma0);
            //点位中误差sigmaP=σ0*(Qxx+Qyy)^(1/2)
            sigmaP = sigma0 * Math.Sqrt(Matrix_N_BB_ni.GetElement(4, 4) + Matrix_N_BB_ni.GetElement(5, 5));

            ////////输出结果////////
            Text=
                     "参数改正值x =\n" + Matrix_x.ToString()
                 + "\n参数平差值X=\n" + Matrix_X.ToString()
                 + "\n观测值改正数（″）v=\n" + Matrix_v.ToString()
                 + "\n观测值平差值L=\n" + Matrix_L.ToString()
                 + "\n方差-协方差阵D_xx=\n" + Matrix_D_xx.ToString()
                 + "\n验后单位权中误差=" + sigma0
                 + "\n\n点位中误差=" + sigmaP;
            /*Text =
                  "误差方程系数B=\n" + Matrix_B.ToString()
                + "\n误差方程系数的转置BT=\n" + Matrix_B.Transpose().ToString()
                + "\n权阵P=\n" + Matrix_P.ToString()
                + "\n自由项l=\n" + Matrix_l.ToString()
                + "\n参数改正值x=\n" + Matrix_x.ToString()
                + "\n参数平差值X=\n" + Matrix_X.ToString()
                + "\n观测值改正数（″）v=\n" + Matrix_v.ToString()
                + "\n观测值平差值L=\n" + Matrix_L.ToString()
                + "\n协因数阵N_BB_ni=\n" + Matrix_N_BB_ni.ToString()
                + "\n协因数阵N_BB=\n" + Matrix_N_BB.ToString()
                + "\n方差-协方差阵D_xx=\n" + Matrix_D_xx.ToString()
                + "\n验后单位权中误差=" + sigma0
                + "\n\n点位中误差=" + sigmaP;*/
            Result c = new Result(Text);//传递结果至结果窗口
            c.ShowDialog();//显示结果窗口
        }

        private void 新建NToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
