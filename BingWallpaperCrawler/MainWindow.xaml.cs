using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using OpenQA.Selenium;
using BingWallpaperCrawler.Events;
using System.Net;
using System.IO;
using System.Windows.Forms;
using BingWallpaperCrawler.Control;
using System.ComponentModel;

namespace BingWallpaperCrawler
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> ImageUrlList { get; set; }
        private BackgroundWorker worker;
        private string[] imageFiles;
        private Looper loop;
        public static Action stopWorkAction;
        public MainWindow()
        {
            InitializeComponent();
            string defaultPath=@"D:\Wallpaper";
            this.txtSavePath.Text=defaultPath;
            if (!System.IO.Directory.Exists(txtSavePath.Text))
                Directory.CreateDirectory(txtSavePath.Text);
            
        }
        /// <summary>
        /// 当前选择的图片路径
        /// </summary>
        private string currentImage = "";
        /// <summary>
        /// 默认循环切换壁纸时间（分钟）
        /// </summary>
        private int defaultCarouselTime = 5;
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var hotelUrl = "https://bing.ioliu.cn/";
            var crawler = new Crawler();
            crawler.OnStart += (s, p) =>
            {
                SetMessage("爬虫开始抓取地址：" + p.Uri.ToString());
            };
            crawler.OnError += (s, p) =>
            {
                SetMessage("爬虫抓取出现错误：" + p.Uri.ToString() + "，异常消息：" + p.Exception.ToString());
            };
            crawler.OnComplete += (s, p) =>
            {
                //WallpaperCrawler(p);
            };
            var operation = new Operation
            {
                Action = (x) =>
                {
                    //加载首页图片
                    List<IWebElement> list = x.FindElements(By.XPath("//*[@class='item']")).ToList();
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        LoadImageForIndex(list);
                    })); 
                   
                    return x;
                    
                },
                Condition = (x) =>
                {
                    //while (this.gImageList.Children.Count<12)
                    //{
                    //    SetMessage("加载图片中...");
                    //}
                    //SetMessage("加载完成...");
                    return true;
                    
                },
                Timeout = 5000
            };

            crawler.Start(new Uri(hotelUrl), null, operation);//不操作JS先将参数设置为NULL
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSavePath.Text))
            {
                System.Windows.Forms.MessageBox.Show("请选择图片保存路径！");
                return;
            }
            if (imageScrollView1.ChickCurrentIndex<0)
            {
                System.Windows.Forms.MessageBox.Show("请选择图片！");
                return;
            }
            currentImage = ImageUrlList[imageScrollView1.ChickCurrentIndex];
            DownAndSetWallpaper(currentImage, true);
            ////获取系统当前分辨率
            //string screenWidth=SystemParameters.PrimaryScreenWidth.ToString();
            //string screenHeight = SystemParameters.PrimaryScreenHeight.ToString();
            //string wh = string.Format("{0}x{1}", screenWidth, screenHeight);
            //SetMessage(string.Format("获取当前系统分辨率：{0}", wh));
            ////获取下载路径
            //currentImage = ImageUrlList[imageScrollView1.ChickCurrentIndex];
            //string downUrl = currentImage.Replace("800x480", wh);
            //string[] fileNames = downUrl.Replace("http://", "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            //string fileName = fileNames[fileNames.Length - 1];
            //string savePath = System.IO.Path.Combine(this.txtSavePath.Text, fileName);//@"d:/Wallpaper/"+fileName;
            ////开始下载图片
            //string[] imageFiles= Downer.ReadJpgNames(this.txtSavePath.Text, new string[] { fileName.Replace("jpg","bmp") });
            //Downer downer = new Downer(downUrl, savePath);
            //try
            //{
            //    if (imageFiles == null || imageFiles.Length <= 0)
            //    {
            //        downer.OnDown += (s, p) =>
            //        {
            //            SetMessage("开始下载图片...");
                        
            //            if (downer.SavePhotoFromUrl(p.savePath, p.downUrl))
            //            {
            //                SetMessage(string.Format("下载保存成功:{0}", savePath));
            //                var bmpPath = ChangeJpgToBmp(savePath);
            //                SetMessage("开始设置桌面背景...");
            //                if(SystemWinApi.setWallpaperApi(bmpPath)>0)
            //                SetMessage("设置成功...");
            //            }
            //        };
            //        downer.DownLoadAndSave();
            //    }
            //    else
            //    {
            //        SetMessage("开始设置桌面背景...");
            //        if(SystemWinApi.setWallpaperApi(savePath.Replace("jpg","bmp"))>0)
            //        SetMessage("设置成功...");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    SetMessage(ex.ToString());
               
            //}
            
        }
        private void DownAndSetWallpaper(string imageUrl, bool set)
        {
            //获取系统当前分辨率
            string screenWidth = SystemParameters.PrimaryScreenWidth.ToString();
            string screenHeight = SystemParameters.PrimaryScreenHeight.ToString();
            string wh = string.Format("{0}x{1}", screenWidth, screenHeight);
            SetMessage(string.Format("获取当前系统分辨率：{0}", wh));
            //获取下载路径
            //string imageUrl = ImageUrlList[imageIndex];
            string downUrl = imageUrl.Replace("800x480", wh);
            string[] fileNames = downUrl.Replace("http://", "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            string fileName = fileNames[fileNames.Length - 1];
            string savePath = System.IO.Path.Combine(this.txtSavePath.Text, fileName);//@"d:/Wallpaper/"+fileName;
            //开始下载图片
            string[] imageFiles = Downer.ReadJpgNames(this.txtSavePath.Text, new string[] { fileName.Replace("jpg", "bmp") });
            Downer downer = new Downer(downUrl, savePath);
            //壁纸图片路径
            string setWallPath = string.Empty;
            try
            {
                if (imageFiles == null || imageFiles.Length <= 0)
                {
                    downer.OnDown += (s, p) =>
                    {
                        SetMessage("开始下载图片...");

                        if (downer.SavePhotoFromUrl(p.savePath, p.downUrl))
                        {
                            SetMessage(string.Format("下载保存成功:{0}", savePath));
                            var bmpPath = ChangeJpgToBmp(savePath);
                            setWallPath = bmpPath;
                        }
                    };
                    downer.DownLoadAndSave();
                }
                else
                {
                    setWallPath=savePath.Replace("jpg", "bmp");
                }
                if (set)
                {
                    SetMessage("开始设置桌面背景...");
                    if (SystemWinApi.setWallpaperApi(setWallPath) > 0)
                        SetMessage("设置成功...");
                }
            }
            catch (Exception ex)
            {
                SetMessage(ex.ToString());

            }
        }
        /// <summary>
        /// 将jpg格式图片转换为bmp位图
        /// </summary>
        /// <param name="imagePath">jpg图片路径</param>
        /// <returns>bmp图片路径</returns>
        public string ChangeJpgToBmp(string imagePath) {
            string bmpPath=string.Empty;
            if (imagePath.Contains("jpg"))
            {
                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imagePath))
                {
                    bmpPath = imagePath.Replace("jpg", "bmp");
                    bitmap.Save(bmpPath, System.Drawing.Imaging.ImageFormat.Bmp);
                    bitmap.Dispose();
                }
                File.Delete(imagePath);
            }
            return bmpPath;
        }
        /// <summary>
        /// 加载壁纸图片
        /// </summary>
        /// <param name="driverList"></param>
        private void LoadImageForIndex(List<IWebElement> driverList) {
            List<BitmapImage> ls_adv_img = new List<BitmapImage>();
            string[] IMAGES = new string[driverList.Count];
            //List<string> images=new List<string>();
            ImageUrlList = new List<string>();
            if (driverList!=null&&driverList.Count>0)
            {
                SetMessage("开始加载图片...");
                
                for (int i = 0; i < driverList.Count; i++)
                {
                    var imageUrl = driverList[i].FindElement(By.TagName("img")).GetAttribute("data-progressive");
                    ImageUrlList.Add(imageUrl);
                    //Border border = new Border();
                    //border.Tag = imageUrl;
                    //border.Name = "bd" + (i + 1).ToString();
                    //border.Margin = new Thickness(0, 0, 0, 20);
                    //border.BorderThickness = new Thickness(5);
                    //Image image = new Image();
                    //image.Source = new BitmapImage(new Uri(imageUrl));
                    //image.Height=200;
                    //image.Width = 400;
                    //image.Name = "img" + (i + 1).ToString();
                    ////image.PreviewMouseLeftButtonDown
                    //border.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(bd_PreviewMouseLeftButtonDown);
                    //if (i<3)
                    //{
                    //    border.SetValue(Grid.RowProperty, 0);
                    //    border.SetValue(Grid.ColumnProperty, i);
                    //}
                    //else if (i<6)
                    //{
                    //    border.SetValue(Grid.RowProperty,1);
                    //    border.SetValue(Grid.ColumnProperty, i - 3);
                    //}
                    //else if (i < 9) {
                    //    border.SetValue(Grid.RowProperty,2);
                    //    border.SetValue(Grid.ColumnProperty, i - 6);
                    //}
                    //else if (i < 12)
                    //{
                    //    border.SetValue(Grid.RowProperty, 3);
                    //    border.SetValue(Grid.ColumnProperty, i - 9);
                    //}
                    //border.Child = image;
                    //this.gImageList.Children.Add(border);
                }
                IMAGES = ImageUrlList.ToArray();
                imageScrollView1.ChildViewWidth = 400;
                imageScrollView1.ChildViewHeight = 200;
                imageScrollView1.SpaceWidth = 300;
                imageScrollView1.AddImages(IMAGES);
                imageScrollView1.Enableslide = false;
                //imageScrollView1.SlideAffect = ImageScrollView.SlideAffectEnum.JumpSlide;//.OrderSlide;
                
            }
            SetMessage("加载完成...");
        }
        void bd_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Border bd = sender as Border;
            foreach (var item in this.gImageList.Children)
            {
                Border border = item as Border;
                border.BorderBrush = new SolidColorBrush(Colors.White);
            }
            //for (int i = 0; i < 12; i++)
            //{
            //    Border border = this.gImageList.FindName("bd" + (i + 1).ToString()) as Border;
            //    border.BorderBrush = new SolidColorBrush(Colors.White);//new SolidColorBrush(Colors.LightSkyBlue);
            //}
            bd.BorderBrush = new SolidColorBrush(Colors.LightSkyBlue);
            this.currentImage = bd.Tag.ToString();
        }
        
        /// <summary>
        /// 设置显示消息
        /// </summary>
        /// <param name="messasge">消息内容</param>
        public void SetMessage(string messasge)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.txtMessage.AppendText(messasge);
                this.txtMessage.AppendText(Environment.NewLine);
                this.txtMessage.ScrollToEnd();
                DoEvents();
            }));
        }
        private static DispatcherOperationCallback exitFrameCallback = new DispatcherOperationCallback(ExitFrame);
        public static void DoEvents()
        {
            DispatcherFrame nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);
            if (exitOperation.Status !=
            DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }
        private static Object ExitFrame(Object state)
        {
            DispatcherFrame frame = state as
            DispatcherFrame;
            frame.Continue = false;
            return null;
        }

        private void btnSetFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();
            this.txtSavePath.Text = m_Dir;  
        }

        private void btnDownAll_Click(object sender, RoutedEventArgs e)
        {
            if (ImageUrlList==null||ImageUrlList.Count==0)
            {
                System.Windows.MessageBox.Show("请先抓取图片！");
                return;
            }
            foreach (var item in ImageUrlList)
            {
                DownAndSetWallpaper(item, false);
            }
        }

        private void btnStartCarousel_Click(object sender, RoutedEventArgs e)
        {
            string[] imageFiles = Downer.ReadJpgNames(this.txtSavePath.Text, new string[] { "*.bmp" });
            if (imageFiles == null || imageFiles.Length == 0)
            {
                System.Windows.MessageBox.Show("请先下载图片！");
                return;
            }
            List<FileInfo> fileInfoList = new List<FileInfo>();
            foreach (var item in imageFiles)
            {
                fileInfoList.Add(new FileInfo(item));
            }
            fileInfoList= fileInfoList.OrderByDescending(a => a.CreationTime).ToList();
            
            
            //Looper loop = new Looper(imageFiles.ToList(), 2);
            //loop.OnLoop += (s, p) =>
            //{
            //    SetMessage("开始切换桌面背景...");
            //    if (SystemWinApi.setWallpaperApi(imageFiles[p.LoopIndex]) > 0)
            //        SetMessage("切换成功...");
            //};
            //int threadId = 0;
            SetMessage("启动轮循线程...");
            loop = new Looper(fileInfoList, 2);
            loop.actionSetMessage += (m) =>
            {
                SetMessage(m);
            };
            loop.Start();
            //this.worker = loop.worker;
            //loop.LoopSetStart(out threadId);
            this.btnStartCarousel.IsEnabled = false;
            this.btnCancel.IsEnabled = true;
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //停止后台线程工作
            stopWorkAction();
            //禁用取消按钮
            this.btnCancel.IsEnabled = false;
            //启用启动按钮
            this.btnStartCarousel.IsEnabled = true;
        }

        
    }
}
