﻿using EntitiesLibrary;
using FluentAssertions;
using NSubstitute;
using TaskManagerService.DataBaseAccessLayer;
using Xunit;

namespace TaskManagerService.TaskManager
{
    public class ToDoList : IToDoList
    {
        private readonly ITaskFactory factory;
        private IRepository repository;

        public ToDoList(ITaskFactory factory, IRepository repository)
        {
            this.factory = factory;
            this.repository = repository;
        }

        public ITask AddTask(ITask task)
        {
            return null;
        }
    }

    public class ToDoListTests
    {
        private readonly ITask incomingTask = new ContractTask();
        private readonly ServiceTask expectedTask = new ServiceTask();
        private readonly ITaskFactory factory = Substitute.For<ITaskFactory>();
        private IRepository repository = Substitute.For<IRepository>();


        [Fact]
        public void todolist_asks_factory_for_new_task_and_saves_list()
        {
            var list = new ToDoList(factory, repository);
            factory.Create(incomingTask).Returns(expectedTask);

            list.AddTask(incomingTask);
            repository.Received().SaveTask(expectedTask);

        }
    }
}