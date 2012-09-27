﻿using System.Collections.Generic;
using System.Collections.Generic;
using EntitiesLibrary;
using NSubstitute;
using Xunit;

namespace TaskConsoleClient.Manager
{
    class CommandManager : ICommandManager
    {
        private readonly IConnection conn;

        public CommandManager(IConnection conn)
        {
            this.conn = conn;
        }
        public ContractTask AddTask(ContractTask task)
        public List<ContractTask> GetAllTasks()
        {
            return conn.GetClient().AddTask(task); ;
        }

        public ContractTask GetTaskById(int id)
        {
            return conn.GetClient().GetTaskById(id);
        }

        public ContractTask Edit(ContractTask task)
        {
            return conn.GetClient().Edit(task);
        }

        public List<ContractTask> GetAllTasks()
        {
            return conn.GetClient().GetAllTasks();
        }
    }

    public class CommandManagerTests
    {
        readonly ContractTask task = new ContractTask();

        [Fact]
        public void should_send_add_task_to_service()
        {
        }
    }
}
