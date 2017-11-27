using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace LogMonitor
{
    public class StreamHub : Hub
    {
        private FileSystemWatcher fileSystemWatcher;
        private Stream fileStream;
        
        static int count;
        //public ChannelReader<string> StreamLog(int count, int delay)
        //{
        //    var channel = Channel.CreateUnbounded<string>();

        //    Task.Run(async () =>
        //    {
        //        for (var i = 0; i < count; i++)
        //        {
        //            await channel.Writer.WriteAsync("Test");
        //            await Task.Delay(delay);
        //        }
        //        channel.Writer.TryComplete();
        //    });

        //    return channel.Reader;

        //}
        
        public IObservable<int> ObservableCounter(int count, int delay)
        {
            return Observable.Interval(TimeSpan.FromMilliseconds(delay))
                             .Select((_, index) => index)
                             .Take(count);
        }
    }
}


