using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 测方向三角网_间接平差
{
    class File
    {
        public static void CreateTxtWithText(string savePath, string text)
        {
            try
            {
                System.IO.File.WriteAllText(savePath, text);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}