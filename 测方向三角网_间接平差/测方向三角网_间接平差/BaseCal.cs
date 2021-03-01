using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 测方向三角网_间接平差
{
    class BaseCal
    {
        public static double angel(double px1, double py1, double px2, double py2)//坐标方位角计算，输出十进制度
        {
            double A, rad;
            if (px1 == px2)
                if (py2 > py1)
                    A = 0;
                else A = 180;
            else
            {
                rad = Math.Atan((py2 - py1) / (px2 - px1));//结果为弧度
                A = rad * 180 / Math.PI;//转换成十进制度
                if (px2 < px1)
                    A = A + 180;
                if (px2 > px1 && py2 < py1)
                    A = A + 360;
            }
            return A;
        }

        public static double distance(double px1, double py1, double px2, double py2)//距离计算
        {
            double d;
            d = Math.Sqrt((py2 - py1) * (py2 - py1) + (px2 - px1) * (px2 - px1));
            return d;
        }

    }
}
