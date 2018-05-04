using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingWallpaperCrawler.Events
{
    public class OnSaveEventArgs:System.EventArgs
    {
        /// <summary>
        /// 文件保存地址
        /// </summary>
        public string FilePath { get; set; }
        public OnSaveEventArgs(string filePath) {
            this.FilePath = filePath;
        }
    }
}
