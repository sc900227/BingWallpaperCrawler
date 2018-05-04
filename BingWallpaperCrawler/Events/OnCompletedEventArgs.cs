using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace BingWallpaperCrawler.Events
{
    /// <summary>
    /// 爬虫完成事件
    /// </summary>
    public class OnCompletedEventArgs:System.EventArgs
    {
        public Uri Uri { get;private set; }
        public int ThreadId { get; private set; }
        public string PageHtml { get; private set; }
        public IWebDriver WebDriver { get; private set; }
        public long Milliseconds { get; private set; }
        public OnCompletedEventArgs(Uri uri,int threadId,string pageHtml,IWebDriver webDriver,long milliseconds) {
            this.Uri = uri;
            this.ThreadId = threadId;
            this.PageHtml = pageHtml;
            this.WebDriver = webDriver;
            this.Milliseconds = milliseconds;
        }
    }
}
