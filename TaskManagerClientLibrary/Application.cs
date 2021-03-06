﻿using System;
using ConnectToWcf;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using TaskManagerClientLibrary.CommandContainer;
using TaskManagerClientLibrary.ConcreteCommands;

namespace TaskManagerClientLibrary
{
    public class Application
    {
        public void Run()
        {
            Console.Title = "Task Manager Client";

            var module = new TaskManagerModule();

            var kernel = new StandardKernel(module);
            var notifier = kernel.Get<UserNotifier>();

            var greeting = notifier.GenerateGreeting();
            Console.WriteLine(greeting);

            for (string s; ((s = Console.ReadLine()) != null); )
            {
                kernel.Get<LineParser>().ExecuteCommand(s);
            }
        }
    }

    public class TaskManagerModule : NinjectModule
    {

        public override void Load()
        {
            this.Bind(x => x.FromThisAssembly()
                               .SelectAllClasses()
                               .InNamespaceOf<ICommand>()
                               .BindAllInterfaces().Configure(b => b.WithConstructorArgument("textWriter", Console.Out))
                );


            Bind<ICommandContainer>()
                .To<CommandContainer.CommandContainer>()
                .InSingletonScope()
                .WithConstructorArgument("commands", Kernel.GetAll<ICommand>());
            var configManager = new ConfigurationManager();
            var address = configManager.GetAddress();

            Bind<UserNotifier>()
                .ToSelf()
                .WithConstructorArgument("address", address);

            Bind<IClient>()
                .To<ToDoServiceClient>()
                .WithConstructorArgument("address", address);
        }
    }
}