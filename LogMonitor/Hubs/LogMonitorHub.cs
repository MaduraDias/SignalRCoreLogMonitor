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
    public class LogMonitorHub : Hub
    {
        static FileSystemWatcher fileSystemWatcher;
        static ConcurrentDictionary<string, IClientProxy> clientList;
        static long lastPosition;

        static LogMonitorHub()
        {
            fileSystemWatcher = new FileSystemWatcher(@"C:\temp");
            fileSystemWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite;
            fileSystemWatcher.Filter = "Error.txt";
            clientList = new ConcurrentDictionary<string, IClientProxy>();

            fileSystemWatcher.Changed += new FileSystemEventHandler((object sender, FileSystemEventArgs e) =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(50));
                publishLogChange();
            });
        }

        private static void publishLogChange()
        {
            using (Stream stream = File.Open(@"C:\temp\Error.txt", FileMode.Open, FileAccess.Read))
            {
                stream.Seek(lastPosition, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(stream))
                {
                    string record;
                    while ((record = streamReader.ReadLine()) != null)
                    {
                        clientList.ToList().ForEach(
                           clientProxy => clientProxy.Value.InvokeAsync("notify", record));
                    }
                    lastPosition = stream.Position;
                }
            }
        }

        public void subscribe()
        {
            register(base.Context.ConnectionId, Clients.Client(base.Context.ConnectionId));
        }

        public static void register(string connectionId, IClientProxy client)
        {
            clientList.TryAdd(connectionId, client);
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            clientList.TryRemove(base.Context.ConnectionId, out IClientProxy clientProxy);
            return base.OnDisconnectedAsync(exception);
        }

    }


}
