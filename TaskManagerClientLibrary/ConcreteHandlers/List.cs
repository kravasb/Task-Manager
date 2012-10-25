using System;
using System.Collections.Generic;
using System.IO;
using ConnectToWcf;
using EntitiesLibrary;
using EntitiesLibrary.CommandArguments;
using NSubstitute;
using Specifications.ClientSpecification;
using TaskManagerClientLibrary.ConcreteHandlers.TaskFormatter;
using Xunit;

namespace TaskManagerClientLibrary.ConcreteHandlers
{
    public class List : ICommand
    {
        public string Name { get { return GetType().Name.ToLower(); } }
        public string Description { get; private set; }
        private readonly IClientConnection client;
        private readonly ArgumentConverter<ListArgs> converter;
        private readonly TextWriter textWriter;
        private readonly ITaskFormatterFactory taskFormatterFactory;

        public List(ArgumentConverter<ListArgs> converter, TextWriter textWriter,
                    ITaskFormatterFactory taskFormatterFactory, IClientConnection client)
        {
            Description = "Displays list of all tasks or single task, specified by ID.";
            this.converter = converter;
            this.textWriter = textWriter;
            this.taskFormatterFactory = taskFormatterFactory;
            this.client = client;
        }

        private void PrintWithFormatter(List<ContractTask> list, ITaskFormatter formatter)
        {
            textWriter.WriteLine(formatter.Show(list));
        }

        public void Execute(List<string> argument)
        {
            var listArgs = converter.Convert(argument);
            var data = GetClientSpecification(listArgs);

            var tasks = client.GetTasks(data);
            var formatter = tasks.Count > 1 ? taskFormatterFactory.GetListFormatter() : taskFormatterFactory.GetSingleFormatter();

            PrintWithFormatter(tasks, formatter);
        }

        private IClientSpecification GetClientSpecification(ListArgs listArgs)
        {
            IClientSpecification data;

            if (listArgs.Date != default(DateTime) && listArgs.Id == 0)
                data = new ListByDate { Data = listArgs.Date };

            else if (listArgs.Date == default(DateTime) && listArgs.Id != null)
                data = new ListSingle { Data = listArgs.Id.Value };

            else data = new ListAll { Data = null };
            return data;
        }
    }

    public class ListTests
    {
        private readonly IClientConnection connection = Substitute.For<IClientConnection>();
        private readonly ArgumentConverter<ListArgs> converter = Substitute.For<ArgumentConverter<ListArgs>>();
        private readonly ITaskFormatterFactory formatter = Substitute.For<ITaskFormatterFactory>();
        private IClientSpecification data;
        private readonly List list;

        public ListTests()
        {
            list = new List(converter, new StringWriter(), formatter, connection);
        }

        [Fact]
        public void should_get_all_commands()
        {
            data = new ListAll();
            var input = new List<string> { "153" };
            converter.Convert(input).Returns(new ListArgs { Id = 153 });
            connection.GetTasks(data).ReturnsForAnyArgs(new List<ContractTask>());

            list.Execute(input);
            connection.ReceivedWithAnyArgs().GetTasks(data);
        }
        [Fact]
        public void should_get_one_command_by_id()
        {
            data = new ListSingle();
            var input = new List<string>();
            connection.GetTasks(data).ReturnsForAnyArgs(new List<ContractTask>());
            converter.Convert(input).Returns(new ListArgs { Id = null });

            list.Execute(input);
            connection.ReceivedWithAnyArgs().GetTasks(data);
        }
        [Fact]
        public void should_get_one_command_by_date()
        {
            data = new ListByDate();
            var input = new List<string>();
            connection.GetTasks(data).ReturnsForAnyArgs(new List<ContractTask>());
            converter.Convert(input).Returns(new ListArgs { Id = 0, Date = DateTime.Now });

            list.Execute(input);
            connection.ReceivedWithAnyArgs().GetTasks(data);
        }
    }
}