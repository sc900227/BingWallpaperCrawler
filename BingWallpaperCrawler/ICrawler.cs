using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingWallpaperCrawler.Events;
using System.Threading.Tasks;

namespace BingWallpaperCrawler
{
    public interface ICrawler
    {
        event EventHandler<OnStartEventArgs> OnStart;
        event EventHandler<OnCompletedEventArgs> OnComplete;
        event EventHandler<OnErrorEventArgs> OnError;

        Task Start(Uri uri, Script script, Operation operation);
    }
}
