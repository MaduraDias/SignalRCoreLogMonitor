using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LogMonitor
{
    public class StreamHub:Hub
    {
        public ChannelReader<string> StreamLog(int count, int delay)
        {
            var channel = Channel.CreateUnbounded<string>();

            Task.Run(async () =>
            {
                for (var i = 0; i < count; i++)
                {
                    await channel.Writer.WriteAsync("Test");
                    await Task.Delay(delay);
                }

                channel.Writer.TryComplete();
            });

            return channel.Reader;

        }

      

    }

    
}
