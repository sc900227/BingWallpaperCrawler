using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingWallpaperCrawler.Events
{
    /// <summary>
    /// 下载事件
    /// </summary>
    public class OnDownLoadEventArgs:System.EventArgs
    {
        /// <summary>
        /// 下载地址
        /// </summary>
        public string downUrl { get; set; }
        public string savePath { get; set; }
        public OnDownLoadEventArgs(string downUrl,string filePath) {
            this.downUrl = downUrl;
            this.savePath = filePath;
        }
    }
}
