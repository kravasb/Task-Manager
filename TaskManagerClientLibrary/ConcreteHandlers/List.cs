using System;
using System.Collections.Generic;
using System.IO;
using ConnectToWcf;
using EntitiesLibrary;
using FluentAssertions;
using NSubstitute;
using TaskManagerServiceLibrary;
using Xunit;
using System.Linq;

namespace TaskManagerClientLibrary.ConcreteHandlers
{
    public class List : Command<int?>
    {
        private readonly TaskFormatterFactory taskFormatterFactory;

        public List(IClientConnection client, ArgumentConverter<int?> converter, TextWriter textWriter, TaskFormatterFactory taskFormatterFactory)
            : base(client, converter, textWriter)
        {
            this.taskFormatterFactory = taskFormatterFactory;
        }

        protected override void ExecuteWithGenericInput(int? input)
        {
            if (input == null)
                ExecutePr(s => s.GetAllTasks(), taskFormatterFactory.GetListFormatter());
            else
                ExecutePr(s => s.GetTaskById(input.Value), taskFormatterFactory.GetSingleFormatter());
                tasks = (input == null)
            catch (TaskNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

        private void ExecutePr(Func<IClientConnection, List<ContractTask>> func, ITaskFormatter formatter)

            var tasks = func(client);
            OutText(formatter.Show(tasks));
            else if (tasks.Count == 1)
            {
                Console.WriteLine();
                Console.WriteLine("ID: {0}" + delim + "Name: {1}" + delim + "Completed: {2}", tasks[0].Id, tasks[0].Name,
                                  tasks[0].IsCompleted ? "+" : "-");
        }
                tasks.ForEach(x => Console.WriteLine(" {0}\t|" + delim + "{1}" + delim + "\t|\t{2}", x.Id, x.Name, x.IsCompleted ? "+" : "-"));

    }

    public class ListTests
    {
        private readonly ArgumentConverter<int?> converter = Substitute.For<ArgumentConverter<int?>>();
        private readonly IClientConnection client = Substitute.For<IClientConnection>();
        private readonly ArgumentConverter<int?> converter = Substitute.For<ArgumentConverter<int?>>();
        private readonly SingleTaskFormatter formatter1 = Substitute.For<SingleTaskFormatter>();
        private readonly ListTaskFormatter formatter2 = Substitute.For<ListTaskFormatter>();
        private readonly TaskFormatterFactory taskFormatterFactory;
        private readonly List list;

        //public ListTests()
        //{
            taskFormatterFactory = Substitute.For<TaskFormatterFactory>(formatter1, formatter2);
            list = new List(client, converter, new StringWriter(), taskFormatterFactory);
            taskFormatterFactory.GetSingleFormatter().Returns(formatter1);
            taskFormatterFactory.GetListFormatter().Returns(formatter2);
        //}
        //public void should_check_receiving_one_task()
        //{
        //    var taskList = new List<ContractTask> { new ContractTask { Id = 1, Name = "some", IsCompleted = false } };
        //    client.GetTaskById(1).Returns(taskList);
        //    list.ExecuteWithGenericInput("1");
        //    formatter1.Received().Show(taskList);
        //}

        //[Fact]
        //public void should_check_receiving_all_task()
        //{
        //    var taskList = new List<ContractTask>
        //                       {
        //                           new ContractTask { Id = 1, Name = "task1", IsCompleted = false },
        //                           new ContractTask{Id = 2, Name = "task2", IsCompleted = true}
        //                       };
        //    client.GetAllTasks().Returns(taskList);
        //    list.ExecuteWithGenericInput("");
        //    formatter2.Received().Show(taskList);
        //}
        [Fact]
        public void if_input_is_null_should_get_all_tasks()
        }

        [Fact]
        public void if_get_task_by_id_should_call_specifical_printer_for_it()
        {
            converter.Convert("1").Returns(1);

            converter.Convert(id).Returns(int.Parse(id));
            handler.Execute(id);

            client.Received().GetTaskById(5);
        }

        [Fact]
        public void should_execute_in_client_receiving_show_all_tasks()
        {
            converter.Convert("").Returns((int?)null);
            list.Execute("");
            client.Received().GetAllTasks();
        }

        [Fact]
        {
            var sb = new StringBuilder();
            Console.SetOut(new StringWriter(sb));

        public void should_check_receiving_all_task()
        {
            converter.Convert("").Returns((int?)null);
            var taskList = new List<ContractTask>
                               {
                                   new ContractTask { Id = 1, Name = "task1", IsCompleted = false },
                                   new ContractTask{Id = 2, Name = "task2", IsCompleted = true}
                               };

            converter.Convert("5").Returns(5);
            formatter2.Received().Show(taskList);
        }

        [Fact]
        public void should_execute_in_client_receiving_show_one_tasks()
        {
            converter.Convert("1").Returns(1);
            list.Execute("1");
            client.Received().GetTaskById(1);
        }
    }

            sb.ToString().ShouldBeEquivalentTo("Task not found: (Id = 5)\r\n");
    }
}
