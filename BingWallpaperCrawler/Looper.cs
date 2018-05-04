using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingWallpaperCrawler.Events;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace BingWallpaperCrawler
{
    public class Looper
    {
        public EventHandler<OnLoopEventArgs> OnLoop;
        public Action<string> actionSetMessage;
        private List<FileInfo> imageFiles;
        private int waitMin;
        private int loopIndex = 0;
        public BackgroundWorker worker{get;set;}

        public Looper(List<FileInfo> imageFiles, int waitMin)
        {
            this.imageFiles = imageFiles;
            this.waitMin = waitMin;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            MainWindow.stopWorkAction += () =>
            {
                Stop();
            };
            worker.DoWork += DoWorkLoop;
            worker.RunWorkerCompleted += RunWorkerCompletedLoop;
        }
        public void Stop() {
            this.worker.CancelAsync();
            actionSetMessage("后台线程已取消...");
        }
        public void Start()
        {
            worker.RunWorkerAsync();
            //return worker;
            //var loopSetWalls = new BackgroundWorker[3];
            //for (int i = 0; i < loopSetWalls.Length; i++)
            //{
            //    loopSetWalls[i] = new BackgroundWorker();
            //    loopSetWalls[i].WorkerSupportsCancellation = true;
            //    loopSetWalls[i].DoWork += DoWorkLoop;
            //    loopSetWalls[i].RunWorkerCompleted += RunWorkerCompletedLoop;
            //}
            //while (true)
            //{
            //    BackgroundWorker loopSetWall = loopSetWalls.FirstOrDefault(a => !a.IsBusy);
            //    if (loopSetWall != null)
            //    {
            //        lock (imagePaths)
            //        {
            //            loopSetWall.RunWorkerAsync(imagePaths);
            //            Thread.Sleep(6000);
            //        }
            //    }
            //    else
            //    {
            //        Thread.Sleep(1000);
            //    }
            //    //return loopSetWall;
            //}
        }
        private void DoWorkLoop(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker loopSetWall = sender as BackgroundWorker;
            actionSetMessage(string.Format("开始设置壁纸，循环索引：{0}...",loopIndex+1));
            int result = SystemWinApi.setWallpaperApi(imageFiles[loopIndex].FullName);
        }
        private void RunWorkerCompletedLoop(object sender, RunWorkerCompletedEventArgs e)
        {
            //如果用户取消了当前操作就关闭窗口。
            if (worker.CancellationPending == true)
            {
                return;
            }
            if (e.Cancelled)
            {
               return;
            }

            if (loopIndex + 1 < imageFiles.Count)
            {
                loopIndex++;
            }
            else
            {
                loopIndex = 0;
            }
            Delay(60000*waitMin);
            actionSetMessage("重启线程...");
            worker.RunWorkerAsync();
        }
        public void Delay(int mm)
        {
            DateTime current = DateTime.Now;
            while (current.AddMilliseconds(mm) > DateTime.Now)
            {
                Application.DoEvents();
            }
            return;
        }
        public void LoopSetStart(out int threadId)
        {
            while (true)
            {
                Task<bool> t = Task.Factory.StartNew<bool>(() =>
                {
                    return SystemWinApi.setWallpaperApi(imageFiles[loopIndex].FullName) > 0;

                    //if (this.OnLoop != null)
                    //{
                    //    this.OnLoop.Invoke(this, new OnLoopEventArgs(loopIndex));
                    //}
                });
                t.Wait();
                if (t.IsCompleted && t.Result)
                {
                    Thread.Sleep(waitMin*60000);
                    if (loopIndex + 1 < imageFiles.Count)
                    {
                        loopIndex++;
                    }
                    else
                    {
                        loopIndex = 0;
                    }

                }
            }

        }

        //public void Start(Task<bool> t)
        //{
        //    try
        //    {
        //        //var taskResult = LoopSetStart();
        //        //t.Start();
        //        //Thread.Sleep(10000);
        //        t.Wait();
        //        if (t.IsCompleted)
        //        {
        //            Thread.Sleep(6000);
        //            if (loopIndex + 1 < imagePaths.Count)
        //            {
        //                loopIndex++;
        //            }
        //            else
        //            {
        //                loopIndex = 0;
        //            }
        //            //t.Dispose();
        //            int threadId = 0;
        //            Start(LoopSetStart(out threadId));
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }



        //}
    }
}
