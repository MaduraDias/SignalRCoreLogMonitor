using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading.Tasks.Channels;

namespace LogMonitor
{
    public class SniffHub : Hub
    {
        static FileSystemWatcher fileSystemWatcher;
        static ConcurrentBag<IClientProxy> clientList;
        static long lastPosition;

        static SniffHub()
        {
            fileSystemWatcher = new FileSystemWatcher(@"C:\temp");
            fileSystemWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite;
            fileSystemWatcher.Filter = "Error.txt";
            clientList = new ConcurrentBag<IClientProxy>();

            fileSystemWatcher.Changed += new FileSystemEventHandler((object sender, FileSystemEventArgs e) =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(50));
                using (Stream stream = File.Open(@"C:\temp\Error.txt", FileMode.Open, FileAccess.Read))
                {
                    stream.Seek(lastPosition, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(stream))
                    {
                        string record;
                        while ((record = streamReader.ReadLine()) != null)
                        {

                            clientList.ToList().ForEach(
                               clientProxy => clientProxy.InvokeAsync("notify", record));
                        }
                        lastPosition = stream.Position;
                    }
                }

            });
        }

            

        public void subscribe()
        {
            var t = base.Context.Connection.ProtocolReaderWriter;
            register(Clients.Client(base.Context.ConnectionId));
            
        }

        public static void register(IClientProxy client)
        {
            clientList.Add(client);
            using (Stream stream = File.Open(@"C:\temp\Error.txt", FileMode.Open, FileAccess.Read))
            {
                stream.Seek(lastPosition, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(stream))
                {
                    string record;
                    while ((record = streamReader.ReadLine()) != null)
                    {
                       client.InvokeAsync("notify", record);
                    }
                }
            }
            fileSystemWatcher.EnableRaisingEvents = true;
        }

    }


}
