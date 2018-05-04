using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium.PhantomJS;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using BingWallpaperCrawler.Events;

namespace BingWallpaperCrawler
{
    public class Crawler:ICrawler
    {
        public event EventHandler<OnStartEventArgs> OnStart;//爬虫启动事件

        public event EventHandler<OnCompletedEventArgs> OnComplete;//爬虫完成事件

        public event EventHandler<OnErrorEventArgs> OnError;//爬虫出错事件

        private PhantomJSOptions _options;//定义PhantomJS内核参数

        private PhantomJSDriverService _service;//定义Selenium驱动配置

        public Crawler(string proxy = null)
        {
            this._options = new PhantomJSOptions();//定义PhantomJS的参数配置对象
            this._service = PhantomJSDriverService.CreateDefaultService(Environment.CurrentDirectory);//初始化Selenium配置，传入存放phantomjs.exe文件的目录
            _service.IgnoreSslErrors = true;//忽略证书错误
            _service.WebSecurity = false;//禁用网页安全
            _service.HideCommandPromptWindow = true;//隐藏弹出窗口
            _service.LoadImages = false;//禁止加载图片
            _service.LocalToRemoteUrlAccess = true;//允许使用本地资源响应远程 URL
            _options.AddAdditionalCapability(@"phantomjs.page.settings.userAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36");
            if (proxy != null)
            {
                _service.ProxyType = "HTTP";//使用HTTP代理
                _service.Proxy = proxy;//代理IP及端口
            }
            else
            {
                _service.ProxyType = "none";//不使用代理
            }
        }

        public Task Start(Uri uri, Script script, Operation operation)
        {
           return Task.Factory.StartNew(() =>
            {
                if (OnStart != null) this.OnStart(this, new OnStartEventArgs(uri));
                var driver = new PhantomJSDriver(_service, _options);//实例化PhantomJS的WebDriver
                try
                {

                    var watch = DateTime.Now;
                    driver.Navigate().GoToUrl(uri.ToString());//请求URL地址
                    if (script != null) driver.ExecuteScript(script.Code, script.Args);//执行Javascript代码
                    if (operation.Action != null) operation.Action.Invoke(driver);
                    var driverWait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(operation.Timeout));//设置超时时间为x毫秒
                    if (operation.Condition != null) driverWait.Until(operation.Condition);
                    var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;//获取当前任务线程ID
                    var milliseconds = DateTime.Now.Subtract(watch).Milliseconds;//获取请求执行时间;
                    var pageSource = driver.PageSource;//获取网页Dom结构
                    if(this.OnComplete!=null)this.OnComplete.Invoke(this, new OnCompletedEventArgs(uri,threadId,pageSource,driver,milliseconds));
                }
                catch (Exception ex)
                {
                    if (this.OnError != null) OnError.Invoke(this, new OnErrorEventArgs(uri, ex));
                }
                finally
                {
                    driver.Close();
                    driver.Quit();
                }
            });
        }
    }
}
