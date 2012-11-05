using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConnectToWcf;
using EntitiesLibrary;
using EntitiesLibrary.CommandArguments;
using FluentAssertions;
using NSubstitute;
using TaskManagerClientLibrary.ConcreteCommands.TaskFormatter;
using Xunit;

namespace TaskManagerClientLibrary.ConcreteCommands
{
    public class ListCommand : ICommand
    {
        public string Name { get { return GetType().Name.Split(new[] { "Command" }, StringSplitOptions.None)[0].ToLower(); } }
        public string Description { get; private set; }
        private readonly IClient client;
        private readonly ITaskFormatterFactory factory;
        private readonly TaskArgsConverter converter;
        private readonly TextWriter textWriter;

        public ListCommand(TaskArgsConverter converter, TextWriter textWriter,
                    IClient client, ITaskFormatterFactory factory)
        {
            Description = "Displays list of all tasks or single task, specified by ID.";
            this.converter = converter;
            this.textWriter = textWriter;

            this.client = client;
            this.factory = factory;
        }

        private void PrintWithFormatter(List<ClientTask> list, ITaskFormatter formatter)
        {
            textWriter.WriteLine(formatter.ToFormatString(list));
        }

        public void Execute(List<string> argument)
        {
            var listArgs = GetClientSpecification(argument);
            var formatter = factory.GetFormatter(listArgs);
            var tasks = client.GetTasks(listArgs);
            PrintWithFormatter(tasks, formatter);
        }

        private IListCommandArguments GetClientSpecification(List<string> source)
        {
            var types = new List<Type> { typeof(ListByDateTaskArgs), typeof(ListSingleTaskArgs), typeof(ListAllTaskArgs) };

            return converter.Convert(source, types) as IListCommandArguments;
        }
    }

    public class ListTests
    {
        private readonly IClient connection = Substitute.For<IClient>();
        private readonly TaskArgsConverter converter = Substitute.For<TaskArgsConverter>();
        private IListCommandArguments data;
        private readonly StringBuilder sb = new StringBuilder();
        readonly StringWriter writer;
        private readonly ListCommand list;
        private readonly ITaskFormatterFactory factory = Substitute.For<ITaskFormatterFactory>();

        public ListTests()
        {
            writer = new StringWriter(sb);
            list = new ListCommand(converter, writer, connection, factory);
            //list = new ListCommand(converter, writer, connection);
        }

        [Fact]
        public void property_name_should_be_classname()
        {
            list.Name.Should().Be("list");
        }

        [Fact]
        public void should_get_all_commands()
        {
            data = new ListAllTaskArgs();
            var input = new List<string> { "153" };
            converter.Convert(input, new List<Type> { typeof(ListAllTaskArgs) }).Returns(data);
            connection.GetTasks(data).ReturnsForAnyArgs(new List<ClientTask>());

            list.Execute(input);
            connection.ReceivedWithAnyArgs().GetTasks(data);
        }
        [Fact]
        public void should_get_one_command_by_id()
        {
            data = new ListSingleTaskArgs();
            var input = new List<string>();
            connection.GetTasks(data).ReturnsForAnyArgs(new List<ClientTask>());
            converter.Convert(input, new List<Type> { typeof(ListSingleTaskArgs) }).Returns(new ListSingleTaskArgs());

            list.Execute(input);
            connection.ReceivedWithAnyArgs().GetTasks(data);
        }
        [Fact]
        public void should_get_one_command_by_date()
        {
            data = new ListByDateTaskArgs();
            var input = new List<string>();
            connection.GetTasks(data).ReturnsForAnyArgs(new List<ClientTask>());
            converter.Convert(input, new List<Type> { typeof(ListByDateTaskArgs) }).Returns(data);

            list.Execute(input);
            connection.ReceivedWithAnyArgs().GetTasks(data);
        }

        [Fact]
        public void should_print_info_on_required_tasks()
        {

            //var args = new ListTaskArgs { Id = 153 };
            //var input = new List<string> { "153" };
            //var listPackage = new List<ClientTask> { new ClientTask { DueDate = DateTime.Now, Id = 1, IsCompleted = true } };

            //var formatter = Substitute.For<ITaskFormatter>();
            //data = Substitute.For<IClientSpecification>();

            //converter.Convert(input).Returns(args);
            //factory.GetClientSpecification(args).Returns(data);
            //factory.GetFormatter(data).Returns(formatter);
            //formatter.ToFormatString(listPackage).Returns("hello world");

            //connection.GetTasks(data).Returns(listPackage);

            //list.Execute(input);
            //sb.ToString().Should().BeEquivalentTo("hello world\r\n");
        }
    }
}