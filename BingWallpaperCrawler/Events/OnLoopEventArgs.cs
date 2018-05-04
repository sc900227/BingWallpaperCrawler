using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingWallpaperCrawler.Events
{
    public class OnLoopEventArgs:EventArgs
    {
        public int LoopIndex{get;set;}
        public OnLoopEventArgs(int loopIndex)
        {
            this.LoopIndex = loopIndex;
        }
    }
}
