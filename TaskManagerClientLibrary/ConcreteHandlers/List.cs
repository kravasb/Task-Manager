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
    public class List : Command<ListArgs>
    {
        private readonly ITaskFormatterFactory taskFormatterFactory;
        private IClientConnection Client { get; set; }

        public List(ArgumentConverter<ListArgs> converter, TextWriter textWriter,
                    ITaskFormatterFactory taskFormatterFactory, IClientConnection client)
            : base(converter, textWriter)
        {
            Description = "Displays list of all tasks or single task, specified by ID.";
            this.taskFormatterFactory = taskFormatterFactory;
            Client = client;
        }

        private void PrintWithFormatter(List<ContractTask> list, ITaskFormatter formatter)
        {
            OutText(formatter.Show(list));
        }

        public override void Execute(List<string> argument)
        {
            var listArgs = converter.Convert(argument);
            IClientSpecification data;

            if (listArgs.Date != default(DateTime) && listArgs.Id == 0)
                data = new ListByDate { Date = listArgs.Date };

            else if (listArgs.Date == default(DateTime) && listArgs.Id != null)
                data = new ListSingle { Id = listArgs.Id.Value };

            else data = new ListAll ();

            var tasks = Client.GetTasks(data);
            var formatter = tasks.Count > 1 ? taskFormatterFactory.GetListFormatter() : taskFormatterFactory.GetSingleFormatter();

            PrintWithFormatter(tasks, formatter);
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