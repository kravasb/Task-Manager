using System;
using System.Collections.Generic;
using System.ServiceModel;
using EntitiesLibrary;
using FluentAssertions;
using NSubstitute;
using TaskManagerHost.TaskManager;
using Xunit;

namespace TaskManagerHost.WCFServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TaskManagerService : ITaskManagerService
    {
        private readonly IToDoList taskList;

        public TaskManagerService(IToDoList list)
        {
            taskList = list;
        }

        public int AddTask(string task)
        {
            return taskList.AddTask(task);
        }

        public ContractTask GetTaskById(int id)
        {
            return taskList.GetTaskById(id);
        }

        public List<ContractTask> GetAllTasks()
        {
            return taskList.GetAllTasks();
        }

        public bool MarkCompleted(int id)
        {
            return taskList.MarkCompleted(id);
        }

        public bool TestConnection()
        {
            return true;
        }
    }

    public class TaskManagerServiceTests
    {
        private readonly ITaskManagerService service;
        private readonly IToDoList list = Substitute.For<IToDoList>(); 

        public TaskManagerServiceTests()
        {
            service = new TaskManagerService(list);
        }

        [Fact]
        public void should_create_task_and_return_taskid()
        {
            list.AddTask("some task").Returns(1);
            var res = service.AddTask("some task");
            res.Should().Be(1);
        }

        [Fact]
        public void should_get_task_by_id_and_return_task()
        {
            var task = new ContractTask {Id = 1};
            list.GetTaskById(1).Returns(task);
            var res = service.GetTaskById(1);
            res.Should().Be(task);
        }

        [Fact]
        public void should_get_all_taasks()
        {
            var listTasks = new List<ContractTask>() {new ContractTask {Id = 1, Name = "some", IsCompleted = false}};
            list.GetAllTasks().Returns(listTasks);
            var res = service.GetAllTasks();
            res.Should().BeEquivalentTo(listTasks);
        }

        [Fact]
        public void should_send_id_receive_completed_value()
        {
            list.MarkCompleted(1).Returns(true);
            var res = service.MarkCompleted(1);
            res.Should().Be(true);
        }
    }
}