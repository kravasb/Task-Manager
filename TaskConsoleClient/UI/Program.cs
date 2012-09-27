<<<<<<< HEAD
﻿using TaskConsoleClient.Manager;
﻿using System;


namespace TaskConsoleClient.UI
{
    static class Program
    {
        static void Main()
        {
            var client = new CommandManager();
            while (true)
            {
                client.Run();
            }
        }
    }
}
=======
﻿namespace TaskConsoleClient.UI
{
    {
        static void Main()
        {
            var task = new ContractTask();
            var factory = new ChannelFactory<ITaskManagerService>(new NetTcpBinding(), "net.tcp://localhost:44444");
            var client = factory.CreateChannel();
            client.AddTask(task);
        }
    }
}
>>>>>>> added wcf client
