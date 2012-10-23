using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using EntitiesLibrary;
using EntitiesLibrary.Arguments.AddTask;
using EntitiesLibrary.Arguments.CompleteTask;
using EntitiesLibrary.Arguments.RenameTask;
using EntitiesLibrary.Arguments.SetDate;
using FluentAssertions;
using NSubstitute;
using Specifications.ServiceSpecifications;
using TaskManagerServiceLibrary.TaskManager;
using Xunit;

namespace TaskManagerServiceLibrary.Repositories
{
    public class MemoRepository : IRepository
    {
        private readonly ITaskMapper mapper;
        private static readonly List<ServiceTask> taskList = new List<ServiceTask>();
        private int currentId;

        public MemoRepository(ITaskMapper mapper)
        {
            this.mapper = mapper;
        }

        public int AddTask(AddTaskArgs args)
        {
            var serviceTask = new ServiceTask {Name = args.Name, DueDate = args.DueDate, Id = GetNewId()};

            taskList.Add(serviceTask);

            return serviceTask.Id;
        }

        public List<ContractTask> GetTasks(IServiceSpecification spec)
        {
            return taskList
                .Where(spec.IsSatisfied)
                .Select(mapper.ConvertToContract)
                .ToList();
        }

        public void Complete(CompleteTaskArgs args)
        {
            taskList.First(x => x.Id == args.Id).IsCompleted = true;
        }

        public void RenameTask(RenameTaskArgs args)
        {
            taskList.First(x => x.Id == args.Id).Name = args.Name;
        }

        public void SetTaskDueDate(SetDateArgs args)
        {
            taskList.First(x => x.Id == args.Id).DueDate = args.DueDate;
        }

        private int GetNewId()
        {
            Interlocked.Increment(ref currentId);
            return currentId;
        }
    }

    public class MemoRepositoryTests
    {
        private readonly ITaskMapper mapper = Substitute.For<ITaskMapper>();
        readonly MemoRepository repo;

        private readonly List<string> taskNames = new List<string> { "test task", "another task", "my task" };

        public MemoRepositoryTests()
        {
            repo = new MemoRepository(mapper);
        }

        [Fact]
        public void should_add_task_to_repo()
        {
            var task = new AddTaskArgs {DueDate = DateTime.Now, Name = "task1"};

            var result = repo.AddTask(task);

            result.Should().Be(1);
        }
    }
}