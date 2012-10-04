﻿using System.Collections.Generic;
using System.ServiceModel;
using NSubstitute;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using TaskConsoleClient.Manager;
using System;
using TaskConsoleClient.UI.ConcreteHandlers;
using TaskManagerService.WCFService;
using Xunit;

namespace TaskConsoleClient.UI
{
    public static class Program
    {
        public static void Main()
        {
            Console.Title = "Task Manager Client";
            Console.WriteLine(TestConnection()
                ? "Connection established."
                : "Wrong server address.");

            var module = new TaskManagerModule();

            var kernel = new StandardKernel(module);

            for (string s; ((s = Console.ReadLine()) != null); )
                kernel.Get<ConsoleHelper>().Execute(s);
        }

        private static bool TestConnection()
        {
            return new ChannelFactory<ITaskManagerService>("tcpEndPoint")
                .CreateChannel()
                .TestConnection();
        }
    }

    public class TaskManagerModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind(x => x.FromThisAssembly()
                               .SelectAllClasses()
                               .InNamespaceOf<ICommandHandler>()
                               .BindAllInterfaces()
                               .Configure(b => b.InThreadScope()));

            Bind<IClientConnection>().To<ClientConnection>();
        }
    }
}

