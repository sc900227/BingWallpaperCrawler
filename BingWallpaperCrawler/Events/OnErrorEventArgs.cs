using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingWallpaperCrawler.Events
{
    public class OnErrorEventArgs:System.EventArgs
    {
        public Uri Uri { get; set; }
        public Exception Exception { get; set; }
        public OnErrorEventArgs(Uri uri,Exception exception) {
            this.Uri = uri;
            this.Exception = exception;

        }
    }
}
