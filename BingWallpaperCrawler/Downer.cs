using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingWallpaperCrawler.Events;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace BingWallpaperCrawler
{
    
    public class Downer
    {
        public event EventHandler<OnDownLoadEventArgs> OnDown;
        //public event EventHandler<OnSaveEventArgs> OnSave;
        public string downLoadUrl { get; private set; }
        public string savePath { get; private set; }
        public Downer(string downLoadUrl,string savePath) {
            this.downLoadUrl = downLoadUrl;
            this.savePath = savePath;
        }

        public Task DownLoadAndSave() {
            return Task.Factory.StartNew(() => {
                if (this.OnDown != null) this.OnDown(this,new OnDownLoadEventArgs(downLoadUrl,savePath));
                //if (this.OnSave != null) this.OnSave(this,new OnSaveEventArgs(savePath));
            });
        }
        /// <summary>
        /// 读取指定格式的多个文件，返回文件名数组
        /// </summary>
        /// <param name="jpgFolder">要读取的文件所在的文件夹</param>
        /// <param name="searchPatterns">如："*-???-green.jpg", "*-???-green.jpeg"</param>
        /// <returns>文件名组成的字符串数组</returns>
        public static string[] ReadJpgNames(string jpgFolder, params string[] searchPatterns)
        {
            string[] str = getImgNames(jpgFolder, searchPatterns);//"*-???-green.jpg", "*-???-green.jpeg");
            if (str == null) return null;
            str = str//.Where(s=>s.ToLower().EndsWith("-green.jpg")||s.ToLower().EndsWith("-green.jpeg"))
                .OrderBy(s => s)
                .ToArray<string>();
            return str;
        }

        private static string[] getImgNames(string dirPath, params string[] searchPatterns)
        {

            if (searchPatterns.Length <= 0)
                return null;
            else
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dirPath);
                if (!di.Exists) return null;

                System.IO.FileInfo[][] fis = new System.IO.FileInfo[searchPatterns.Length][];
                int count = 0;
                for (int i = 0; i < searchPatterns.Length; i++)
                {
                    System.IO.FileInfo[] fileinfos = di.GetFiles(searchPatterns[i]);
                    fis[i] = fileinfos;
                    count += fileinfos.Length;
                }
                string[] files = new string[count];
                int n = 0;
                for (int i = 0; i <= fis.GetUpperBound(0); i++)
                {
                    for (int j = 0; j < fis[i].Length; j++)
                    {
                        string temp = fis[i][j].FullName;
                        files[n] = temp;
                        n++;
                    }
                }
                return files;
            }
        }
        /// <summary>
        /// 从图片地址下载图片到本地磁盘
        /// </summary>
        /// <param name="ToLocalPath">图片本地磁盘地址</param>
        /// <param name="Url">图片网址</param>
        /// <returns></returns>
        public bool SavePhotoFromUrl(string FileName, string Url)
        {
            bool Value = false;
            WebResponse response = null;
            Stream stream = null;
            
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

                response = request.GetResponse();
                stream = response.GetResponseStream();

                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    Value = SaveBinaryFile(response, FileName);

                }

            }
            catch (Exception err)
            {
                string aa = err.ToString();
            }
            return Value;
        }

        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="response">The response used to save the file</param>
        // 将二进制文件保存到磁盘
        private bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                Stream outStream = System.IO.File.Create(FileName);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch
            {
                Value = false;
            }
            return Value;
        }
    }
}
