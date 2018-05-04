using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace BingWallpaperCrawler
{
    public class Operation
    {
        public int Timeout { get; set; }

        public Func<IWebDriver,IWebDriver> Action { get; set; }

        public Func<IWebDriver, bool> Condition { get; set; }
    }
}
