using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Windows;
using Microsoft.Win32;

namespace BingWallpaperCrawler
{
    public class SystemWinApi
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
                int uAction,
                int uParam,
                string lpvParam,
                int fuWinIni
                );
        public static int setWallpaperApi(string strSavePath)
        {
           return SystemParametersInfo(20, 1, strSavePath, 0x2);
        }

        public static void GetScreenPix() {

            //double workWidth = SystemParameters.WorkArea.Width; // 屏幕工作区域宽度
            //double workHeight = SystemParameters.WorkArea.Height; // 屏幕工作区域高度
            double screenWidth = SystemParameters.PrimaryScreenWidth; // 屏幕整体宽度
            double screenHeight = SystemParameters.PrimaryScreenHeight; // 屏幕整体高度
        }
    }
}
