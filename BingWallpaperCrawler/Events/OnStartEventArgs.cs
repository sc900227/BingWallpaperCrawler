using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingWallpaperCrawler.Events
{
    /// <summary>
    /// 爬虫启动事件
    /// </summary>
    public class OnStartEventArgs:System.EventArgs
    {
        public Uri Uri { get; set; }
        public OnStartEventArgs(Uri uri) {
            this.Uri = uri;
        }
    }
}
